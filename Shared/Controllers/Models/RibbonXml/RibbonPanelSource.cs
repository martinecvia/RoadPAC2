#pragma warning disable CS8601
#pragma warning disable CS8603
#pragma warning disable CS8625

using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
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
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource
    [XmlInclude(typeof(RibbonPanelSourceDef))]
    [XmlInclude(typeof(RibbonPanelSpacerDef))]
    public class RibbonPanelSourceDef : BaseRibbonXml
    {
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_Description
        [RPInfoOut]
        [XmlAttribute("Description")]
        [DefaultValue(null)]
        [Description("Gets or sets the panel description text. " +
            "The description text is not currently used by the framework. " +
            "Applications can use this to store a description if it is required in other UI customization dialogs. " +
            "The default value is null.")]
        public string Description { get; set; } = null;
        /*
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the command item to be used as the panel's dialog launcher. " +
            "The dialog launcher is displayed as a small button in the panel title bar. " +
            "Clicking the button raises a command that follows the standard ribbon command routing. " +
            "If this property is null the panel does not have a dialog launcher button. " +
            "The default value is null.")]
        public RibbonCommandItem DialogLauncher => Transform(new RibbonCommandItem(), DialogLauncherDef);

        [RPInternalUseOnly]
        [XmlElement("DialogLauncher")]
        public RibbonCommandItemDef DialogLauncherDef { get; set; }
        */
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_Id
        [RPInfoOut]
        [XmlAttribute("Id")]
        [DefaultValue(null)]
        [Description("Gets or sets the id for the panel source. " +
            "The framework does not use or validate this id. " +
            "It is left to an application to set and use this id. " +
            "The default value is null.")]
        public string Id { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_Name
        [RPInfoOut]
        [XmlAttribute("Name")]
        [DefaultValue(null)]
        [Description("Gets or sets the name of the ribbon panel. " +
            "The framework uses the Title property of the panel to display the panel title in the ribbon. " +
            "The name property is not currently used by the framework. " +
            "Applications can use this property to store a longer name for a panel if this is required in other UI customization dialogs. " +
            "The default value is null.")]
        public string Name { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_KeyTip
        [RPInfoOut]
        [XmlAttribute("KeyTip")]
        [DefaultValue(null)]
        [Description("Gets or sets the name of the ribbon panel. " +
            "The framework uses the Title property of the panel to display the panel title in the ribbon. " +
            "The name property is not currently used by the framework. " +
            "Applications can use this property to store a longer name for a panel if this is required in other UI customization dialogs. " +
            "The default value is null.")]
        public string KeyTip { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_Tag
        [RPInfoOut]
        [XmlAttribute("Tag")]
        [DefaultValue(null)]
        [Description("Gets or sets the custom data object in the panel source. " +
            "This property can be used to store any object a as custom data object in a panel source. " +
            "This data is not used by the framework. " +
            "The default value is null.")]
        public string Tag { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_Title
        [RPInfoOut]
        [XmlAttribute("Title")]
        [DefaultValue(null)]
        [Description("The panel title set with this property is displayed in the panel's title bar in the ribbon. " +
            "The default value is null.")]
        public string Title { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSpacer
        public class RibbonPanelSpacerDef : RibbonPanelSourceDef
        {
            /*
             * Creation:
            <RibbonPanelSpacerDef xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                  xmlns:xsd="http://www.w3.org/2001/XMLSchema">
              <LeftBorderBrush>
                &lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" Color="#FFFF0000" /&gt;
              </LeftBorderBrush>
              <RightBorderBrush>
                &lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" Color="#FF0000FF" /&gt;
              </RightBorderBrush>
            </RibbonPanelSpacerDef>
             */
            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue("Transparent")]
            [Description("This is LeftBorderBrush, a member of class RibbonPanelSpacer.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSpacer_LeftBorderBrush
            public Brush LeftBorderBrush { get; set; } = Brushes.Transparent;

            [RPInternalUseOnly]
            [XmlElement("LeftBorderBrush")]
            public XmlElement LeftBorderBrushDef
            {
                get
                {
                    if (LeftBorderBrush == null)
                        return null;
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(XamlWriter.Save(LeftBorderBrush));
                    return document.DocumentElement;
                }
                set
                {
                    if (value != null)
                    {
                        string xaml = value.OuterXml;
                        LeftBorderBrush = (Brush)XamlReader.Parse(xaml);
                    }
                    else
                    {
                        LeftBorderBrush = Brushes.Transparent;
                    }
                }
            }

            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue("Transparent")]
            [Description("This is RightBorderBrush, a member of class RibbonPanelSpacer.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSpacer_RightBorderBrush
            public Brush RightBorderBrush { get; set; } = Brushes.Transparent;

            [RPInternalUseOnly]
            [XmlElement("RightBorderBrush")]
            public XmlElement RightBorderBrushDef
            {
                get
                {
                    if (RightBorderBrush == null)
                        return null;
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(XamlWriter.Save(RightBorderBrush));
                    return document.DocumentElement;
                }
                set
                {
                    if (value != null)
                    {
                        string xaml = value.OuterXml;
                        RightBorderBrush = (Brush)XamlReader.Parse(xaml);
                    }
                    else
                    {
                        RightBorderBrush = Brushes.Transparent;
                    }
                }
            }
        }
    }
}