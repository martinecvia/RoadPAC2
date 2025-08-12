using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Shared;
using Shared.Controllers.Models.RibbonXml;

namespace NET_46_TEST
{
    internal class Documenteer
    {
        public Documenteer()
        {
            Type baseXml = typeof(BaseRibbonXml);
            Assembly assembly = baseXml.Assembly;
            List<Type> derivedClasses = assembly.GetTypes().Where(t => t.IsClass && baseXml.IsAssignableFrom(t)).ToList();
            foreach (Type type in derivedClasses)
            {
                Debug.WriteLine($"--- {type.Name} ---");
                PropertyInfo[] properties = type.GetProperties().Where(property => property.GetCustomAttribute<RPInfoOutAttribute>() != null).ToArray();
                Debug.WriteLine(string.Join(", ", properties.Select(p => p.Name).ToArray()));
                var attributes = properties.Select(p => {
                    var def = p.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault();
                    return $"{p.Name}=" + '"' + (def?.Value ?? "") + '"'; 
                }).ToArray();
                Debug.WriteLine($"<{type.Name} {string.Join(" ", attributes)}");
                foreach (PropertyInfo property in properties)
                {
                    var def = property.GetCustomAttributes<DefaultValueAttribute>().FirstOrDefault();
                    string DefString = "";
                    if (def != null)
                    {
                        if (def.Value != null && def.Value.GetType().IsEnum)
                            DefString = $" = {def.Value.GetType().Name}.{def.Value}";
                        else
                            DefString = $" = {def?.Value ?? "null"}";
                    }
                    var des = property.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
                    Type propType = property.PropertyType;
                    if (propType.IsArray)
                        propType = propType.GetElementType();
                    if (propType.IsGenericType && propType.GetGenericArguments().Length == 1)
                        propType = propType.GetGenericArguments()[0];
                    var enumValues = propType.IsEnum ? Enum.GetValues(propType) : null;
                    string enumString = enumValues != null
                        ? $" // Enum: [{string.Join(",", enumValues.Cast<object>())}]"
                        : "";
                    if (des != null)
                        Debug.WriteLine($"// {des.Description}");
                    Debug.WriteLine($"{property.Name}{DefString}{enumString}");
                }
                Debug.WriteLine("\n");
            }
        }
    }
}
