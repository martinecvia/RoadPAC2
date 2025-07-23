using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Shared.Controllers.Models.RibbonXml.RibbonItem
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox
    public class RibbonTextBoxDef
    {
        [XmlIgnore]
        [DefaultValue(System.Windows.Controls.Orientation.Horizontal)]
        [Description("This is Orientation, a member of class RibbonTextBox.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_Orientation
        public System.Windows.Controls.Orientation Orientation { get; private set; } = System.Windows.Controls.Orientation.Horizontal;

        [XmlAttribute("Orientation")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string OrientationDef
        {
            private get => Orientation.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out System.Windows.Controls.Orientation result))
                    result = System.Windows.Controls.Orientation.Horizontal;
                Orientation = result;
            }
        }

        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value that indicates whether edited text should be accepted when the text box loses focus before enter is pressed or the text box button is clicked. " +
            "This property controls whether the edited text is accepted or rejected if the text box loses focus before the text box button is clicked or the enter key pressed. " +
            "If the value is true, the edited text is accepted. " +
            "If it is false, the edited text is rejected and the previous value is restored. " +
            "If the text is accepted, the command handler is invoked if InvokesCommand is true. " +
            "If the text box button is clicked or the enter key is pressed, the text is accepted regardless of this property value. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_AcceptTextOnLostFocus
        public bool AcceptTextOnLostFocus { get; private set; } = true;

        [XmlAttribute("AcceptTextOnLostFocus")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string AcceptTextOnLostFocusDef
        {
            private get => AcceptTextOnLostFocus.ToString();
            set
            {
                if (value == null)
                {
                    AcceptTextOnLostFocus = true;
                    return;
                }
                AcceptTextOnLostFocus
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the command handler to be called when the text is changed." +
            "The property InvokesCommand must be true for the command handler to be called. " +
            "If it is false, no command will be invoked by the text box. " +
            "Also, the command will not be invoked by the text box if the text is not accepted (i.e., validation has failed). " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_CommandHandler
        public CommandHandler CommandHandler { get; private set; } = null;

        [XmlAttribute("CommandHandler")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string CommandHandlerDef
        {
            private get => CommandHandler.Command;
            set
            {
                if (value != null)
                    CommandHandler = new CommandHandler(value);
            }
        }
    }
}
