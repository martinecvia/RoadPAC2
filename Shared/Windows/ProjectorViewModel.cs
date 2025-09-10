#pragma warning disable CS8625

using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq; // Keep for .NET 4.6
using System.Windows.Data;

using Shared.Controllers;
using Shared.Windows.Models;

namespace Shared.Windows
{
    public class ProjectorViewModel
    {
        public ObservableCollection<TreeItem> Items => new ObservableCollection<TreeItem>(BuildProjectTree(RPApp.Projector?.CurrentWorkingDirectory));
        public ICollectionView FilteredItems { get; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    RefreshFilter();
                }
            }
        }

        private void RefreshFilter() => FilteredItems.Refresh();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ProjectorViewModel()
        {
            FilteredItems = CollectionViewSource.GetDefaultView(Items);
            FilteredItems.Filter = RootFilter;
        }

        private bool RootFilter(object obj)
        {
            if (obj is TreeItem item)
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                    return true;
                return item.Label?.IndexOf(SearchText, System.StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return false;
        }

        [RPPrivateUseOnly]
        private IEnumerable<TreeItem> BuildProjectTree(string lsPath)
        {
            var routes = RPApp.Projector?.GetRoutes(lsPath);
            if (routes == null)
                return new ObservableCollection<TreeItem>();
            var tree = new ObservableCollection<TreeItem>();
            foreach (var route in routes)
            {
                // If for whatever reason our route will be null
                if (route == null) continue;
                var currentRoute = RPApp.Projector?.CurrentRoute?.ToUpperInvariant();
                var routeName = Path.GetFileNameWithoutExtension(route.File).ToUpperInvariant();
                var isCurrent = routeName == currentRoute;

                var routeNode = new TreeItem
                {
                    Label = routeName,
                    IsRouteNode = true,
                    IsActiveRoute = isCurrent,
                    Value = isCurrent ? "(CurrentRoute)" : string.Empty,
                    ValueColor = isCurrent ? "Green" : "White",
                    Image = "./Assets/route.png",
                    File = route
                };
                routeNode.Add(new TreeItem { Label = $"Směrové řešení:", Value= route.File, File = route, Image = "./Assets/shb.ico" });
                var related = RPApp.Projector?.GetRoute(lsPath, route.File) ?? new HashSet<ProjectController.ProjectFile>();
                // Not a great implementaton but it works at least a little
                void MarkOutdated(TreeItem node, ProjectController.ProjectFile parent)
                {
                    // Falling shit system,
                    // from the oldest to the newest record, and if anything is older, then it's outdated
                    if (node.File?.UpdatedAt != null && parent != null && !string.IsNullOrEmpty(parent.File))
                        node.DisplayWarning = node.File.UpdatedAt < parent.UpdatedAt - TimeSpan.FromSeconds(10);
                    else if ((parent != null || parent?.UpdatedAt != null) && node.File == null)
                        node.DisplayWarning = true; // If file that should be there before is not there

                    // We don't want to display warning for Route or empty filed nodes
                    if (node.File == null || node.IsRouteNode || node.IsGroupNode)
                        node.DisplayWarning = false;
                    if (node.DisplayWarning)
                        node.WarningToolTip = $"Soubor je zastaralý, vůči: {parent.File}";
                    foreach (var child in node)
                    {
                        MarkOutdated(child, parent);
                        if (child.DisplayWarning)
                            child.WarningToolTip = $"Soubor je zastaralý, vůči: {parent.File}";
                        parent = child.File ?? parent;
                    }
                }

                var profile = related.FirstOrDefault(r => r.Flag.HasFlag(ProjectController.FClass.Profile) && !r.Flag.HasFlag(ProjectController.FClass.Listing));
                routeNode.Add(new TreeItem
                {
                    Label = profile != null
                        ? $"Niveleta:"
                        : "Niveleta",
                    // Default put in place just so we can know from ribbon that we have selected this node
                    File = profile ?? new ProjectController.ProjectFile() { Flag = ProjectController.FClass.Profile },
                    Value = profile?.File
                });

                AddCorridor(routeNode, related);
                AddGroup(routeNode, related, ProjectController.FClass.Survey, "Vytyčení", "./Assets/survey.png", "./Assets/list-item.png");
                AddGroup(routeNode, related, ProjectController.FClass.IFC, "Podklady pro IFC", "./Assets/ifc.png", "./Assets/list-item.png");
                AddGroup(routeNode, related, ProjectController.FClass.CombinedCrossSections, "Kreslení příčných řezů", "./Assets/shb.ico", "./Assets/list-item.png");
                MarkOutdated(routeNode, route);
                tree.Add(routeNode);
            }
            return tree;
        }

        [RPPrivateUseOnly]
        private void AddGroup(TreeItem parent, IEnumerable<ProjectController.ProjectFile> related, ProjectController.FClass flag, string label,
            string groupNodeImage = null, string itemNodeImage = null)
        {
            var groupNode = new TreeItem { Label = label, IsGroupNode = true, Image = groupNodeImage, 
                File = new ProjectController.ProjectFile() { Flag = flag } };
            var matches = related.Where(r => r.Flag.HasFlag(flag)).ToList();
            if (matches.Any())
            {
                foreach (var file in matches)
                    if (!file.Flag.HasFlag(ProjectController.FClass.Listing))
                        groupNode.Add(new TreeItem { Value = file.File, Image = itemNodeImage, File = file });
            }
            parent.Add(groupNode);
        }

        [RPPrivateUseOnly]
        private void AddCorridor(TreeItem parent, IEnumerable<ProjectController.ProjectFile> related)
        {
            var corridor = related.FirstOrDefault(r => r.Flag == ProjectController.FClass.Corridor);
            var node = new TreeItem { Label = "Koridor", IsGroupNode = true, Image = "./Assets/road.png", 
                File = new ProjectController.ProjectFile() { Flag = ProjectController.FClass.Corridor } };
            if (corridor != null)
            {
                node.Add(new TreeItem { Label = $"Pokrytí:", Value = corridor.File, Image = "./Assets/list-item.png", File = corridor });
                var crossSection = related.FirstOrDefault(r => r.Flag == (ProjectController.FClass.Corridor | ProjectController.FClass.CrossSection));
                node.Add(new TreeItem
                {
                    Label = crossSection != null 
                        ? $"Příčné řezy:"
                        : "Příčné řezy",
                    File = crossSection ?? new ProjectController.ProjectFile() // Default put in place just so we can know from ribbon that we have selected this node
                    { Flag = ProjectController.FClass.Corridor | ProjectController.FClass.CrossSection },
                    Value = crossSection?.File,
                    Image = "./Assets/v91.ico"
                });
            }
            parent.Add(node);
        }
    }
}