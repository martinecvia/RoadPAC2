using System.IO;
using System.Threading.Tasks;
using System.Xml;

using Shared.Helpers;

namespace Shared.Controllers.Models.Project
{
    public class SurveyFile : BaseProjectXml
    {
        public async override Task BeginInit()
        {
            if (string.IsNullOrEmpty(Path) || string.IsNullOrEmpty(File))
                return;
            string[] lines = await FilePeekHelper.PeekFileAsync(System.IO.Path.Combine(Path, File), 2);
            if (lines.Length > 1)
            {
                string xmlLine = lines[1].Trim();
                if (!xmlLine.EndsWith("/>") && !xmlLine.EndsWith(">"))
                    xmlLine += " />";
                using (var reader = XmlReader.Create(new StringReader(xmlLine)))
                {
                    if (reader.ReadToFollowing("Nastaveni"))
                    {
                        // For some reason this is lowercased
                        Route = reader.GetAttribute("trasaName")?.ToUpper();
                        if (Route != null)
                            Root = Route;
                        TerrainModelFile = reader.GetAttribute("DTMFileName");
                    }
                }
            }
        }
        public override string ToString()
            => $"{nameof(SurveyFile)}(File={File}, Path={Path}, Root={Root}, Flag={Flag}, Route={Route}, TerrainModelFile={TerrainModelFile})";
    }
}