#pragma warning disable CS8600, CS8604, CS8618, CS8620

using System;  // Keep for .NET 4.6
using System.Collections.Concurrent;
using System.Collections.Generic; // Keep for .NET 4.6
using System.Diagnostics;
using System.IO;
using System.Linq; // Keep for .NET 4.6
using System.Threading;
using System.Threading.Tasks; // Keep for ZWCAD

namespace Shared.Controllers
{
    internal class FileWatcherController : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers 
            = new ConcurrentDictionary<string, FileSystemWatcher>();
        private readonly ConcurrentDictionary<string, HashSet<string>> _files
            = new ConcurrentDictionary<string, HashSet<string>>();

        public event Action<string, string> FileCreated;
        public event Action<string, string> FileDeleted;
        public event Action<string, string, string> FileRenamed;

        public IReadOnlyDictionary<string, HashSet<string>> Files
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _files.ToDictionary(p => p.Key,
                        p => new HashSet<string>(p.Value));
                }
                finally
                { _lock.ExitReadLock(); }
            }
        }

        public void AddDirectory(string lsPath)
        {
            if (lsPath == null) return;
            if (!Directory.Exists(lsPath))
                throw new DirectoryNotFoundException(lsPath);
            if (_watchers.ContainsKey(lsPath)) return;
            HashSet<string> files = new HashSet<string>(Directory.GetFiles(lsPath)
                .Select(System.IO.Path.GetFileName));
            UpdateFiles(lsPath, files);
            _lock.EnterWriteLock();
            try
            {
                _files[lsPath] = files;
            } finally { _lock.ExitWriteLock(); }
            FileSystemWatcher watcher = new FileSystemWatcher(@lsPath)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                IncludeSubdirectories = false,
                InternalBufferSize = 64 * 1024 // If there are many changes in a short time, the buffer can overflow.
                                               // This causes the component to lose track of changes in the directory,
                                               // and it will only provide blanket notification.
                                               // Increasing the size of the buffer can prevent missing file system change events. 
            };
#if NET8_0_OR_GREATER
            string[] filters = [
                "*.xhb", "*.shb", // Route 
            ];
            //foreach (var filter in filters)
            //    watcher.Filters.Add(filter);
#endif
            watcher.Created += (_, args) => OnFileCreated(lsPath, args.Name);
            watcher.Deleted += (_, args) => OnFileDeleted(lsPath, args.Name);
            watcher.Renamed += (_, args) => OnFileRenamed(lsPath, args.Name, args.OldName);
            watcher.EnableRaisingEvents = true;
            _watchers[lsPath] = watcher;
            Debug.WriteLine($"[@] Adding directory: {lsPath} to watcher");
        }

        public void RemoveDirectory(string lsPath)
        {
            if (lsPath == null) return;
            if (_watchers.TryRemove(lsPath, out FileSystemWatcher watcher))
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _lock.EnterWriteLock();
            try
            {
                _files.TryRemove(lsPath, out _);
            } finally { _lock.ExitWriteLock(); }
        }

        private void OnFileCreated(string lsPath, string fileName)
        {
            Debug.WriteLine($"[@] OnFileCreated: {lsPath}{fileName}");
            if (fileName == null) 
                return;
            _lock.EnterWriteLock();
            try
            {
                if (_files.TryGetValue(lsPath, out HashSet<string> files))
                    files.Add(fileName);
            } finally { _lock.ExitWriteLock(); }
#if ZWCAD
            ThreadPool.QueueUserWorkItem(_ => FileCreated?.Invoke(lsPath, fileName));
#else
            FileCreated?.Invoke(lsPath, fileName);
#endif
        }

        private void OnFileDeleted(string lsPath, string fileName)
        {
            Debug.WriteLine($"[@] OnFileDeleted: {lsPath}{fileName}");
            if (fileName == null) 
                return;
            _lock.EnterWriteLock();
            try
            {
                if (_files.TryGetValue(lsPath, out HashSet<string> files))
                    files.Remove(fileName);
            } finally { _lock.ExitWriteLock(); }
#if ZWCAD
            ThreadPool.QueueUserWorkItem(_ => FileDeleted?.Invoke(lsPath, fileName));
#else
            FileDeleted?.Invoke(lsPath, fileName);
#endif
        }

        private void OnFileRenamed(string lsPath, string fileName, string oldName)
        {
            Debug.WriteLine($"[@] OnFileDeleted: {lsPath}{fileName}->{oldName}");
            if (fileName == null || oldName == null) 
                return;
            _lock.EnterWriteLock();
            try
            {
                if (_files.TryGetValue(lsPath, out HashSet<string> files))
                {
                    files.Remove(oldName);
                    files.Add(fileName);
                }
            } finally { _lock.ExitWriteLock(); }
#if ZWCAD
            ThreadPool.QueueUserWorkItem(_ => FileRenamed?.Invoke(lsPath, fileName, oldName));
#else
            FileRenamed?.Invoke(lsPath, fileName, oldName);
#endif
        }

        private void UpdateFiles(string lsPath, HashSet<string> newFiles)
        {
            _lock.EnterWriteLock();
            try
            {
                _files[lsPath] = newFiles;
            } finally { _lock.ExitWriteLock(); }
        }

        public void Dispose()
        {
            foreach (KeyValuePair<string, FileSystemWatcher> watcher in _watchers)
            {
                if (watcher.Value != null)
                {
                    watcher.Value.EnableRaisingEvents = false;
                    watcher.Value.Dispose();
                }
            }
            _watchers.Clear();
            _files.Clear();
            _lock.Dispose();
        }
    }
}
