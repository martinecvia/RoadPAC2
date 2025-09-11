#pragma warning disable CS8603, CS8618, CS8625

using System.Threading.Tasks; // Keep for .NET 4.6

using RDPFILELib;

namespace Shared.Helpers
{
    /// <summary>
    /// Safe wrapper for <see cref="RDPConfig"/> and related RDPFILEib.dll components.
    /// Protects against missing DLLs or unexpected structural changes, so that
    /// the hosting CAD platform does not crash if the dependency is unavailable.
    /// </summary>
    public sealed class RDPFileHelper
    {
        private readonly RDPConfig _config;
        private readonly object _lock = new object();

        internal RDPFileHelper() 
        {
            try
            { _config = new RDPConfigClass(); }
            catch
            { _config = null; }
        }

        /// <summary>
        /// Gets or sets the current working directory from RDP configuration.
        /// </summary>
        public string CurrentWorkingDirectory
        {
            get
            {
                lock (_lock)
                {
                    return _config?.AdresarProjektu;
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                lock (_lock)
                {
                    if (_config != null)
                        _config.AdresarProjektu = value;
                }
            }
        }

        /// <summary>
        /// Async version of <see cref="CurrentWorkingDirectory"/>.
        /// </summary>
        public Task<string> GetCurrentWorkingDirectory()
        {
            return Task.FromResult(CurrentWorkingDirectory);
        }

        /// <summary>
        /// Gets or sets the current route from RDP configuration.
        /// </summary>
        public string CurrentRoute
        {
            get
            {
                lock (_lock)
                {
                    return _config?.AktivniTrasa;
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                lock (_lock)
                {
                    if (_config != null)
                        _config.AktivniTrasa = value;
                }
            }
        }

        /// <summary>
        /// Async version of <see cref="CurrentRoute"/>.
        /// </summary>
        public Task<string> GetCurrentRoute()
        {
            return Task.FromResult(CurrentRoute);
        }

        /// <summary>
        /// Creates an RDP configuration file in the given working directory for the given route.
        /// Returns true on success, false if parameters are invalid or the call fails.
        /// </summary>
        public bool CreateConfigRDP(string _workingDirectory, string _route)
        {
            if (string.IsNullOrEmpty(_workingDirectory) || string.IsNullOrEmpty(_route))
                return false;
            try
            {
                if (!_workingDirectory.EndsWith("\\"))
                    _workingDirectory += "\\";
                new RDPInfoClass().RoadPacUtilita("MAKECFG", "", _workingDirectory, _route);
                return true;
            }
            catch { return false; }
        }
    }
}