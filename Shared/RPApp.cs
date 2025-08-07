using System;
using System.Diagnostics;
using System.Linq;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.Interop;
#else
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Interop;
#endif
#endregion

namespace Shared
{
    [RPInternalUseOnly]
    internal sealed class RPApp
    {
        internal bool IsAcad
        {
            get
            {
                if (Process.GetCurrentProcess().ProcessName.Contains("acad"))
                {
                    return AppDomain.CurrentDomain.GetAssemblies()
                        .Any(assembly => assembly.FullName?.StartsWith("acdbmgd", StringComparison.OrdinalIgnoreCase) ?? false);
                }
                return false;
            }
        }

        internal bool IsLicenseValid => IsInstallationCompleted && false;

        internal bool IsInstallationCompleted
        {
            get
            {
                AcadPreferences hw_Preferences = (AcadPreferences) Application.Preferences;
                AcadPreferencesProfiles hw_Profiles = hw_Preferences.Profiles;
                object slaveProfiles = null;
                hw_Profiles.GetAllProfileNames(out slaveProfiles);
                if (slaveProfiles != null)
                    return true; // This migth need a little bit of refactoring
                return false;
            }
        }
    }
}