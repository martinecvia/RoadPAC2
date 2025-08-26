using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Shared.Controllers
{
    internal class FileWatcherController : IDisposable
    {
        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers 
            = new ConcurrentDictionary<string, FileSystemWatcher>();
        private readonly ConcurrentDictionary<string, HashSet<string>> _files
            = new ConcurrentDictionary<string, HashSet<string>>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

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
            if (!Directory.Exists(lsPath))
                throw new DirectoryNotFoundException(lsPath);
            lsPath = Path.GetFullPath(lsPath);
            if (_watchers.ContainsKey(lsPath)) return;
            HashSet<string> files = new HashSet<string>(Directory.GetFiles(lsPath)
                .Select(System.IO.Path.GetFileName));
            _lock.EnterWriteLock();
            try
            {
                _files[lsPath] = files;
            } finally { _lock.ExitWriteLock(); }
            FileSystemWatcher watcher = new FileSystemWatcher(@lsPath)
            {
                NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size
            };
            // Events must be handled before setting: EnableRaisingEvents = true
            watcher.Created += (_, args) => OnFileCreated(lsPath, args.Name);
            watcher.Deleted += (_, args) => OnFileDeleted(lsPath, args.Name);
            watcher.Renamed += (_, args) => OnFileRenamed(lsPath, args.Name, args.OldName);
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;
            _watchers[lsPath] = watcher;
        }

        public void RemoveDirectory(string lsPath)
        {
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
            Debug.WriteLine($"[@] OnFileCreated: {lsPath}, {fileName}");
            if (fileName == null) 
                return;
            _lock.EnterWriteLock();
            try
            {
                if (_files.TryGetValue(lsPath, out HashSet<string> files))
                    files.Add(fileName);
            } finally { _lock.ExitWriteLock(); }
            FileCreated?.Invoke(lsPath, fileName);
        }

        private void OnFileDeleted(string lsPath, string fileName)
        {
            Debug.WriteLine($"[@] OnFileDeleted: {lsPath}, {fileName}");
            if (fileName == null) 
                return;
            _lock.EnterWriteLock();
            try
            {
                if (_files.TryGetValue(lsPath, out HashSet<string> files))
                    files.Remove(fileName);
            }
            finally { _lock.ExitWriteLock(); }
            FileDeleted?.Invoke(lsPath, fileName);
        }

        private void OnFileRenamed(string lsPath, string fileName, string oldName)
        {
            Debug.WriteLine($"[@] OnFileDeleted: {lsPath}, {fileName}->{oldName}");
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
            }
            finally { _lock.ExitWriteLock(); }
            FileRenamed?.Invoke(lsPath, fileName, oldName);
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
