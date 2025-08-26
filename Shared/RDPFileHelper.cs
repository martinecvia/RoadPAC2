using System;
using RDPFILELib;

namespace Shared
{
    // Purpose of this class is to keep thread safe if RDPFile.dll was not found,
    // so we can safely exit program without cad platform crashing
    // Added factor is that DLL can change it's structure so it will be harder for us to
    public class RDPFileHelper
    {
        private readonly RDPConfig _config;
        internal RDPFileHelper()
        {
            _config = new RDPConfigClass();
        }

        public string CurrentWorkingDirectory
        { 
            get => _config.AdresarProjektu;
            set
            {
                if (value != null)
                    _config.AdresarProjektu = value;
            }
        }

        public string CurrentRoute
        {
            get => _config.AktivniTrasa;
            set
            {
                if (value != null)
                    _config.AktivniTrasa = value;
            }
        }
    }
}
