#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8603

using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Shared.Controllers.Models.RibbonXml
{
    public abstract class BaseRibbonXml
    {
        /// <summary>
        /// Returns a string representation of the object, including only properties marked with <see cref="RPInfoOutAttribute"/>
        /// that have non-null values.
        /// </summary>
        /// <remarks>
        /// - Properties must be public instance properties.<br/>
        /// - Properties are included only if decorated with <c>[RPInfoOut]</c> and their value is not <c>null</c>.<br/>
        /// - If a property's value is a collection (excluding strings), the output will list the items, joined by commas.<br/>
        /// - If an item in the collection is of type <see cref="BaseRibbonXml"/>, its <c>ToString()</c> result is used.
        /// </remarks>
        /// <returns>
        /// A formatted string like: <c>ClassName(Property1=Value1, Property2=[Item1, Item2], ...)</c>.
        /// </returns>
        public override string ToString()
        {
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var values = properties.Where(property => property.GetCustomAttribute<RPInfoOutAttribute>() != null)
                .Where(property => property.GetValue(this) != null)
                .Select(property =>
                {
                    object value = property.GetValue(this);
                    if (value is IEnumerable enumerable && !(value is string))
                    {
                        var list = string.Join(", ", enumerable.Cast<object>()
                            .Select(item => item is BaseRibbonXml xml ? xml.ToString() : item?.ToString()));
                        return $"{property.Name}=[{list}]";
                    }
                    return $"{property.Name}={value}";
                });
            return $"{GetType().Name}({string.Join(", ", values)})";
        }

        internal Target Transform<Target, Source>(Target target, Source source) where Source : BaseRibbonXml
        {
            if (target == null || source == null)
                return default;
            PropertyInfo[] applyableProperties = typeof(Source)
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(property => property.GetCustomAttribute<RPInfoOutAttribute>() != null)
                    .ToArray();
            foreach (PropertyInfo property in applyableProperties)
            {
                if (!property.CanRead)
                    continue; // Very weird? Must be manipulated in memory
                try
                {
                    // Since we are supporting multiple versions of CAD
                    // we have to first check if property exists in current running version
                    // if not we will just print information into a debug console and call it a day
                    PropertyInfo targetProperty = typeof(Target)
                        .GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public);
                    if (targetProperty?.CanWrite == true
                        && targetProperty.PropertyType.IsAssignableFrom(property.PropertyType))
                        targetProperty.SetValue(target, property.GetValue(source), null);
                    else
                    {
#if DEBUG
                        Debug.WriteLine($"{property.Name}: " +
                            $"Has different type target:{targetProperty.PropertyType} from source:{property.PropertyType}");
#endif
                    }
                }
                catch (System.Exception exception) // Collision with *CAD.Runtime.Exception & System.Exception
                {
#if DEBUG
                    Debug.WriteLine($"{property.Name}: {exception.Message}");
#endif
                }
            }
            return target;
        }
    }
}