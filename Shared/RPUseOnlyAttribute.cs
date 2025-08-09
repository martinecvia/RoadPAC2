using System; // Keep for .NET 4.6

namespace Shared
{
    /// <summary>
    /// Indicates that the associated code element is intended exclusively for private use
    /// within the RoadPAC system. This attribute serves as a marker and does not contain
    /// any additional logic or metadata.
    /// </summary>
    /// <remarks>
    /// This attribute can be applied to any code element, including classes, methods,
    /// properties, fields, etc. It supports multiple usages on a single element and is
    /// inherited by derived classes or overridden members.
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    internal sealed class RPPrivateUseOnlyAttribute : Attribute
    { }

    /// <summary>
    /// Indicates that the associated code element is intended exclusively for internal use
    /// within the RoadPAC system. This attribute serves as a marker and does not contain
    /// any additional logic or metadata.
    /// </summary>
    /// <remarks>
    /// This attribute can be applied to any code element, including classes, methods,
    /// properties, fields, etc. It supports multiple usages on a single element and is
    /// inherited by derived classes or overridden members.
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    internal sealed class RPInternalUseOnlyAttribute : Attribute
    { }

    /// <summary>
    /// Indicates that the associated code element participates in reflective data
    /// transfer operations within the RoadPAC system.
    /// This attribute is typically applied to members whose values are intended
    /// to be read via reflection and assigned to corresponding members in another
    /// object instance.
    /// </summary>
    /// <remarks>
    /// This attribute serves as a marker for reflection-based copy or synchronization
    /// processes. It does not contain additional logic or metadata.
    /// It supports multiple usages on a single element and is inherited by derived
    /// classes or overridden members.
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    internal sealed class RPInfoOutAttribute : Attribute
    { }
}