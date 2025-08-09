using System; // Keep for .NET 4.6
using System.Diagnostics;
using System.Linq; // Keep for .NET 4.6

namespace Shared
{
    [RPInternalUseOnly]
    internal sealed class RPApp
    {
        /// <summary>
        /// Determines whether the current process is running within an AutoCAD environment.
        /// </summary>
        /// <remarks>
        /// This check first verifies if the current process name contains the substring
        /// <c>"acad"</c>. If so, it further inspects the loaded assemblies to see if any
        /// assembly name begins with <c>"acdbmgd"</c>, which is a core AutoCAD .NET API
        /// assembly.
        /// <para>
        /// Returns <see langword="false"/> if running in ZWCAD or any non-AutoCAD host
        /// application.
        /// </para>
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if running in AutoCAD; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool IsAcad
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
    }
}