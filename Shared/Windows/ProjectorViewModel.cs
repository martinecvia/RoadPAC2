using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using System.Windows;
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
                var routeNode = new TreeItem
                {
                    Label = Path.GetFileNameWithoutExtension(route.File).ToUpperInvariant(),
                    IsRouteNode = true,
                    Image = "./Assets/route.png",
                    File = route
                };
                routeNode.Add(new TreeItem { Label = $"Směrové řešení:", Value= route.File, File = route });
                var related = RPApp.Projector?.GetRoute(lsPath, route.File) ?? new HashSet<ProjectController.ProjectFile>();
                // Not a great implementaton but it works at least a little
                void MarkOutdated(TreeItem node, ProjectController.ProjectFile parent)
                {
                    // Falling shit system,
                    // from the oldest to the newest record, and if anything is older, then it's outdated
                    if (node.File?.UpdatedAt != null && parent != null)
                        node.DisplayWarning = node.File.UpdatedAt < parent.UpdatedAt - TimeSpan.FromSeconds(10);
                    else if (parent != null && node.File == null)
                        node.DisplayWarning = true; // If file that should be there before is not there

                    // We don't want to display Route or empty filed nodes
                    if (node.File == null || node.IsRouteNode)
                        node.DisplayWarning = false;
                    if (node.DisplayWarning == true)
                        node.WarningToolTip = $"Soubor je zastaralý, vůči: {parent.File}";
                    foreach (var child in node)
                    {
                        MarkOutdated(child, parent);
                        if (child.DisplayWarning == true)
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
                    File = profile,
                    Value = profile?.File
                });

                AddCorridor(routeNode, related);
                AddGroup(routeNode, related, ProjectController.FClass.Survey, "Vytyčení", "./Assets/survey.png", "./Assets/list-item.png");
                AddGroup(routeNode, related, ProjectController.FClass.IFC, "Podklady pro IFC", "./Assets/ifc.png", "./Assets/list-item.png");
                AddGroup(routeNode, related, ProjectController.FClass.CombinedCrossSections, "Kreslení příčných řezů", null, "./Assets/list-item.png");
                MarkOutdated(routeNode, route);
                tree.Add(routeNode);
            }
            return tree;
        }

        [RPPrivateUseOnly]
        private IEnumerable<ProjectController.ProjectFile> GetByFlag(HashSet<ProjectController.ProjectFile> related, ProjectController.FClass flag) 
            => related.Where(f => f.Flag == flag);

        [RPPrivateUseOnly]
        private void AddGroup(TreeItem parent, IEnumerable<ProjectController.ProjectFile> related, ProjectController.FClass flag, string label,
            string groupNodeImage = null, string itemNodeImage = null)
        {
            var groupNode = new TreeItem { Label = label, IsGroupNode = true, Image = groupNodeImage };
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
            var node = new TreeItem { Label = "Koridor", IsGroupNode = true, Image = "./Assets/road.png" };
            if (corridor != null)
            {
                node.Add(new TreeItem { Label = $"Pokrytí:", Value = corridor.File, Image = "./Assets/list-item.png", File = corridor });
                var crossSection = related.FirstOrDefault(r => r.Flag == (ProjectController.FClass.Corridor | ProjectController.FClass.CrossSection));
                node.Add(new TreeItem
                {
                    Label = crossSection != null
                        ? $"Příčné řezy:"
                        : "Příčné řezy",
                    File = crossSection,
                    Value = crossSection?.File,
                    Image = "./Assets/list-item.png"
                });
            }
            parent.Add(node);
        }
    }
}