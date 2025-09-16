using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace Shared.Windows.Models
{
    // https://github.com/xceedsoftware/wpftoolkit/blob/master/ExtendedWPFToolkitSolution/Src/Xceed.Wpf.Toolkit/AutoSelectTextBox/Implementation/AutoSelectTextBox.cs
    public class AutoSelectTextBox : System.Windows.Controls.TextBox
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

        [RPInfoOut]
        public AutoSelectBehavior AutoSelectBehavior
        {
            get => (AutoSelectBehavior)GetValue(AutoSelectBehaviorProperty);
            set => SetValue(AutoSelectBehaviorProperty, value);
        }

        [RPInfoOut]
        public bool AutoMoveFocus
        {
            get => (bool)GetValue(AutoMoveFocusProperty);
            set => SetValue(AutoMoveFocusProperty, value);
        }
        #region EVENTS
        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (!this.AutoMoveFocus)
            {
                base.OnPreviewKeyDown(e);
                return;
            }
            if ((e.Key == Key.Left)
                && ((Keyboard.Modifiers == ModifierKeys.None)
                || (Keyboard.Modifiers == ModifierKeys.Control)))
                e.Handled = this.MoveFocusLeft();
            if ((e.Key == Key.Right)
                && ((Keyboard.Modifiers == ModifierKeys.None)
                || (Keyboard.Modifiers == ModifierKeys.Control)))
                e.Handled = this.MoveFocusRight();
            if (((e.Key == Key.Up) || (e.Key == Key.PageUp))
                && ((Keyboard.Modifiers == ModifierKeys.None)
                || (Keyboard.Modifiers == ModifierKeys.Control)))
                e.Handled = this.MoveFocusUp();
            if (((e.Key == Key.Down) || (e.Key == Key.PageDown))
                && ((Keyboard.Modifiers == ModifierKeys.None)
                || (Keyboard.Modifiers == ModifierKeys.Control)))
                e.Handled = this.MoveFocusDown();
            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnPreviewGotKeyboardFocus(e);
            if (this.AutoSelectBehavior == AutoSelectBehavior.OnFocus)
            {
                if (!IsDescendantOf(e.OldFocus as DependencyObject, this))
                    this.SelectAll();
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (this.AutoSelectBehavior == AutoSelectBehavior.Never)
                return;
            if (this.IsKeyboardFocusWithin == false)
            {
                this.Focus();
                e.Handled = true;
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (!this.AutoMoveFocus)
                return;
            if ((this.Text.Length != 0) && (this.Text.Length == this.MaxLength) && (this.CaretIndex == this.MaxLength))
            {
                if (this.CanMoveFocus(FocusNavigationDirection.Right, true))
                {
                    FocusNavigationDirection direction = (this.FlowDirection == System.Windows.FlowDirection.LeftToRight)
                        ? FocusNavigationDirection.Right
                        : FocusNavigationDirection.Left;
                    this.MoveFocus(new TraversalRequest(direction));
                }
            }
        }
        #endregion
        #region PRIVATE
        [RPPrivateUseOnly]
        private bool IsDescendantOf(DependencyObject element,
                                   DependencyObject parent, bool recurseIntoPopup = true)
        {
            while (element != null)
            {
                if (element == parent)
                    return true;
                element = GetParent(element, recurseIntoPopup);
            }
            return false;
        }

        [RPPrivateUseOnly]
        private DependencyObject GetParent(DependencyObject element, bool recurseIntoPopup)
        {
            if (recurseIntoPopup)
            {
                Popup popup = element as Popup;
                if ((popup != null) && (popup.PlacementTarget != null))
                    return popup.PlacementTarget;
            }
            Visual visual = element as Visual;
            DependencyObject parent = (visual == null) ? null : VisualTreeHelper.GetParent(visual);
            if (parent == null)
            {
                FrameworkElement frameworkElement = element as FrameworkElement;
                if (frameworkElement != null)
                {
                    parent = frameworkElement.Parent;
                    if (parent == null)
                        parent = frameworkElement.TemplatedParent;
                }
                else
                {
                    FrameworkContentElement fcElement = element as FrameworkContentElement;
                    if (fcElement != null)
                    {
                        parent = fcElement.Parent;
                        if (parent == null)
                            parent = fcElement.TemplatedParent;
                    }
                }
            }
            return parent;
        }

        [RPPrivateUseOnly]
        private bool CanMoveFocus(FocusNavigationDirection direction, bool maxLength)
        {
            QueryMoveFocusEventArgs e = new QueryMoveFocusEventArgs(direction, maxLength);
            this.RaiseEvent(e);
            return e.CanMoveFocus;
        }

        [RPPrivateUseOnly]
        private bool MoveFocusLeft()
        {
            if (this.FlowDirection == System.Windows.FlowDirection.LeftToRight)
            {
                if ((this.CaretIndex == 0) && (this.SelectionLength == 0))
                {
                    if (ComponentCommands.MoveFocusBack.CanExecute(null, this))
                    {
                        ComponentCommands.MoveFocusBack.Execute(null, this);
                        return true;
                    }
                    else if (this.CanMoveFocus(FocusNavigationDirection.Left, false))
                    {
                        this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                        return true;
                    }
                }
            }
            else
            {
                if ((this.CaretIndex == this.Text.Length) && (this.SelectionLength == 0))
                {
                    if (ComponentCommands.MoveFocusBack.CanExecute(null, this))
                    {
                        ComponentCommands.MoveFocusBack.Execute(null, this);
                        return true;
                    }
                    else if (this.CanMoveFocus(FocusNavigationDirection.Left, false))
                    {
                        this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                        return true;
                    }
                }
            }
            return false;
        }

        [RPPrivateUseOnly]
        private bool MoveFocusRight()
        {
            if (this.FlowDirection == System.Windows.FlowDirection.LeftToRight)
            {
                if ((this.CaretIndex == this.Text.Length) && (this.SelectionLength == 0))
                {
                    if (ComponentCommands.MoveFocusForward.CanExecute(null, this))
                    {
                        ComponentCommands.MoveFocusForward.Execute(null, this);
                        return true;
                    }
                    else if (this.CanMoveFocus(FocusNavigationDirection.Right, false))
                    {
                        this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                        return true;
                    }
                }
            } 
            else
            {
                if ((this.CaretIndex == 0) && (this.SelectionLength == 0))
                {
                    if (ComponentCommands.MoveFocusForward.CanExecute(null, this))
                    {
                        ComponentCommands.MoveFocusForward.Execute(null, this);
                        return true;
                    }
                    else if (this.CanMoveFocus(FocusNavigationDirection.Right, false))
                    {
                        this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                        return true;
                    }
                }
            }
            return false;
        }

        [RPPrivateUseOnly]
        private bool MoveFocusUp()
        {
            int lineNumber = this.GetLineIndexFromCharacterIndex(this.SelectionStart);
            if (lineNumber == 0)
            {
                if (ComponentCommands.MoveFocusUp.CanExecute(null, this))
                {
                    ComponentCommands.MoveFocusUp.Execute(null, this);
                    return true;
                }
                else if (this.CanMoveFocus(FocusNavigationDirection.Up, false))
                {
                    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
                    return true;
                }
            }
            return false;
        }

        [RPPrivateUseOnly]
        private bool MoveFocusDown()
        {
            int lineNumber = this.GetLineIndexFromCharacterIndex(this.SelectionStart);
            if (lineNumber == (this.LineCount - 1))
            {
                if (ComponentCommands.MoveFocusDown.CanExecute(null, this))
                {
                    ComponentCommands.MoveFocusDown.Execute(null, this);
                    return true;
                }
                else if (this.CanMoveFocus(FocusNavigationDirection.Down, false))
                {
                    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}