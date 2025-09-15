using System.IO;
using System.Threading.Tasks;
using System.Xml;

using Shared.Helpers;

namespace Shared.Controllers.Models.Project
{
    public class IFCFile : BaseProjectXml
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
                    if (reader.ReadToFollowing("x940"))
                    {
                        // For some reason this is lowercased
                        Route = reader?.GetAttribute("HlavniTrasa")?.ToUpper();
                        if (Route != null)
                            Root = Route;
                        string _terrain = reader.GetAttribute("DtmFile");
                        TerrainModelFile = string.IsNullOrEmpty(_terrain) ? null : _terrain;
                    }
                }
            }
        }
        public override string ToString()
            => $"{nameof(IFCFile)}(File={File}, Path={Path}, Root={Root}, Flag={Flag}, Route={Route}, TerrainModelFile={TerrainModelFile})";
    }
}