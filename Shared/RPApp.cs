#pragma warning disable CS1998, CS8600, CS8604, CS8618, CS8625

using System; // Keep for .NET 4.6
using System.Linq; // Keep for .NET 4.6
using System.Runtime.InteropServices;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.ApplicationServices;
using System.Threading.Tasks;
#else
using Autodesk.AutoCAD.ApplicationServices;
#endif
#endregion

// Microsoft WIN32 API
using Microsoft.Win32;

using Shared.Controllers;
using Shared.Models;

namespace Shared
{
    sealed class RPApp : IDisposable
    {
        public static bool IsAcad => AppDomain.CurrentDomain.GetAssemblies()
            .Any(assembly => assembly.FullName?.StartsWith("acdbmgd", StringComparison.OrdinalIgnoreCase) ?? false);
        public static bool IsLicensed { get; private set; } = false;

        private Document document = Application.DocumentManager.MdiActiveDocument;

        public static RPConfig Config { get; private set; } = null;
        public static FileWatcherController FileWatcher { get; private set; }
        public static DocumentCollection AsyncCommandContext { get; private set; }

        internal RPApp(DocumentCollection context)
        {
            AsyncCommandContext = context;
            ResourceController.LoadEmbeddedResources();
            Config = ConfigController.LoadConfig<RPConfig>();
            CheckForInstallationRegistry();
            if (Config.InstallPath == null)
                throw new UnauthorizedAccessException("RoadPAC is not installed or installation is malformed!");
            SetDllDirectory(Config.InstallPath);
            FileWatcher = new FileWatcherController(context);
            #region RIBBON_REGISTRY
            RibbonController.CreateTab("rp_RoadPAC");
            RibbonController.CreateContextualTab("rp_Contextual_SelectView", selection => { return true; });
            #endregion
            void BeginInit()
            {
                try
                {
                    var rpfile = new RDPFileHelper();
                    FileWatcher.AddDirectory(rpfile.CurrentWorkingDirectory);
                    IsLicensed = true;
                }
                catch (COMException) // User don't have valid license for RoadPAC
                { }
            }
#if ZWCAD || NET8_0_OR_GREATER
            Task.Run(BeginInit);
#else
            BeginInit(); // AutoCAD does not support multi-threading,
                         // hence we are sacrificing a little bit of performance here,
                         // but at init it does not really matter;
#endif
            document.Editor.WriteMessage("^C");
        }

        #region O_INSTALLATION"HKEY_LOCAL_MACHINE"
        private void CheckForInstallationRegistry()
        {
            if (string.IsNullOrEmpty(Config?.InstallPath))
            {
                string[] registryKeys =
                {
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                    @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
                };
                foreach (string key in registryKeys)
                {
                    using (RegistryKey registryParent = Registry.LocalMachine.OpenSubKey(key))
                    {
                        if (registryParent == null) continue;
                        foreach (string registrySubKey in registryParent.GetSubKeyNames())
                        {
                            using (RegistryKey registrySub = registryParent.OpenSubKey(registrySubKey))
                            {
                                if (registrySub == null) continue;
                                string _publisher = (string)registrySub.GetValue("Publisher") ?? string.Empty;
                                string _displayName = (string)registrySub.GetValue("DisplayName") ?? string.Empty;
                                if ((!string.IsNullOrEmpty(_publisher) && _publisher == "Pragoprojekt a.s") ||   // Publisher   check 
                                    (!string.IsNullOrEmpty(_displayName) && _displayName.StartsWith("RoadPAC"))) // DisplayName check
                                {
                                    // At this point registry key can't be null if already installed
                                    if (Config == null)
                                        Config = ConfigController.LoadConfig<RPConfig>();
                                    Config.InstallPath = (string)registrySub.GetValue("InstallLocation")
                                        ?? throw new ArgumentNullException("RoadPAC install location is null in registry");
                                    Config.DisplayName = (string)registrySub.GetValue("DisplayName")
                                        ?? throw new ArgumentNullException("RoadPAC install location is null in registry");
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        public void Dispose()
        {
            if (Config != null)
                ConfigController.SaveConfig(Config);
            FileWatcher.Dispose();
            FileWatcher = null;
        }

        #region WIN32_API
        public object GetService(Type serviceType) => this;
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetCurrentProcessId();
        #endregion
    }
}