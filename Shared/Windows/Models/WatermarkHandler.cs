using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Shared.Windows.Models
{
    public static class WatermarkHandler
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark",
                typeof(string),
                typeof(WatermarkHandler),
                new PropertyMetadata(string.Empty, OnWatermarkChanged));

        public static string GetWatermark(DependencyObject obj) => (string)obj.GetValue(WatermarkProperty);
        public static void SetWatermark(DependencyObject obj, string value) => obj.SetValue(WatermarkProperty, value);

        [RPPrivateUseOnly]
        private static void OnWatermarkChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Reattach events
                textBox.GotFocus -= TextBox_GotFocus; textBox.GotFocus += TextBox_GotFocus;
                textBox.LostFocus -= TextBox_LostFocus; textBox.LostFocus += TextBox_LostFocus;
                textBox.TextChanged -= TextBox_TextChanged; textBox.TextChanged += TextBox_TextChanged;
                UpdateWatermark(textBox);
            }
        }

        [RPPrivateUseOnly]
        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (IsWatermarkDisplayed(textBox))
                {
                    textBox.Text = string.Empty;
                    ApplyNormalStyle(textBox);
                }
            }
        }

        [RPPrivateUseOnly]
        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
                UpdateWatermark(textBox);
        }

        [RPPrivateUseOnly]
        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (string.IsNullOrEmpty(textBox.Text) && !textBox.IsFocused)
                {
                    UpdateWatermark(textBox);
                }
                else if (!string.IsNullOrEmpty(textBox.Text))
                {
                    ApplyNormalStyle(textBox);
                }
            }
        }

        [RPPrivateUseOnly]
        private static void UpdateWatermark(TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text) && !textBox.IsFocused)
            {
                SetWatermarkDisplay(textBox, GetWatermark(textBox));
            }
            else if (!string.IsNullOrEmpty(textBox.Text))
            {
                ApplyNormalStyle(textBox);
            }
        }

        [RPPrivateUseOnly]
        private static bool IsWatermarkDisplayed(TextBox textBox) 
            => textBox.Text == GetWatermark(textBox) 
            && textBox.Foreground == SystemColors.GrayTextBrush;

        [RPPrivateUseOnly]
        private static void SetWatermarkDisplay(TextBox textBox, string watermarkText)
        {
            textBox.Text = watermarkText;
            textBox.Foreground = SystemColors.GrayTextBrush;
            textBox.FontStyle = FontStyles.Italic;
            textBox.Background = Brushes.White;
        }

        [RPPrivateUseOnly]
        private static void ApplyNormalStyle(TextBox textBox)
        {
            textBox.Foreground = SystemColors.ControlTextBrush;
            textBox.FontStyle = FontStyles.Normal;
            textBox.Background = Brushes.LightGoldenrodYellow;
        }
    }
}