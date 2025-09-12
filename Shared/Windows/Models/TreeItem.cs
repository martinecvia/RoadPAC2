using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media.Imaging;

using Shared.Controllers;

namespace Shared.Windows.Models
{
    public class TreeItem : ObservableCollection<TreeItem>, INotifyPropertyChanged
    {
        private bool _selected;
        public bool IsSelected
        {
            get => _selected;
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public bool IsRouteNode { get; set; } = false;

        // IsActiveRoute
        private bool _activeRoute = false;
        public bool IsActiveRoute
        {
            get => _activeRoute;
            set
            {
                if (_activeRoute != value)
                {
                    _activeRoute = value;
                    Value = (value ? "(CurrentRoute)" : string.Empty);
                    ValueColor = (value ? "Green" : "White");
                    OnPropertyChanged(nameof(IsActiveRoute));
                }
            }
        }

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
                    OnPropertyChanged(nameof(RouteImage));
                    return;
                }
                try
                { _routeImage = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)); }
                catch
                { _routeImage = DefaultRouteImage; }
                OnPropertyChanged(nameof(RouteImage));
            }
        }

        public bool IsGroupNode { get; set; } = false;

        public ProjectController.ProjectFile File { get; set; }

        // Label
        public string Label { get; set; } = string.Empty;
        public string LabelColor { get; set; } = "Black";

        // Value
        private string _value = string.Empty;
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        private string _valueColor = "Black";
        public string ValueColor
        {
            get => _valueColor;
            set
            {
                if (_valueColor != value)
                {
                    _valueColor = value;
                    OnPropertyChanged(nameof(ValueColor));
                }
            }
        }

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
                    OnPropertyChanged(nameof(Image));
                    return;
                }
                try
                { _image = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)); }
                catch
                {  _image = DefaultImage; }
                OnPropertyChanged(nameof(Image));
            }
        }

        // ItemCount
        public int ItemCount => IsGroupNode ? Count : 0;
        private string _itemCountColor;
        public string ItemCountColor
        {
            get => _itemCountColor ?? (ItemCount < 3 ? "Green" : ItemCount <= 10 ? "Orange" : "Red");
            set
            {
                if (_itemCountColor != value)
                {
                    _itemCountColor = value;
                    OnPropertyChanged(nameof(ItemCountColor));
                }
            }
        }


        // Warning
        public bool DisplayWarning { get; set; } = false;
        private string _warningToolTip = "Soubor je zastaralý !";
        public string WarningToolTip
        {
            get => _warningToolTip;
            set
            {
                if (_warningToolTip != value)
                {
                    _warningToolTip = value;
                    OnPropertyChanged(nameof(WarningToolTip));
                }
            }
        }
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
                    OnPropertyChanged(nameof(WarningImage));
                    return;
                }
                try
                { _image = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)); }
                catch
                { _image = DefaultImage; }
                OnPropertyChanged(nameof(WarningImage));
            }
        }

        public new event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}