using System.Diagnostics;
using System.Threading.Tasks;

namespace Shared.Controllers.Models.Project
{
    internal class CombinedCrossSectionsFile : ProjectController.ProjectFile
    {
        public async override Task BeginInit()
        {
            if (string.IsNullOrEmpty(Path) || string.IsNullOrEmpty(Name))
                return;
            Debug.WriteLine($"Processing Xml: {Path}{Name}");
        }
    }
}