#pragma warning disable CS1998

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Shared.Controllers.Models.Project;

namespace Shared.Controllers
{
    // https://adndevblog.typepad.com/autocad/2012/06/use-thread-for-background-processing.html
    internal class ProjectController : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly ConcurrentDictionary<string, HashSet<string>> _changes
            = new ConcurrentDictionary<string, HashSet<string>>();

        private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        // Project related
        private readonly ConcurrentDictionary<string, HashSet<ProjectFile>> _project 
            = new ConcurrentDictionary<string, HashSet<ProjectFile>>();
        public event Action<string, ProjectFile> ProjectChanged;

        public IReadOnlyDictionary<string, HashSet<ProjectFile>> Projects
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

        public void RefreshProject(IReadOnlyDictionary<string, HashSet<string>> files)
        {
            foreach (KeyValuePair<string, HashSet<string>> pair in RPApp.FileWatcher.Files)
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

        [RPInternalUseOnly]
        internal class ProjectFile
        {
            public string Name { get; internal set; }
            public string Path { get; internal set; }
            public FClass Flag { get; internal set; } = FClass.None;
            /// <summary>
            /// Base initialization. Does nothing by default.
            /// Derived classes can override to provide async initialization logic.
            /// </summary>
            public virtual Task BeginInit() => Task.CompletedTask;
            public override string ToString() 
                => $"{nameof(ProjectFile)}(Name={Name}, Path={Path}, Flag={Flag})";
        }

        [Flags]
        [RPInternalUseOnly]
        internal enum FClass : int
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

        // Just one processor at a time
        private volatile bool _isProcessing;

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
                    foreach (KeyValuePair<string, HashSet<string>> pair in snapshot)
                    {
                        foreach (string fileName in pair.Value)
                        {
                            string extension = Path.GetExtension(fileName)?.TrimStart('.').ToUpper() ?? "";
                            var recognizedFile = ProjectFileFactory.TryGetValue(extension, out var factory) ? factory() : null;
                            if (recognizedFile == null)
                            {
                                //Debug.WriteLine($"{fileName} was not mapped");
                                continue;
                            };
                            _lock.EnterWriteLock();
                            try
                            {
                                if (_project.TryGetValue(pair.Key, out HashSet<ProjectFile> files))
                                    files.Add(recognizedFile);
                            }
                            finally { _lock.ExitWriteLock(); }
                            recognizedFile.Path = pair.Key;
                            recognizedFile.Name = fileName;
                            // If it had some logic -> xml reading, instantiating etc.
                            await recognizedFile.BeginInit();
#if !ZWCAD && !NET8_0_OR_GREATER
                            ProjectChanged?.Invoke(pair.Key, recognizedFile);
#else
                            ThreadPool.QueueUserWorkItem(_ => ProjectChanged?.Invoke(pair.Key, recognizedFile));
#endif
                        }
                    }
                }
                finally
                {
                    _isProcessing = false;
                }
            }
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
            { "V51", () => new ProjectFile() { Flag = FClass.CrossSection } },
            { "L51", () => new ProjectFile() { Flag = FClass.CrossSection | FClass.Listing } },
            { "L51A", () => new ProjectFile() { Flag = FClass.CrossSection | FClass.Listing } },
            // Spojené příčné řezy
            { "V91", () => new CombinedCrossSectionsFile() { Flag = FClass.CombinedCrossSections | FClass.Xml | FClass.Multi } },
            // IFC Podklady
            { "V94", () => new IFCFile() { Flag = FClass.IFC | FClass.Xml | FClass.Multi } },
            // Niveleta
            { "XNI", () => new ProjectFile() { Flag = FClass.Profile } },
            // Trasa / Směrové řešení
            { "XHB", () => new ProjectFile() { Flag = FClass.Route } },
        };

        private void ProcessFileChanged(string lsPath, string fileName)
        {
            _changes.AddOrUpdate(
                lsPath,
                _ => new HashSet<string> { fileName },
                (_, set) => { lock (set) set.Add(fileName); return set; });
        }

        private void ProcessFileRenamed(string lsPath, string fileName, string oldName)
        {
            _changes.AddOrUpdate(
                lsPath,
                _ => new HashSet<string> { fileName },
                (_, set) => { lock (set) set.Add(fileName); return set; });
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
