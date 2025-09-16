#pragma warning disable CS8625

namespace Shared.Models
{
    public class RPConfig
    {
        [RPInfoOut]
        public string InstallPath { get; set; } = null;
        [RPInfoOut]
        public string DisplayName { get; set; } = null;
        [RPInfoOut]
        public string LastRoute { get; set; } = null;
    }
}