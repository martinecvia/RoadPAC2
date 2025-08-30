#pragma warning disable CS1998, CS8600, CS8604, CS8618, CS8620

using System;  // Keep for .NET 4.6
using System.Collections.Concurrent;
using System.Collections.Generic; // Keep for .NET 4.6
using System.IO;
using System.Linq; // Keep for .NET 4.6
using System.Threading; // Keep for .NET 4.6

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if !ZWCAD
using Autodesk.AutoCAD.ApplicationServices;
#else
using System.Threading.Tasks;
using ZwSoft.ZwCAD.ApplicationServices;
#endif
#endregion

namespace Shared.Controllers
{
    // https://www.keanw.com/2015/03/autocad-2016-calling-commands-from-external-events-using-net.html
    internal class FileWatcherController : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers 
            = new ConcurrentDictionary<string, FileSystemWatcher>();
        private readonly ConcurrentDictionary<string, HashSet<string>> _files
            = new ConcurrentDictionary<string, HashSet<string>>();

        public event Action<string, string> FileCreated;
        public event Action<string, string> FileChanged;
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

        private DocumentCollection _context;
        internal FileWatcherController(DocumentCollection context)
            => _context = context
#if !ZWCAD && !NET8_0_OR_GREATER // This is unfortunate, but it is what it is. AutoCAD part won't work without context provided
            ?? throw new ArgumentNullException(nameof(context))
#endif
            ;

        public void AddDirectory(string lsPath)
        {
            if (lsPath == null) return;
            if (_watchers.ContainsKey(lsPath)) return;
            if (!Directory.Exists(lsPath))
                throw new DirectoryNotFoundException(lsPath);
            HashSet<string> files = new HashSet<string>(Directory.GetFiles(lsPath)
                .Select(System.IO.Path.GetFileName));
            _lock.EnterWriteLock();
            try
            {
                _files[lsPath] = files;
            } finally { _lock.ExitWriteLock(); }
            FileSystemWatcher watcher = new FileSystemWatcher(lsPath, "*.*");
#if NET8_0_OR_GREATER
            string[] filters = [
                "*.xhb", "*.shb", // Route 
            ];
            //foreach (var filter in filters)
            //    watcher.Filters.Add(filter);
#endif
            watcher.Created += (o, s) => OnFileCreated(lsPath, s.Name);
            watcher.Changed += (o, s) => OnFileChanged(lsPath, s.Name, s.ChangeType);
            watcher.Deleted += (o, s) => OnFileDeleted(lsPath, s.Name);
            watcher.Renamed += (o, s) => OnFileRenamed(lsPath, s.Name, s.OldName);
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = false;
            watcher.InternalBufferSize = 64 * 1024; // If there are many changes in a short time, the buffer can overflow.
                                                    // This causes the component to lose track of changes in the directory,
                                                    // and it will only provide blanket notification.
                                                    // Increasing the size of the buffer can prevent missing file system change events. 
            _watchers[lsPath] = watcher;
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

        private async void OnFileCreated(string lsPath, string fileName)
        {
            if (fileName == null) 
                return;
            _lock.EnterWriteLock();
            try
            {
                if (_files.TryGetValue(lsPath, out HashSet<string> files))
                    files.Add(fileName);
            } finally { _lock.ExitWriteLock(); }
#if !ZWCAD && !NET8_0_OR_GREATER
            if (_context == null)
                _context = Application.DocumentManager;
            await _context?.ExecuteInCommandContextAsync(async (_) 
                => FileCreated?.Invoke(lsPath, fileName), null);
#else
            ThreadPool.QueueUserWorkItem(_ => FileCreated?.Invoke(lsPath, fileName));
#endif
        }

        int x = 0;
        private async void OnFileChanged(string lsPath, string fileName, WatcherChangeTypes change)
        {
            x++;
            //Interlocked.Increment(ref x);

            if (change != WatcherChangeTypes.Changed)
                return;
#if !ZWCAD && !NET8_0_OR_GREATER
            if (_context == null)
                _context = Application.DocumentManager;
            await _context?.ExecuteInCommandContextAsync(async (_) 
                => FileChanged?.Invoke(lsPath, fileName), null);
#else
            ThreadPool.QueueUserWorkItem(_ => FileChanged?.Invoke(lsPath, fileName));
#endif
        }

        private async void OnFileDeleted(string lsPath, string fileName)
        {
            if (fileName == null) 
                return;
            _lock.EnterWriteLock();
            try
            {
                if (_files.TryGetValue(lsPath, out HashSet<string> files))
                    files.Remove(fileName);
            } finally { _lock.ExitWriteLock(); }
#if !ZWCAD && !NET8_0_OR_GREATER
            if (_context == null)
                _context = Application.DocumentManager;
            await _context?.ExecuteInCommandContextAsync(async (_) 
                => FileDeleted?.Invoke(lsPath, fileName), null);
#else
            ThreadPool.QueueUserWorkItem(_ => FileDeleted?.Invoke(lsPath, fileName));
#endif
        }

        private async void OnFileRenamed(string lsPath, string fileName, string oldName)
        {
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
#if !ZWCAD && !NET8_0_OR_GREATER
            if (_context == null)
                _context = Application.DocumentManager;
            await _context?.ExecuteInCommandContextAsync(async (_) 
                => FileRenamed?.Invoke(lsPath, fileName, oldName), null);
#else
            ThreadPool.QueueUserWorkItem(_ => FileRenamed?.Invoke(lsPath, fileName, oldName));
#endif
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