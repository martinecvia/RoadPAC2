#pragma warning disable CS1998

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shared.Controllers.Models.Project;

namespace Shared.Controllers
{
    // https://adndevblog.typepad.com/autocad/2012/06/use-thread-for-background-processing.html
    public class ProjectController : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly ConcurrentDictionary<string, HashSet<string>> _changes
            = new ConcurrentDictionary<string, HashSet<string>>();

        private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        // Project related
        private readonly ConcurrentDictionary<string, HashSet<ProjectFile>> _project 
            = new ConcurrentDictionary<string, HashSet<ProjectFile>>();
        public event Action<string, ProjectFile, WatcherChangeTypes> ProjectChanged;
        public string CurrentWorkingDirectory { get; set; }

        public IReadOnlyDictionary<string, HashSet<ProjectFile>> ProjectFiles
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _project.ToDictionary(p => p.Key,
                        p => new HashSet<ProjectFile>(p.Value));
                }
                finally
                { _lock.ExitReadLock(); }
            }
        }

        [RPInfoOut]
        public HashSet<ProjectFile> GetRoutes() => GetRoutes(CurrentWorkingDirectory);

        [RPInfoOut]
        public HashSet<ProjectFile> GetRoutes(string lsPath)
        {
            if (string.IsNullOrEmpty(lsPath))
                return new HashSet<ProjectFile>();
            if (!ProjectFiles.TryGetValue(lsPath, out var files))
                return new HashSet<ProjectFile>();
            return new HashSet<ProjectFile>(files.Where(f => f != null 
                && !string.IsNullOrEmpty(f.File)
                && f.Flag == FClass.Route));
        }

        [RPInfoOut]
        public HashSet<ProjectFile> GetRoute(string lsPath, string routeName)
        {
            if (string.IsNullOrEmpty(lsPath) || string.IsNullOrEmpty(routeName))
                return new HashSet<ProjectFile>();
            if (!ProjectFiles.TryGetValue(lsPath, out var files))
                return new HashSet<ProjectFile>();
            routeName = Path.GetFileNameWithoutExtension(routeName);
            var result = new HashSet<ProjectFile>(files.Where(f => f != null
                && !string.IsNullOrEmpty(f.File)
                && f.File.StartsWith(routeName, StringComparison.OrdinalIgnoreCase)
                && !(f is BaseProjectXml)));
            var awaits = files.Where(f => f != null
                && !string.IsNullOrEmpty(f.File)
                && f is BaseProjectXml xml && string.Equals(xml.Route, routeName, StringComparison.OrdinalIgnoreCase));
            foreach (var xml in awaits)
                result.Add(xml);
            return result;
        }

        public class ProjectFile
        {
            public string File { get; internal set; }
            public string Path { get; internal set; }
            public FClass Flag { get; internal set; } = FClass.None;
            public DateTime LastUpdatedAt { get; internal set; }
            /// <summary>
            /// Base initialization. Does nothing by default.
            /// Derived classes can override to provide async initialization logic.
            /// </summary>
            public virtual Task BeginInit() => Task.CompletedTask;
            public override string ToString()
                => $"{nameof(ProjectFile)}(File={File}, Path={Path}, Flag={Flag})";
        }

        [Flags]
        [RPInternalUseOnly]
        public enum FClass : int
        {
            None = 0,
            // Mappings
            Profile = 1 << 0,  // Niveleta
            Route = 1 << 1,  // Trasa / Směrové řešení
            Corridor = 1 << 2,  // Koridor
            Survey = 1 << 3,  // Vytyčení
            CrossSection = 1 << 4,  // Příčné řezy
            CombinedCrossSections = 1 << 5,  // Spojené příčné řezy
            IFC = 1 << 6,  // IFC Podklady
            // Types
            Listing = 1 << 7,
            Calculation = 1 << 8,
            Result = 1 << 9,
            Xml = 1 << 10,
            // Should be expanded
            Multi = 1 << 11,
        }

        // We don't really want to expose our contructor,
        // it's supposed to be initialized during app build process
        internal ProjectController()
        {
            if (RPApp.FileWatcher != null)
            {
                RPApp.FileWatcher.FileCreated += ProcessFileChanged;
                RPApp.FileWatcher.FileChanged += ProcessFileChanged;
                RPApp.FileWatcher.FileDeleted += ProcessFileChanged;
                RPApp.FileWatcher.FileRenamed += ProcessFileRenamed;
                RefreshProject(RPApp.FileWatcher.Files);
            }
        }

        [RPInfoOut]
        public void RefreshProject(IReadOnlyDictionary<string, HashSet<string>> files)
        {
            if (RPApp.FileWatcher == null) return;
            foreach (KeyValuePair<string, HashSet<string>> pair in RPApp.FileWatcher?.Files)
            {
                _changes.AddOrUpdate(pair.Key,
                    _ => new HashSet<string>(pair.Value),
                    (_, set) =>
                    {
                        lock (set)
                        {
                            foreach (var item in pair.Value)
                                set.Add(item);
                        }
                        return set;
                    });
            }
        }

        [RPInternalUseOnly]
        internal void BeginInit()
            => Task.Run(ProcessChangesInBackground, _cts.Token);

        [RPPrivateUseOnly]
        private void ProcessFileChanged(string lsPath, string fileName)
        {
            _changes.AddOrUpdate(
                lsPath,
                _ => new HashSet<string> { fileName },
                (_, set) => { lock (set) set.Add(fileName); return set; });
        }

        [RPPrivateUseOnly]
        private void ProcessFileRenamed(string lsPath, string fileName, string oldName)
        {
            _changes.AddOrUpdate(
                lsPath,
                _ => new HashSet<string> { fileName },
                (_, set) => { lock (set) set.Add(fileName); return set; });
        }

        // Just one processor at a time
        private volatile bool _isProcessing;

        [RPPrivateUseOnly]
        private async Task ProcessChangesInBackground()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                await Task.Delay(_interval, _cts.Token);
                if (_changes.IsEmpty || _isProcessing)
                    continue;
                try
                {
                    _isProcessing = true;
                    var snapshot = _changes.ToDictionary(
                        kvp => kvp.Key,
                        kvp => { lock (kvp.Value) return kvp.Value; });
                    _changes.Clear();
                    var InitTasks = snapshot.SelectMany(pair
                        => pair.Value.Select(fileName => ProcessFile(pair.Key, fileName)));
                    await Task.WhenAll(InitTasks);
                }
                finally
                {
                    _isProcessing = false;
                }
            }
        }

        [RPPrivateUseOnly]
        private async Task ProcessFile(string lsPath, string fileName)
        {
            ProjectFile result = null;
            WatcherChangeTypes change = WatcherChangeTypes.Changed;
            var fullPath = Path.Combine(lsPath, fileName);
            _lock.EnterWriteLock();
            try
            {
                if (!_project.TryGetValue(lsPath, out HashSet<ProjectFile> files))
                    files = _project[lsPath] = new HashSet<ProjectFile>();
                var existing = files.FirstOrDefault(f => f.File.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                if (File.Exists(fullPath))
                {
                    // We fetch updated date early, as change can happen anytime dureing fetch process
                    DateTime lastUpdatedAt = File.GetLastWriteTimeUtc(fullPath);
                    string extension = Path.GetExtension(fileName)?.TrimStart('.').ToUpper() ?? "";
                    if (existing != null)
                    {
                        // Just update it's values
                        existing.LastUpdatedAt = lastUpdatedAt;
                        result = existing;
                        change = WatcherChangeTypes.Changed;
                        await existing.BeginInit();
                    }
                    else if (ProjectFileFactory.TryGetValue(extension, out var factory))
                    {
                        var newFactory = factory();
                        newFactory.Path = lsPath;
                        newFactory.File = fileName;
                        newFactory.LastUpdatedAt = lastUpdatedAt;
                        // BeginInit must happen before adding to set
                        await newFactory.BeginInit();
                        result = newFactory;
                        change = WatcherChangeTypes.Created;
                        files.Add(newFactory);
                    }
                    else
                    {
                        // File does not have Factory
                        // Presumably unmapped file, ignoring it
                    }
                } 
                else
                {
                    // File have been removed
                    if (existing != null)
                    {
                        result = existing;
                        change = WatcherChangeTypes.Deleted;
                        files.Remove(existing);
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            // Signalize event that something in project changed
#if !ZWCAD && !NET8_0_OR_GREATER
            ProjectChanged?.Invoke(lsPath, result, change);
#else
            ThreadPool.QueueUserWorkItem(_ => ProjectChanged?.Invoke(lsPath, result, change));
#endif
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        [RPPrivateUseOnly]
        private static readonly Dictionary<string, Func<ProjectFile>> ProjectFileFactory = new Dictionary<string, Func<ProjectFile>>()
        {
            // Koridor
            { "V43", () => new ProjectFile() { Flag = FClass.Corridor } },
            { "L43", () => new ProjectFile() { Flag = FClass.Corridor | FClass.Listing } },
            { "L43A", () => new ProjectFile() { Flag = FClass.Corridor | FClass.Listing } },
            // Vytyčení
            { "V47", () => new SurveyFile() { Flag = FClass.Survey | FClass.Xml | FClass.Multi } },
            { "V47X", () => new SurveyFile() { Flag = FClass.Survey | FClass.Xml | FClass.Multi } },
            // Příčné řezy
            { "V51", () => new ProjectFile() { Flag = FClass.Corridor | FClass.CrossSection } },
            { "L51", () => new ProjectFile() { Flag = FClass.Corridor | FClass.CrossSection | FClass.Listing } },
            { "L51A", () => new ProjectFile() { Flag = FClass.Corridor | FClass.CrossSection | FClass.Listing } },
            // Spojené příčné řezy
            { "V91", () => new CombinedCrossSectionsFile() { Flag = FClass.CombinedCrossSections | FClass.Xml | FClass.Multi } },
            // IFC Podklady
            { "V94", () => new IFCFile() { Flag = FClass.IFC | FClass.Xml | FClass.Multi } },
            // Niveleta
            { "XNI", () => new ProjectFile() { Flag = FClass.Profile } },
            // Trasa / Směrové řešení
            { "XHB", () => new ProjectFile() { Flag = FClass.Route } },
        };
    }
}
