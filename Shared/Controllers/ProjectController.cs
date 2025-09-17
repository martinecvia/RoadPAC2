#pragma warning disable CS1998

using System; // Keep for .NET 4.6
using System.Collections.Concurrent;
using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.IO;
using System.Linq; // Keep for .NET 4.6
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading; // Keep for .NET 4.6
using System.Threading.Tasks;
using System.Xml.Serialization;

using Shared.Controllers.Models.Project;
using Shared.Helpers;

namespace Shared.Controllers
{
    // https://adndevblog.typepad.com/autocad/2012/06/use-thread-for-background-processing.html
    public class ProjectController : IDisposable, INotifyPropertyChanged
    {
        private const int INTERVAL_CHECK_TIME = 1; // In seconds
        private const int RPUPDATE_CHECK_TIME = 1; // In seconds

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private readonly ConcurrentDictionary<string, HashSet<string>> _changes
            = new ConcurrentDictionary<string, HashSet<string>>();

        // Project related
        private readonly ConcurrentDictionary<string, HashSet<ProjectFile>> _project
            = new ConcurrentDictionary<string, HashSet<ProjectFile>>();
        private volatile string _currentWorkingDirectory;
        private volatile string _currentRoute;
        private volatile ProjectFile _currentProjectFile;

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // From, To
        public event Action<string, string> CurrentWorkingDirectoryChanged;
        public event Action<string, string> CurrentRouteChanged;
        public event Action<string> ProjectChanged;

        public event Action<ProjectFile> CurrentProjectFileChanged;

        [RPInfoOut]
        public string CurrentWorkingDirectory
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    return _currentWorkingDirectory;
                }
                finally
                { _lock.ExitReadLock(); }
            }
            set
            {
                if (value == null) return;
                if (!value.EndsWith("\\"))
                    value += "\\";
                if (_currentWorkingDirectory == value) return;
                if (!Directory.Exists(value)) return;
                string _tmp = _currentWorkingDirectory;
                _lock.EnterWriteLock();
                try
                {
                    _currentWorkingDirectory = value;
                }
                finally
                { _lock.ExitWriteLock(); }
                CurrentWorkingDirectoryChanged?.Invoke(_tmp, value);
                NotifyPropertyChanged(nameof(CurrentWorkingDirectory));
            }
        }

        [RPInfoOut]
        public string CurrentRoute
        {
            get {
                _lock.EnterReadLock();
                try
                {
                    return _currentRoute;
                }
                finally
                { _lock.ExitReadLock(); }
            }
            set
            {
                if (value == null) return;
                if (_currentRoute == value) return;
                string _tmp = _currentRoute;
                _lock.EnterWriteLock();
                try
                {
                    _currentRoute = value;
                }
                finally
                { _lock.ExitWriteLock(); }
                CurrentRouteChanged?.Invoke(_tmp, value);
                NotifyPropertyChanged(nameof(CurrentRoute));
            }
        }

        [RPInfoOut]
        public ProjectFile CurrentProjectFile
        {
            get => _currentProjectFile;
            set
            {
                if (_currentProjectFile != value)
                {
                    _currentProjectFile = value;
                    CurrentProjectFileChanged?.Invoke(value);
                    NotifyPropertyChanged(nameof(CurrentProjectFile));
                }
            }
        }

        [RPInfoOut]
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
            return new HashSet<ProjectFile>(files.Where(f => f != null && !string.IsNullOrEmpty(f.File))
                .GroupBy(f => Path.GetFileNameWithoutExtension(f.File).ToUpper())
                .Select(g => g.FirstOrDefault(f => f.File.EndsWith(".shb", StringComparison.OrdinalIgnoreCase))
                          ?? g.FirstOrDefault(f => f.File.EndsWith(".xhb", StringComparison.OrdinalIgnoreCase)))
                .Where(f => f != null));
        }

        [RPInfoOut]
        public HashSet<ProjectFile> GetRoute(string lsPath, string routeName)
        {
            if (string.IsNullOrEmpty(lsPath) || string.IsNullOrEmpty(routeName))
                return new HashSet<ProjectFile>();
            if (!ProjectFiles.TryGetValue(lsPath, out var files))
                return new HashSet<ProjectFile>();
            return new HashSet<ProjectFile>(files.Where(f => f != null && !string.IsNullOrEmpty(f.File))
                .Where(f => f.Root != null && f.Root == Path.GetFileNameWithoutExtension(routeName).ToUpper()));
        }

        public class RPConfig
        {
            [RPInfoOut]
            public string LastRoute { get; set; }
        }

        [RPInternalUseOnly]
        public class ProjectFile : INotifyPropertyChanged
        {
            public string File { get; internal set; }
            public string Path { get; internal set; }

            public string Root { get; internal set; }
            public FClass Flag { get; internal set; } = FClass.None;
            public DateTime CreatedAt { get; internal set; } = DateTime.UtcNow;

            // UpdatedAt
            private DateTime _updatedAt;
            public DateTime UpdatedAt
            {
                get => _updatedAt;
                internal set
                {
                    if (_updatedAt != value)
                    {
                        _updatedAt = value;
                        NotifyPropertyChanged(nameof(UpdatedAt));
                    }
                }
            }
            // Internal marker
            [XmlIgnore]
            internal bool IsRoot { get; set; }
            /// <summary>
            /// Base initialization. Does nothing by default.
            /// Derived classes can override to provide async initialization logic.
            /// </summary>
            public virtual Task BeginInit() => Task.CompletedTask;
            // INotifyPropertyChanged
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            public override string ToString()
                => $"{nameof(ProjectFile)}(File={File}, Path={Path}, Root={Root}::IsRoot={IsRoot}, Flag={Flag})";
        }

        [Flags]
        [RPInternalUseOnly]
        public enum FClass : int
        {
            None = 0,
            // Mappings
            Route = 1 << 0,  // Trasa / Směrové řešení
            Profile = 1 << 1,  // Niveleta
            Corridor = 1 << 2,  // Koridor
            CrossSection = 1 << 3,  // Příčné řezy
            Survey = 1 << 4,  // Vytyčení
            IFC = 1 << 5,  // IFC Podklady
            CombinedCrossSections = 1 << 6,  // Spojené příčné řezy
            // Types
            Listing = 1 << 7,
            Xml = 1 << 8,
        }

        private System.Threading.Timer _roadPacTimer;
        private System.Threading.Timer _changesTimer;


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
                CurrentWorkingDirectoryChanged += (_, t) =>
                {
                    RPApp.FileWatcher?.AddDirectory(t);
                    RefreshProject(RPApp.FileWatcher?.Files);
                };
            }
            // This should be reworked to be handled asynchrounously
            _roadPacTimer = new System.Threading.Timer(_ => _ = ProcessRoadPacInBackground(), null, Timeout.Infinite, Timeout.Infinite);
            _changesTimer = new System.Threading.Timer(_ => _ = ProcessChangesInBackground(), null, Timeout.Infinite, Timeout.Infinite);
        }

        [RPInternalUseOnly]
        internal void BeginInit()
        {
            _roadPacTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(RPUPDATE_CHECK_TIME));
            _changesTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(INTERVAL_CHECK_TIME));
        }

        [RPInternalUseOnly]
        internal void RefreshProject(IReadOnlyDictionary<string, HashSet<string>> files)
        {
            if (RPApp.FileWatcher == null || files == null) return;
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
        #region PRIVATE
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
        private volatile bool _generalOperationActive;
        private volatile bool _roadPacOperationActive;

        [RPPrivateUseOnly]
        private async Task ProcessRoadPacInBackground()
        {
            if (_roadPacOperationActive)
                return;
            _roadPacOperationActive = true;
            try
            {
                if (RPApp.RDPHelper == null)
                    RPApp.RDPHelper = new RDPFileHelper();
                CurrentWorkingDirectory = await RPApp.RDPHelper.GetCurrentWorkingDirectory();
                CurrentRoute            = await RPApp.RDPHelper.GetCurrentRoute();
            }
            catch (COMException) { }
            finally
            {
                _roadPacOperationActive = false; // We want to halt this task from updating,
                                                 // since user don't have valid RoadPAC license
            }
        }

        [RPPrivateUseOnly]
        private async Task ProcessChangesInBackground()
        {
            if (_changes.IsEmpty || _generalOperationActive)
                return;
            List<string> projects = new List<string>();
            _generalOperationActive = true;
            try
            {
                var snapshot = _changes.ToDictionary(
                    kvp => kvp.Key,
                    kvp => { lock (kvp.Value) return kvp.Value; });
                _changes.Clear();
                foreach (var kvp in snapshot)
                    if (!projects.Contains(kvp.Key))
                        projects.Add(kvp.Key);
                var InitTasks = snapshot.SelectMany(pair
                    => pair.Value
                    // We want to load Route files first
                    .OrderBy(f => f.EndsWith(".xhb", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".shb", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                    .Select(fileName => ProcessFile(pair.Key, fileName)));
                await Task.WhenAll(InitTasks);
            }
            finally
            {
                foreach (var lsPath in projects)
                {
                    ProjectChanged?.Invoke(lsPath);
                    NotifyPropertyChanged(nameof(ProjectFiles));
                }   
                _generalOperationActive = false;
            }
        }

        [RPPrivateUseOnly]
        private async Task ProcessFile(string lsPath, string fileName)
        {
            var fullPath = Path.Combine(lsPath, fileName);
            _lock.EnterWriteLock();
            try
            {
                if (!_project.TryGetValue(lsPath, out HashSet<ProjectFile> files))
                    files = _project[lsPath] = new HashSet<ProjectFile>();
                var projectFile = files.FirstOrDefault(f => f.File.Equals(fileName, StringComparison.OrdinalIgnoreCase));
                if (File.Exists(fullPath))
                {
                    FileInfo lsFile = new FileInfo(fullPath);
                    if (!lsFile.IsReadOnly)
                    {
                        // We fetch updated date early, as change can happen anytime dureing fetch process
                        DateTime updatedAt = File.GetLastWriteTimeUtc(fullPath);
                        string extension = Path.GetExtension(fileName)?.TrimStart('.').ToUpper() ?? "";
                        if (projectFile != null)
                        {
                            // Just update it's values
                            projectFile.UpdatedAt = updatedAt;
                            await projectFile.BeginInit();
                        }
                        else if (ProjectFileFactory.TryGetValue(extension, out var factory))
                        {
                            DateTime createdAt = File.GetCreationTimeUtc(fullPath);
                            projectFile = factory();
                            projectFile.Path = lsPath;
                            projectFile.Root = Path.GetFileNameWithoutExtension(fileName).ToUpperInvariant();
                            projectFile.File = fileName;
                            if (createdAt > updatedAt)
                            {
                                // Attempt to fix datetimes when file was copied from another source,
                                // and lost track of their creation date & time
                                try
                                {
                                    File.SetCreationTimeUtc(fullPath, updatedAt);   // Set the creation time
                                    File.SetLastWriteTimeUtc(fullPath, updatedAt);  // Restore the original last write time
                                }
                                catch (Exception)
                                { return; }
                                createdAt = updatedAt; // Weird thing when copying files from network,
                                                       // then their creation date is newer than update date
                            }
                            projectFile.CreatedAt = createdAt;
                            projectFile.UpdatedAt = updatedAt;
                            await projectFile.BeginInit();
                            files.Add(projectFile);
                        }
                        else
                        {
                            // File does not have Factory
                            // Presumably unmapped file, ignoring it
                        }
                    }
                } 
                else
                {
                    // File have been removed
                    if (projectFile != null)
                    {
                        projectFile.UpdatedAt = DateTime.UtcNow;
                        files.Remove(projectFile);
                    }
                }
            }
            catch (Exception) { }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
        #endregion
        public void Dispose()
        {
            _roadPacTimer?.Dispose();
            _changesTimer?.Dispose();
            _roadPacTimer = null;
            _changesTimer = null;
            _project.Clear();
            _changes.Clear();
            _lock?.Dispose();
        }

        [RPPrivateUseOnly]
        private static readonly Dictionary<string, Func<ProjectFile>> ProjectFileFactory = new Dictionary<string, Func<ProjectFile>>()
        {
            // Koridor
            { "V43", () => new ProjectFile() { Flag = FClass.Corridor } },
            { "L43", () => new ProjectFile() { Flag = FClass.Corridor | FClass.Listing } },
            { "L43A", () => new ProjectFile() { Flag = FClass.Corridor | FClass.Listing } },
            // Vytyčení
            { "V47", () => new SurveyFile() { Flag = FClass.Survey | FClass.Xml } },
            { "V47X", () => new SurveyFile() { Flag = FClass.Survey | FClass.Xml } },
            // Příčné řezy
            { "V51", () => new ProjectFile() { Flag = FClass.Corridor | FClass.CrossSection } },
            { "L51", () => new ProjectFile() { Flag = FClass.Corridor | FClass.CrossSection | FClass.Listing } },
            { "L51A", () => new ProjectFile() { Flag = FClass.Corridor | FClass.CrossSection | FClass.Listing } },
            // Spojené příčné řezy
            { "V91", () => new CombinedCrossSectionsFile() { Flag = FClass.CombinedCrossSections | FClass.Xml } },
            // IFC Podklady
            { "V94", () => new IFCFile() { Flag = FClass.IFC | FClass.Xml } },
            // Niveleta
            { "XNI", () => new ProjectFile() { Flag = FClass.Profile } },
            { "SNI", () => new ProjectFile() { Flag = FClass.Profile } },
            // Trasa / Směrové řešení
            { "SHB", () => new ProjectFile() { Flag = FClass.Route } },
            { "XHB", () => new ProjectFile() { Flag = FClass.Route } },
            { "L13", () => new ProjectFile() { Flag = FClass.Route| FClass.Listing } },
        };
    }
}