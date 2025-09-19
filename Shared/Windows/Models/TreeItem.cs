using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

using Shared.Controllers;

namespace Shared.Windows.Models
{
    public class TreeItem : ObservableCollection<TreeItem>
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
                    NotifyPropertyChanged(nameof(IsSelected));
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
                    NotifyPropertyChanged(nameof(IsActiveRoute));
                    Value = (value ? "(CurrentRoute)" : string.Empty);
                    ValueColor = (value ? "Green" : "White");
                }
            }
        }

        private static readonly BitmapImage DefaultRouteImage =
            new BitmapImage(new Uri("./Assets/rp_active_route.png", UriKind.Relative));
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
                    NotifyPropertyChanged(nameof(RouteImage));
                    return;
                }
                try
                { _routeImage = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)); }
                catch
                { _routeImage = DefaultRouteImage; }
                NotifyPropertyChanged(nameof(RouteImage));
            }
        }

        public bool IsGroupNode { get; set; } = false;

        public ProjectController.ProjectFile File { get; set; }

        // Label
        private string _label = string.Empty;
        public string Label
        {
            get => _label;
            set
            {
                if (_label != value)
                {
                    _label = value;
                    NotifyPropertyChanged(nameof(Label));
                } 
            }
        }
        private string _labelColor = "Black";
        public string LabelColor
        {
            get => _labelColor;
            set
            {
                if (_labelColor != value)
                {
                    _labelColor = value;
                    NotifyPropertyChanged(nameof(LabelColor));
                }
            }
        }

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
                    NotifyPropertyChanged(nameof(Value));
                }
            }
        }
        private string _valueColor = "Gray";
        public string ValueColor
        {
            get => _valueColor;
            set
            {
                if (_valueColor != value)
                {
                    _valueColor = value;
                    NotifyPropertyChanged(nameof(ValueColor));
                }
            }
        }

        // Image
        private static readonly BitmapImage DefaultImage =
            new BitmapImage(new Uri("./Assets/rp_group_item.png", UriKind.Relative));
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
                    NotifyPropertyChanged(nameof(Image));
                    return;
                }
                try
                { _image = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)); }
                catch
                {  _image = DefaultImage; }
                NotifyPropertyChanged(nameof(Image));
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
                    NotifyPropertyChanged(nameof(ItemCountColor));
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
                    NotifyPropertyChanged(nameof(WarningToolTip));
                }
            }
        }
        private static readonly BitmapImage DefaultWarningImage =
            new BitmapImage(new Uri("./Assets/rp_warning.png", UriKind.Relative));
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
                    NotifyPropertyChanged(nameof(WarningImage));
                    return;
                }
                try
                { _image = new BitmapImage(new Uri(value, UriKind.RelativeOrAbsolute)); }
                catch
                { _image = DefaultImage; }
                NotifyPropertyChanged(nameof(WarningImage));
            }
        }

        // INotifyPropertyChanged
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            => base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
}