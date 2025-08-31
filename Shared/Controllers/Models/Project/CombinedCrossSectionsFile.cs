using System.IO;
using System.Threading.Tasks; // Keep for .NET 4.6
using System.Xml;

using Shared.Helpers;

namespace Shared.Controllers.Models.Project
{
    public class CombinedCrossSectionsFile : BaseProjectXml
    {
        public async override Task BeginInit()
        {
            if (string.IsNullOrEmpty(Path) || string.IsNullOrEmpty(File))
                return;
            string[] lines = await FilePeekHelper.PeekFileAsync(System.IO.Path.Combine(Path, File), 1);
            if (lines.Length > 0)
            {
                string xmlLine = lines[0].Trim();
                if (!xmlLine.EndsWith("/>") && !xmlLine.EndsWith(">"))
                    xmlLine += " />";
                using (var reader = XmlReader.Create(new StringReader(xmlLine)))
                {
                    if (reader.ReadToFollowing("x910"))
                    {
                        // For some reason this is lowercased
                        Route = reader?.GetAttribute("HlavniTrasa")?.ToUpper();
                        TerrainModelFile = reader.GetAttribute("DtmFile");
                    }
                }
            }
        }
        public override string ToString()
            => $"{nameof(CombinedCrossSectionsFile)}(File={File}, Path={Path}, Flag={Flag}, Route={Route}, TerrainModelFile={TerrainModelFile})";
    }
}