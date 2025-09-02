using System;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

using Shared.Controllers;

namespace Shared.Windows.Models
{
    public class TreeItem : ObservableCollection<TreeItem>
    {
        public bool IsRouteNode { get; set; } = false;
        public bool IsActiveRoute { get; set; } = false;
        private static readonly BitmapImage DefaultRouteImage =
            new BitmapImage(new Uri("./Assets/DebugXSLT.png", UriKind.Relative));
        private BitmapImage _routeImage = DefaultRouteImage;
        public string RouteImageSource => _routeImage?.UriSource.ToString();
        public string RouteImage
        {
            get => RouteImageSource;
            set
            {
                if (value == null)
                {
                    _routeImage = DefaultRouteImage;
                    return;
                }
                try
                { _routeImage = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)); }
                catch
                { _routeImage = DefaultRouteImage; }
            }
        }

        public bool IsGroupNode { get; set; } = false;

        public ProjectController.ProjectFile File { get; set; }

        // Label
        public string Label { get; set; } = string.Empty;
        public string LabelColor { get; set; } = "Black";

        // Value
        public string Value { get; set; } = string.Empty;
        public string ValueColor { get; set; } = "Black";

        // Image
        private static readonly BitmapImage DefaultImage =
            new BitmapImage(new Uri("./Assets/arrow-right.png", UriKind.Relative));
        private BitmapImage _image = DefaultImage;
        public string ImageSource => _image?.UriSource.ToString();
        public string Image
        {
            get => ImageSource;
            set
            {
                if (value == null)
                {
                    _image = DefaultImage;
                    return;
                }
                try
                { _image = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)); }
                catch
                {  _image = DefaultImage; }
            }
        }

        // ItemCount
        public int ItemCount => IsGroupNode ? Count : 0;
        private string _itemCountColor;
        public string ItemCountColor
        {
            get => _itemCountColor ?? (ItemCount < 3 ? "Green" : ItemCount <= 10 ? "Orange" : "Red");
            set => _itemCountColor = value;
        }


        // Warning
        public bool DisplayWarning { get; set; } = false;
        public string WarningToolTip = string.Empty;
        private static readonly BitmapImage DefaultWarningImage =
            new BitmapImage(new Uri("./Assets/warning.png", UriKind.Relative));
        private BitmapImage _warningImage = DefaultWarningImage;
        public string WarningImageSource => _warningImage?.UriSource.ToString();
        public string WarningImage
        {
            get => WarningImageSource;
            set
            {
                if (value == null)
                {
                    _warningImage = DefaultWarningImage;
                    return;
                }
                try
                { _image = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)); }
                catch
                { _image = DefaultImage; }
            }
        }
    }
}