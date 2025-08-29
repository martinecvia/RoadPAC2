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
using Shared.Windows.Tree;

using static Shared.Controllers.ProjectController;

namespace Shared.Windows
{
    public class ProjectorViewModel
    {
        public ObservableCollection<TreeItem> Items { get; }
            = new ObservableCollection<TreeItem>();
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
            Items = new ObservableCollection<TreeItem>(BuildProjectTree(RPApp.Projector?.CurrentWorkingDirectory));
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
                return new List<TreeItem>();
            var tree = new List<TreeItem>();
            foreach (var route in routes)
            {
                var routeNode = new TreeItem { DisplayName = Path.GetFileNameWithoutExtension(route.File).ToUpperInvariant() };
                routeNode.Children.Add(new TreeItem { DisplayName = $"Směrové řešení: {route.File}" });

                var related = RPApp.Projector?.GetRoute(lsPath, route.File) ?? new HashSet<ProjectFile>();

                AddFirst(routeNode, related, FClass.Profile, "Niveleta");
                AddCorridor(routeNode, related);
                AddGroup(routeNode, related, FClass.Survey, "Vytyčení:");
                AddGroup(routeNode, related, FClass.IFC, "Podklady pro IFC:");
                AddGroup(routeNode, related, FClass.CombinedCrossSections, "Kreslení příčných řezů:");
                tree.Add(routeNode);
            }
            return tree;
        }

        [RPPrivateUseOnly]
        private IEnumerable<ProjectFile> GetByFlag(HashSet<ProjectFile> related, FClass flag) 
            => related.Where(f => f.Flag == flag);

        [RPPrivateUseOnly]
        private void AddFirst(TreeItem parent, IEnumerable<ProjectFile> related, FClass flag, string label)
        {
            var match = related.FirstOrDefault(r => r.Flag.HasFlag(flag) && !r.Flag.HasFlag(FClass.Listing));
            if (match != null)
                parent.Children.Add(new TreeItem { DisplayName = $"{label}: {match.File}" });
        }

        [RPPrivateUseOnly]
        private void AddGroup(TreeItem parent, IEnumerable<ProjectFile> related, FClass flag, string label)
        {
            var matches = related.Where(r => r.Flag.HasFlag(flag)).ToList();
            if (matches.Any())
            {
                var groupNode = new TreeItem { DisplayName = label, ItemCount = matches.Count(f => !f.Flag.HasFlag(FClass.Listing))};
                foreach (var file in matches)
                    if (!file.Flag.HasFlag(FClass.Listing))
                        groupNode.Children.Add(new TreeItem { DisplayName = file.File });
                parent.Children.Add(groupNode);
            }
        }

        [RPPrivateUseOnly]
        private void AddCorridor(TreeItem parent, IEnumerable<ProjectFile> related)
        {
            var corridor = related.FirstOrDefault(r => r.Flag == FClass.Corridor);
            if (corridor == null) return;
            var node = new TreeItem { DisplayName = "Koridor" };
            node.Children.Add(new TreeItem { DisplayName = $"Pokrytí: {corridor.File}" });
            var crossSection = related.FirstOrDefault(r => r.Flag == (FClass.Corridor | FClass.CrossSection));
            if (crossSection != null)
                node.Children.Add(new TreeItem { DisplayName = $"Příčné řezy: {crossSection.File}" });
            parent.Children.Add(node);
        }
    }
}
