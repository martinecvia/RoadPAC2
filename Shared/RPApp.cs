#pragma warning disable CS8600, CS8625

using System; // Keep for .NET 4.6
using System.Linq; // Keep for .NET 4.6
using System.Runtime.InteropServices;
using Shared.Controllers;


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

namespace Shared
{
    [RPInternalUseOnly]
    [Guid("68AC1E2A-8FB0-8323-98E1-F97CF372FC3D")]
    internal sealed class RPApp : IServiceProvider
    {
        public static bool IsAcad => AppDomain.CurrentDomain.GetAssemblies()
            .Any(assembly => assembly.FullName?.StartsWith("acdbmgd", StringComparison.OrdinalIgnoreCase) ?? false);

        public static RPConfig Config { get; private set; } = null;

        ~RPApp() => ConfigController.SaveConfig(Config);
        internal RPApp()
        {
            // First we need to find install location of RoadPAC
            Config = ConfigController.LoadConfig<RPConfig>();
            #region FRESH_INSTALATION
            if (string.IsNullOrEmpty(Config.InstallPath))
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
            #endregion
            Document document = Application.DocumentManager.MdiActiveDocument;
            if (string.IsNullOrEmpty(Config.InstallPath))
            {
                document.Editor.WriteMessage("Installation of RoadPAC was not found or not valid");
                throw new UnauthorizedAccessException("Installation of RoadPAC was not found or not valid");
            }
            SetDllDirectory(Config.InstallPath);
            document.Editor.WriteMessage("\n" +
                "ANetRoadPAC2: Loaded successfully !\n" +
                $"RpInstallPath: {Config.InstallPath}\n" +
                $"RpDisplayName: {Config.DisplayName}\n" +
                $"RpEnvironment: {(Environment.Is64BitProcess ? "64bit" : "32bit")}");
            ConfigController.SaveConfig(Config);
        }

        public object GetService(Type serviceType) => this;
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);
        
    }
}