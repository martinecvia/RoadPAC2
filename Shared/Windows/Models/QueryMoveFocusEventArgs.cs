using System.Windows;
using System.Windows.Input;

namespace Shared.Windows.Models
{
    // https://github.com/xceedsoftware/wpftoolkit/blob/master/ExtendedWPFToolkitSolution/Src/Xceed.Wpf.Toolkit/AutoSelectTextBox/Implementation/QueryMoveFocusEventArgs.cs
    [RPInfoOut]
    public class QueryMoveFocusEventArgs : RoutedEventArgs
    {
        private FocusNavigationDirection m_navigationDirection;
        private bool m_maxLength;
        private bool m_canMoveFocus = true;

        public FocusNavigationDirection FocusNavigationDirection => m_navigationDirection;
        public bool IsMaxLength => m_maxLength;

        public bool CanMoveFocus
        {
            get => m_canMoveFocus;
            set => m_canMoveFocus = value;
        }

        [RPInternalUseOnly]
        internal QueryMoveFocusEventArgs(FocusNavigationDirection navigationDirection, bool reachedMaxLength)
            : base (AutoSelectTextBox.QueryMoveFocusEvent)
        {
            m_navigationDirection = navigationDirection;
            m_maxLength = reachedMaxLength;
        }

        [RPPrivateUseOnly]
        private QueryMoveFocusEventArgs()
        { }
    }

    public delegate void QueryMoveFocusEventHandler(object sender, QueryMoveFocusEventArgs e);
}