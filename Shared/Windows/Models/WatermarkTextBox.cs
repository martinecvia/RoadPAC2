using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Shared.Windows.Models
{
    // https://github.com/xceedsoftware/wpftoolkit/blob/master/ExtendedWPFToolkitSolution/Src/Xceed.Wpf.Toolkit/WatermarkTextBox/Implementation/WatermarkTextBox.cs
    [RPInfoOut]
    public class WatermarkTextBox : AutoSelectTextBox
    {
        public static readonly DependencyProperty KeepWatermarkOnGotFocusProperty = DependencyProperty.Register(
            "KeepWatermarkOnGotFocus", 
            typeof(bool), 
            typeof(WatermarkTextBox), 
            new UIPropertyMetadata(false));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register(
            "Watermark", 
            typeof(object), 
            typeof(WatermarkTextBox),
            new UIPropertyMetadata(null));
        public static readonly DependencyProperty WatermarkTemplateProperty = DependencyProperty.Register(
            "WatermarkTemplate",
            typeof(DataTemplate),
            typeof(WatermarkTextBox),
            new UIPropertyMetadata(null));

        static WatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(WatermarkTextBox),
                new FrameworkPropertyMetadata(
                    typeof(WatermarkTextBox)));
        }

        [RPInfoOut]
        public static WatermarkTextBox Factory(int height = 24,
                                               bool keepWatermarkOnGotFocus = false,
                                               string imagePath = "rp_SearchContract.png", 
                                               string watermarkText = "Search...")
        {
            StackPanel panel = new StackPanel { Orientation = Orientation.Horizontal };
            panel.Children.Add(new TextBlock { Text = watermarkText, Margin = new Thickness(4, 0, 0, 0) });
            return new WatermarkTextBox
            {
                Height = height,
                VerticalContentAlignment = VerticalAlignment.Center,
                KeepWatermarkOnGotFocus = keepWatermarkOnGotFocus,
                Watermark = panel,
                Text = "{Binding SearchText, UpdateSourceTrigger=PropertyChanged}"
            };
        }

        [RPInfoOut]
        public DataTemplate WatermarkTemplate
        {
            get => (DataTemplate)GetValue(WatermarkTemplateProperty);
            set => SetValue(WatermarkTemplateProperty, value);
        }

        [RPInfoOut]
        public object Watermark
        {
            get => GetValue(WatermarkProperty);
            set => SetValue(WatermarkProperty, value);
        }

        [RPInfoOut]
        public bool KeepWatermarkOnGotFocus
        {
            get => (bool)GetValue(KeepWatermarkOnGotFocusProperty);
            set => SetValue(KeepWatermarkOnGotFocusProperty, value);
        }
    }
}