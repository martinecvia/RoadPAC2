using System;
using System.Windows;
using System.Windows.Controls;

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