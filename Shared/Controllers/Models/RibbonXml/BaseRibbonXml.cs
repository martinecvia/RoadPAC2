using System.Collections;
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
    }
}
