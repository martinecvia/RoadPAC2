#pragma warning disable CS8600, CS8625

using System; // Keep for .NET 4.6
using System.Linq; // Keep for .NET 4.6
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.Windows;
#else
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Internal.Windows;
using Autodesk.Windows;
#endif
#endregion

using Microsoft.Win32;
using Shared.Controllers;

namespace Shared
{
    sealed class RPApp : IDisposable
    {
        public static bool IsAcad => AppDomain.CurrentDomain.GetAssemblies()
            .Any(assembly => assembly.FullName?.StartsWith("acdbmgd", StringComparison.OrdinalIgnoreCase) ?? false);

        public static bool IsRdpPresent => AppDomain.CurrentDomain.GetAssemblies()
            .Any(assembly => assembly.FullName?.StartsWith("RDPFILELib", StringComparison.OrdinalIgnoreCase) ?? false);

        public static  RPConfig Config { get; private set; } = null;

        internal RPApp()
        {
            ResourceController.LoadEmbeddedResources();
            Config = ConfigController.LoadConfig<RPConfig>();
            CheckForInstallationRegistry();
            #region RIBBON_REGISTRY
            RibbonController.CreateTab("rp_RoadPAC");
            RibbonController.CreateContextualTab("rp_Contextual_SelectView", selection => { return true; });
            #endregion
            Document document = Application.DocumentManager.MdiActiveDocument;
            #region O_PROGRAM_HANDLE_ROADPAC \n#define RP_INSTALLED
            // Hacky way hot to trick C# compiler to make sure RP_INSTALLED definition is on
            SetDllDirectory(Config.InstallPath);
            // Check Assemblies if RDPFILELib is present
            if (IsRdpPresent)
                return;
            Assembly.LoadFrom(Path.Combine(Config.InstallPath, "RDPFILELib.dll"));
            #endregion
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