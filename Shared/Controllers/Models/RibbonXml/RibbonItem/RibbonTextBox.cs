﻿using System;
using System.ComponentModel;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml.RibbonItem
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox
    public class RibbonTextBoxDef : RibbonItemDef
    {
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(System.Windows.Controls.Orientation.Horizontal)]
        [Description("This is Orientation, a member of class RibbonTextBox.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_Orientation
        public System.Windows.Controls.Orientation Orientation { get; set; } = System.Windows.Controls.Orientation.Horizontal;

        [RPInternalUseOnly]
        [XmlAttribute("Orientation")]
        public string OrientationDef
        {
            get => Orientation.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out System.Windows.Controls.Orientation result))
                    result = System.Windows.Controls.Orientation.Horizontal;
                Orientation = result;
            }
        }

        [RPInfoOut]
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
        public bool AcceptTextOnLostFocus { get; set; } = true;

        [RPInternalUseOnly]
        [XmlAttribute("AcceptTextOnLostFocus")]
        public string AcceptTextOnLostFocusDef
        {
            get => AcceptTextOnLostFocus.ToString();
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

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the command handler to be called when the text is changed." +
            "The property InvokesCommand must be true for the command handler to be called. " +
            "If it is false, no command will be invoked by the text box. " +
            "Also, the command will not be invoked by the text box if the text is not accepted (i.e., validation has failed). " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_CommandHandler
        public CommandHandler CommandHandler { get; set; } = null;

        [RPInternalUseOnly]
        [XmlAttribute("CommandHandler")]
        public string CommandHandlerDef
        {
            get => CommandHandler?.Command;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    CommandHandler = new CommandHandler(value);
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(RibbonTextBoxImageLocation.Left)]
        [Description("Gets or sets the location for the image that is displayed in the text box. " +
            "ShowImage must be set to true to see the image. " +
            "This property is ignored if ShowImage is false. " +
            "The default value is Left.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_ImageLocation
        public RibbonTextBoxImageLocation ImageLocation { get; set; } = RibbonTextBoxImageLocation.Left;

        [RPInternalUseOnly]
        [XmlAttribute("ImageLocation")]
        public string ImageLocationDef
        {
            get => ImageLocation.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonTextBoxImageLocation result))
                    result = RibbonTextBoxImageLocation.Left;
                ImageLocation = result;
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates whether the command handler needs to be invoked whenever text is changed. " +
            " If the value is true, the command handler is invoked when text is changed in the UI; if the value is false, no command is invoked. " +
            "The default value is false.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_InvokesCommand
        public bool InvokesCommand { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("InvokesCommand")]
        public string InvokesCommandDef
        {
            get => InvokesCommand.ToString();
            set
            {
                if (value == null)
                {
                    InvokesCommand = false;
                    return;
                }
                InvokesCommand
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(true)] // Even tho this property does not directly have default value
                             // for better results this property will be true by default
        [Description("Gets or sets the value that indicates whether empty text should be considered a valid value. " +
            "If IsEmptyTextValid is true, empty text is considered valid text, and the command is invoked even if the text is empty; if it is false, when the text is empty, the command is not invoked, and the text box button is disabled.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_IsEmptyTextValid
        public bool IsEmptyTextValid { get; set; }

        [RPInternalUseOnly]
        [XmlAttribute("IsEmptyTextValid")]
        public string IsEmptyTextValidDef
        {
            get => IsEmptyTextValid.ToString();
            set
            {
                if (value == null)
                {
                    IsEmptyTextValid = true;
                    return;
                }
                IsEmptyTextValid
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlAttribute("Prompt")]
        [DefaultValue(null)]
        [Description("Gets or sets the prompt text for the text box. " +
            "Prompt text is displayed when the text box is empty and does not have keyboard focus. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_Prompt
        public string Prompt { get; set; } = null;

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates whether the text is selected when the text box gains focus. " +
            "If the value is true, all the text in the text box is selected when the text box gets keyboard focus. " +
            "If it is false, the text is not selected. " +
            "The default value is false.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_SelectTextOnFocus
        public bool SelectTextOnFocus { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("SelectTextOnFocus")]
        public string SelectTextOnFocusDef
        {
            get => SelectTextOnFocus.ToString();
            set
            {
                if (value == null)
                {
                    SelectTextOnFocus = false;
                    return;
                }
                SelectTextOnFocus
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates whether the Image set in the text box should be displayed as a clickable button. " +
            "If ShowImageAsButton is true, ShowImage must also be true, and ImageLocation must be set to InsideLeft or InsideRight. " +
            "If ShowImage is false, the button will not be visible. " +
            "If ImageLocation is Left, this property will not have any effect. " +
            "Clicking this button will invoke the command handler if InvokesCommand is true. " +
            "The default value is false.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_ShowImageAsButton
        public bool ShowImageAsButton { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("ShowImageAsButton")]
        public string ShowImageAsButtonDef
        {
            get => ShowImageAsButton.ToString();
            set
            {
                if (value == null)
                {
                    ShowImageAsButton = false;
                    return;
                }
                ShowImageAsButton
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlAttribute("Value")]
        [DefaultValue(null)]
        [Description("Gets or sets the Value property. " +
            "The value can be a string or other data type. " +
            "If it is not a string, you must derive from this class and implement the virtual data conversion methods. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTextBox_Value
        public string Value { get; set; } = null;

    }
}