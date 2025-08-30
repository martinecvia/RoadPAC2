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
                routeNode.Add(new TreeItem { DisplayName = $"Směrové řešení: {route.File}", File = route });
                var related = RPApp.Projector?.GetRoute(lsPath, route.File) ?? new HashSet<ProjectFile>();
                void MarkOutdated(TreeItem node, DateTime? parentUpdatedAt)
                {
                    if (node.File?.UpdatedAt != null && parentUpdatedAt != null)
                    {
                        var tolerance = TimeSpan.FromSeconds(10);
                        var diff = parentUpdatedAt - node.File.UpdatedAt;
                        node.DisplayWarning = diff > tolerance;
                    }
                    else if (parentUpdatedAt.HasValue)
                        node.DisplayWarning = true;
                        var currentUpdatedAt = node.File?.UpdatedAt ?? parentUpdatedAt;
                    foreach (var child in node)
                        MarkOutdated(child, currentUpdatedAt);
                    if (node.File == null || node.IsRouteNode)
                    {
                        // We don't want to display Route or empty filed nodes
                        node.DisplayWarning = false;
                        return;
                    }
                    Debug.WriteLine($"{node.File?.File}, DisplayWarning = {node.DisplayWarning}, because: {node.File?.UpdatedAt} < {parentUpdatedAt}");
                }

                var profile = related.FirstOrDefault(r => r.Flag.HasFlag(FClass.Profile) && !r.Flag.HasFlag(FClass.Listing));
                routeNode.Add(new TreeItem
                {
                    DisplayName = profile != null
                        ? $"Niveleta: {profile.File}"
                        : "Niveleta",
                    File = profile,
                });

                AddCorridor(routeNode, related);
                AddGroup(routeNode, related, FClass.Survey, "Vytyčení", "./Assets/survey.png", "./Assets/list-item.png");
                AddGroup(routeNode, related, FClass.IFC, "Podklady pro IFC", "./Assets/ifc.png", "./Assets/list-item.png");
                AddGroup(routeNode, related, FClass.CombinedCrossSections, "Kreslení příčných řezů", null, "./Assets/list-item.png");
                MarkOutdated(routeNode, route.UpdatedAt);
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
                        groupNode.Add(new TreeItem { DisplayName = file.File, Image = itemNodeImage, File = file });
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
                node.Add(new TreeItem { DisplayName = $"Pokrytí: {corridor.File}", Image = "./Assets/list-item.png", File = corridor });
                var crossSection = related.FirstOrDefault(r => r.Flag == (FClass.Corridor | FClass.CrossSection));
                if (crossSection != null)
                    node.Add(new TreeItem { DisplayName = $"Příčné řezy: {crossSection.File}", Image = "./Assets/list-item.png", File = crossSection });
            }
            parent.Add(node);
        }
    }
}