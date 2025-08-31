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

using static Shared.Controllers.ProjectController;

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
                return item.DisplayName?.IndexOf(SearchText, System.StringComparison.OrdinalIgnoreCase) >= 0;
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
                    DisplayName = Path.GetFileNameWithoutExtension(route.File).ToUpperInvariant(),
                    IsRouteNode = true,
                    Image = "./Assets/route.png",
                    File = route
                };
                routeNode.Add(new TreeItem { DisplayName = $"Směrové řešení: {route.UpdatedAt.ToString()}", File = route });
                var related = RPApp.Projector?.GetRoute(lsPath, route.File) ?? new HashSet<ProjectFile>();
                // Not a great implementaton but it works at least a little
                void MarkOutdated(TreeItem node, ProjectFile parent)
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

                var profile = related.FirstOrDefault(r => r.Flag.HasFlag(FClass.Profile) && !r.Flag.HasFlag(FClass.Listing));
                routeNode.Add(new TreeItem
                {
                    DisplayName = profile != null
                        ? $"Niveleta: {profile.UpdatedAt.ToString()}"
                        : "Niveleta",
                    File = profile,
                });

                AddCorridor(routeNode, related);
                AddGroup(routeNode, related, FClass.Survey, "Vytyčení", "./Assets/survey.png", "./Assets/list-item.png");
                AddGroup(routeNode, related, FClass.IFC, "Podklady pro IFC", "./Assets/ifc.png", "./Assets/list-item.png");
                AddGroup(routeNode, related, FClass.CombinedCrossSections, "Kreslení příčných řezů", null, "./Assets/list-item.png");
                MarkOutdated(routeNode, route);
                tree.Add(routeNode);
            }
            return tree;
        }

        [RPPrivateUseOnly]
        private IEnumerable<ProjectFile> GetByFlag(HashSet<ProjectFile> related, FClass flag) 
            => related.Where(f => f.Flag == flag);

        [RPPrivateUseOnly]
        private void AddGroup(TreeItem parent, IEnumerable<ProjectFile> related, FClass flag, string label,
            string groupNodeImage = null, string itemNodeImage = null)
        {
            var groupNode = new TreeItem { DisplayName = label, IsGroupNode = true, Image = groupNodeImage };
            var matches = related.Where(r => r.Flag.HasFlag(flag)).ToList();
            if (matches.Any())
            {
                foreach (var file in matches)
                    if (!file.Flag.HasFlag(FClass.Listing))
                        groupNode.Add(new TreeItem { DisplayName = file.UpdatedAt.ToString(), Image = itemNodeImage, File = file });
            }
            parent.Add(groupNode);
        }

        [RPPrivateUseOnly]
        private void AddCorridor(TreeItem parent, IEnumerable<ProjectFile> related)
        {
            var corridor = related.FirstOrDefault(r => r.Flag == FClass.Corridor);
            var node = new TreeItem { DisplayName = "Koridor", IsGroupNode = true, Image = "./Assets/road.png" };
            if (corridor != null)
            {
                node.Add(new TreeItem { DisplayName = $"Pokrytí: {corridor.UpdatedAt.ToString()}", Image = "./Assets/list-item.png", File = corridor });
                var crossSection = related.FirstOrDefault(r => r.Flag == (FClass.Corridor | FClass.CrossSection));
                if (crossSection != null)
                    node.Add(new TreeItem { DisplayName = $"Příčné řezy: {crossSection.UpdatedAt.ToString()}", Image = "./Assets/list-item.png", File = crossSection });
            }
            parent.Add(node);
        }
    }
}