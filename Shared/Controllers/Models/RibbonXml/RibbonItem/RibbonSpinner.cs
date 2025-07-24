using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Shared.Controllers.Models.RibbonXml.RibbonItem
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSpinner
    public class RibbonSpinnerDef
    {
        [RPInternalUseOnly]
        [XmlIgnore]
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool? IsDouble = null;

        // Value must be as first field because later on we will use his datatype as global datatype
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the current value. " +
            "The data types int and double are supported by default. " +
            "To support other data types, derive from this class and override the virtual methods. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSpinner_Value
        public object Value { get; set; } = null;

        [RPInternalUseOnly]
        [XmlAttribute("Value")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ValueDef
        {
            get => Value?.ToString();
            set
            {
                if (IsDouble == null)
                {
                    // First we have to determine global datatype
                    if (int.TryParse(value, out int tW_int)) { Value = tW_int; IsDouble = false; }
                    else if (double.TryParse(value, out double tW_double)) { Value = tW_double; IsDouble = true;  }
                    // Supported only values are [int, double]
                    else { Value = null; }
                    return;
                }
                if (IsDouble != null)
                {
                    if (IsDouble == true && double.TryParse(value, out double tW_double)) {
                        Value = tW_double;
                        return;
                    }
                    else if (int.TryParse(value, out int tW_Int))
                    {
                        Value = tW_Int;
                        return;
                    }
                    Value = null;
                }
            }
        }

        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the value specifying the amount of change that occurs when the up or down spin button is pressed. " +
            "The data type of the value assigned to this property must be same as the data type of the Value property. " +
            "The data types int and double are supported by default. " +
            "To support other data types, derive from this class and override the virtual methods. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSpinner_Change
        public object Change { get; set; } = null;

        [RPInternalUseOnly]
        [XmlAttribute("Change")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ChangeDef
        {
            get => Change?.ToString();
            set
            {
                // First we have to determine global datatype
                if (int.TryParse(value, out int tW_int)) { Change = tW_int; }
                else if (double.TryParse(value, out double tW_double)) { Change = tW_double; }
                // Supported only values are [int, double]
                else { Change = null; }
            }
        }
    }
}