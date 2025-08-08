using System;
using System.Diagnostics;
using System.Linq;

namespace Shared
{
    [RPInternalUseOnly]
    internal sealed class RPApp
    {
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