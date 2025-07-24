using System.Collections;
using System.Linq;
using System.Reflection;

namespace Shared.Controllers.Models.RibbonXml
{
    public abstract class BaseRibbonXml
    {
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
