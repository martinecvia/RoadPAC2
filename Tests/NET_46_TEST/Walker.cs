using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Autodesk.Windows;

namespace NET_46_TEST
{
    public class ClassWalker
    {
        public ClassWalker()
        {
            Type[] walkableTypes = new[] {typeof(RibbonButton), typeof(RibbonCommandItem), typeof(RibbonItem), typeof(RibbonLabel), typeof(RibbonList), typeof(RibbonPanelBreak), typeof(RibbonRowBreak), typeof(RibbonRowPanel),
                typeof(RibbonForm),  typeof(RibbonHwnd), typeof(RibbonSeparator), typeof(RibbonSlider), typeof(RibbonSpinner), typeof(RibbonTextBox), typeof(RibbonPanel), typeof(RibbonTab), typeof(RibbonPanelSpacer), typeof(RibbonCombo), typeof(RibbonGallery)};
            foreach (Type type in walkableTypes)
            {
                Debug.WriteLine($"--- {type.Name} ---");
                string name = type.Name;
                PropertyInfo[] availibleProperties = type
                   .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   .Where(property => property.SetMethod != null && property.SetMethod.IsPublic && !property.SetMethod.IsStatic).ToArray();
                foreach (PropertyInfo property in availibleProperties)
                {
                    var declaringSetterType = property.SetMethod.DeclaringType;
                    var inheritedFrom = (declaringSetterType != null
                        && declaringSetterType != type) ? $"{declaringSetterType.Name}->" : "";
                    var attributes = property.GetCustomAttributes(false)
                        .Where(attribute => attribute is DefaultValueAttribute 
                        || (attribute.GetType().Namespace != null && attribute.GetType().Namespace.StartsWith("Autodesk.Windows")));
                    foreach (var attribute in attributes)
                    {
                        if (attribute is DefaultValueAttribute)
                        {
                            Debug.WriteLine($"[{string.Concat(attribute.GetType().Name.Reverse().Skip(9).Reverse())}({((DefaultValueAttribute)attribute).Value ?? ""})]");
                        } else
                        {
                            Debug.WriteLine($"[{string.Concat(attribute.GetType().Name.Reverse().Skip(9).Reverse())}]");
                        }
                    }
                    Debug.WriteLine($"{inheritedFrom}{property.PropertyType.FullName} {property.Name}");
                    Debug.WriteLine("");
                }
                Debug.WriteLine("");
                Debug.WriteLine("");
            }
        }
    }
}
