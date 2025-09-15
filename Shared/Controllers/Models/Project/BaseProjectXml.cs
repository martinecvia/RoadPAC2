#pragma warning disable CS8618
namespace Shared.Controllers.Models.Project
{
    [RPInternalUseOnly]
    public abstract class BaseProjectXml : ProjectController.ProjectFile
    {
        public string Route { get; protected set; }
        public string TerrainModelFile { get; protected set; }
    }
}