using System;
using System.Threading.Tasks;


namespace Shared
{
    // Purpose of this class is to keep thread safe if RDPFile.dll was not found,
    // so we can safely exit program without cad platform crashing
    // Added factor is that DLL can change it's structure so it will be harder for us to
    public class RDPFileHelper
    {
        private readonly RDPFILELib.RDPConfig _config = new RDPFILELib.RDPConfigClass();

        public string CurrentWorkingDirectory
        { 
            get => _config?.AdresarProjektu;
            set
            {
                if (value != null && _config != null)
                    _config.AdresarProjektu = value;
            }
        }

        public async Task<string> GetCurrentWorkingDirectory()
        {
            if (_config == null)
                return null;
            return await Task.Run(() => CurrentWorkingDirectory);
        }

        public string CurrentRoute
        {
            get => _config?.AktivniTrasa;
            set
            {
                if (value != null && _config != null)
                    _config.AktivniTrasa = value;
            }
        }

        public async Task<string> GetCurrentRoute()
        {
            if (_config == null)
                return null;
            return await Task.Run(() => CurrentRoute);
        }
    }
}
