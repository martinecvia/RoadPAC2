using System;

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
    {
        // Marker attribute used to designate code for private use by the RoadPAC system.
    }

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
    {
        // Marker attribute used to designate code for internal use by the RoadPAC system.
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    internal sealed class RPInfoOutAttribute : Attribute
    { }
}