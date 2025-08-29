using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
using Shared.Controllers;

namespace Shared.Windows.Tree
{
    public class TreeItem
    {
        public string DisplayName { get; set; } = string.Empty;
        public List<TreeItem> Children { get; set; } = new List<TreeItem>();
        public string ImageSource
        {
            get => Image.UriSource.ToString();
        }
        public BitmapImage Image = new BitmapImage(new Uri("./Assets/route.png", UriKind.Relative));
        public int ItemCount { get; set; } = 0;
        public string ItemCountColor { get; set; } = "Green";
        public bool DisplayWarning { get; set; } = false;
    }
}