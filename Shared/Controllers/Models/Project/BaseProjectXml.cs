#pragma warning disable CS8618
namespace Shared.Controllers.Models.Project
{
    public abstract class BaseProjectXml : Shared.Controllers.ProjectController.ProjectFile
    {
        public string Route { get; protected set; }
        public string TerrainModelFile { get; protected set; }
        public override string ToString()
            => $"{base.ToString()}(File={File}, Path={Path}, Flag={Flag}, Route={Route}, TerrainModelFile={TerrainModelFile})";
    }
}
