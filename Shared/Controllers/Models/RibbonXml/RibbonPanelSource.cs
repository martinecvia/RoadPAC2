#pragma warning disable CS8601
#pragma warning disable CS8603
#pragma warning disable CS8625

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

using Shared.Controllers.Models.RibbonXml.Items;
using Shared.Controllers.Models.RibbonXml.Items.CommandItems;
using static Shared.Controllers.Models.RibbonXml.Items.RibbonItemDef;

namespace Shared.Controllers.Models.RibbonXml
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource
    [XmlInclude(typeof(RibbonPanelSourceDef))]
    [XmlInclude(typeof(RibbonPanelSpacerDef))]
    [Description("The RibbonPanelSource class is used to store and manage the content of a panel in a ribbon. " +
        "RibbonPanel references an object of this class in its Source property and displays the content from this class. " +
        "The content is a collection of RibbonItem objects stored in an Items collection. " +
        "The items can be organized into multiple rows by adding a RibbonRowBreak item at the index at where a new row is to start. " +
        "The items can also be organized into two panels - main panel and slide-out panel - by adding a RibbonPanelBreak item at the index where the slide-out panel is to start.")]
    public class RibbonPanelSourceDef : BaseRibbonXml
    {
        [XmlIgnore]
        public List<RibbonItem> Items
        {
            get
            {
                List<RibbonItem> items = new List<RibbonItem>();
                if (ItemsDef == null)
                    return items;
                foreach (RibbonItemDef element in ItemsDef)
                {
                    try
                    {
                        items.Add(Transform(ItemsFactory[element.GetType()](), element)); // For some reason I've set ItemsFactory.GetType(),
                                                                                          // instead of element.GetType() which resulted in System.Collections.Generic.KeyNotFoundException
                    }
                    catch (InvalidOperationException exception)
                    {
                        var underlayingException = exception.InnerException == null ? 
                            exception.Message : exception.InnerException.Message;
                        Debug.WriteLine($"RibbonPanelSourceDef({element.GetType().Name}): Failed to load: {underlayingException}");
                    }
                }
                return items;
            }
        }

        [RPInternalUseOnly]
        [XmlElement("RibbonButton", typeof(RibbonButtonDef))]
        [XmlElement("RibbonLabel", typeof(RibbonLabelDef))]
        [XmlElement("RibbonCombo", typeof(RibbonListDef.RibbonComboDef))]
        [XmlElement("RibbonGallery", typeof(RibbonListDef.RibbonComboDef.RibbonGalleryDef))]
        [XmlElement("RibbonPanelBreak", typeof(RibbonPanelBreakDef))]
        [XmlElement("RibbonRowBreak", typeof(RibbonRowBreakDef))]
        [XmlElement("RibbonRowPanel", typeof(RibbonRowPanelDef))]
        [XmlElement("RibbonFlowPanel", typeof(RibbonRowPanelDef.RibbonFlowPanelDef))]
        [XmlElement("RibbonFoldPanel", typeof(RibbonRowPanelDef.RibbonFoldPanelDef))]
        [XmlElement("RibbonSeparator", typeof(RibbonSeparatorDef))]
        [XmlElement("RibbonSlider", typeof(RibbonSliderDef))]
        [XmlElement("RibbonSpinner", typeof(RibbonSpinnerDef))]
        [XmlElement("RibbonTextBox", typeof(RibbonTextBoxDef))]
        public List<RibbonItemDef> ItemsDef { get; set; } = new List<RibbonItemDef>();

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_Description
        [RPInfoOut]
        [XmlAttribute("Description")]
        [DefaultValue(null)]
        [Description("Gets or sets the panel description text. " +
            "The description text is not currently used by the framework. " +
            "Applications can use this to store a description if it is required in other UI customization dialogs. " +
            "The default value is null.")]
        public string Description { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource_DialogLauncher
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the command item to be used as the panel's dialog launcher. " +
            "The dialog launcher is displayed as a small button in the panel title bar. " +
            "Clicking the button raises a command that follows the standard ribbon command routing. " +
            "If this property is null the panel does not have a dialog launcher button. " +
            "The default value is null.")]
        public RibbonCommandItem DialogLauncher { get; set; } = null;

        [RPInternalUseOnly]
        [XmlElement("DialogLauncher")]
        public RibbonCommandItemDef DialogLauncherDef { get; set; }

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
        [RPPrivateUseOnly]
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
                        LeftBorderBrush = (Brush) XamlReader.Parse(xaml);
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
                        RightBorderBrush = (Brush) XamlReader.Parse(xaml);
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