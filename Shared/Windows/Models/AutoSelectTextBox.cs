using System.Windows;
using System.Windows.Controls;

namespace Shared.Windows.Models
{
    public class AutoSelectTextBox : TextBox
    {
        public static readonly DependencyProperty AutoSelectBehaviorProperty = DependencyProperty.Register(
            "AutoSelectBehavior", 
            typeof(AutoSelectBehavior), 
            typeof(AutoSelectTextBox),
            new UIPropertyMetadata(AutoSelectBehavior.Never));
        public static readonly DependencyProperty AutoMoveFocusProperty = DependencyProperty.Register(
            "AutoMoveFocus", 
            typeof(bool), 
            typeof(AutoSelectTextBox), 
            new UIPropertyMetadata(false));
        public static readonly RoutedEvent QueryMoveFocusEvent = EventManager.RegisterRoutedEvent(
            "QueryMoveFocus",
            RoutingStrategy.Bubble,
            typeof(QueryMoveFocusEventHandler),
            typeof(AutoSelectTextBox));

        public AutoSelectBehavior AutoSelectBehavior
        {
            get => (AutoSelectBehavior)GetValue(AutoSelectBehaviorProperty);
            set => SetValue(AutoSelectBehaviorProperty, value);
        }

        public bool AutoMoveFocus
        {
            get => (bool)GetValue(AutoMoveFocusProperty);
            set => SetValue(AutoMoveFocusProperty, value);
        }
    }
}