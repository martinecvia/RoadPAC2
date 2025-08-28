using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using System.Windows;
using Shared.Controllers;
using Shared.Windows.Tree;
using static Shared.Controllers.ProjectController;

namespace Shared.Windows
{
    public class ProjectorViewModel
    {
        public List<TreeTabViewModel> Tabs { get; set; }

        public ProjectorViewModel()
        {
            Tabs = new List<TreeTabViewModel>
            {
                new TreeTabViewModel
                {
                    Name = "Trasy",
                    Item = new List<TreeItem>(BuildProjectTree(RPApp.Projector?.CurrentWorkingDirectory))
                }
            }; 
        }

        private List<TreeItem> BuildProjectTree(string lsPath)
        {
            var routes = RPApp.Projector?.GetRoutes(lsPath);
            if (routes == null)
                return new List<TreeItem>();
            Debug.WriteLine("Routes:\n" + string.Join("\n", routes));
            var tree = new List<TreeItem>();
            foreach (var route in routes)
            {
                var routeNode = new TreeItem { Name = Path.GetFileNameWithoutExtension(route.File).ToUpperInvariant() };
                routeNode.Item.Add(new TreeItem { Name = $"Cesta: {route.Path}" });
                routeNode.Item.Add(new TreeItem { Name = $"Směrové řešení: {route.File}" });
                HashSet<ProjectFile> related = RPApp.Projector?.GetRoute(lsPath, route.File);
                Debug.WriteLine("Related:\n" + string.Join("\n", related));
                var refPro = GetByFlag(related, FClass.Profile);
                var relatedPro = refPro.FirstOrDefault(r => r.Flag != FClass.Listing);
                if (relatedPro != null)
                    routeNode.Item.Add(new TreeItem { Name = $"Niveleta: {relatedPro.File}" });
                var refCor = GetByFlag(related, FClass.Corridor);
                var relatedCor = refCor.FirstOrDefault(r => r.Flag != FClass.Listing);
                if (relatedCor != null)
                {
                    var outCro = new TreeItem { Name = $"Koridor" };
                    outCro.Item.Add(new TreeItem { Name = $"Pokrytí: {relatedCor.File}" });
                    var refCro = GetByFlag(related, FClass.Corridor | FClass.CrossSection);
                    var relatedCro = refCor.FirstOrDefault(r => r.Flag != FClass.Listing);
                    if (relatedCro != null)
                        outCro.Item.Add(new TreeItem { Name = $"Příčné řezy: {relatedCro.File}" });
                    routeNode.Item.Add(outCro);
                }
                var refSur = GetByFlag(related, FClass.Survey);
                var relatedSur = refCor.FirstOrDefault(r => r.Flag != FClass.Listing);
                if (relatedSur != null)
                {

                }
                var refIFC = GetByFlag(related, FClass.IFC);
                var refCom = GetByFlag(related, FClass.CombinedCrossSections);
                tree.Add(routeNode);
            }
            return tree;
        }

        private IEnumerable<ProjectFile> GetByFlag(HashSet<ProjectFile> related, FClass flag) 
            => related.Where(f => f.Flag == flag);
    }
}
