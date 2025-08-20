using System;
using Shared.Controllers.Models.RibbonXml;

namespace Shared.Controllers.Controls
{
    /// <summary>
    /// Base class for all Ribbon control wrappers.
    /// Provides a strongly-typed reference to the underlying Ribbon item
    /// and stores its associated <c>Id</c> for identification.
    /// </summary>
    /// <typeparam name="RibbonRef">The type of the underlying Ribbon item (e.g., RibbonLabel, RibbonButton).</typeparam>
    public abstract class BaseRibbonControl<RibbonRef>
        where RibbonRef : class, new()
    {
        /// <summary>
        /// Gets the strongly-typed reference to the underlying Ribbon item.
        /// </summary>
        public RibbonRef Target { get; }
        public BaseRibbonXml Source { get; }

        /// <summary>
        /// Gets the Id of the Ribbon control.
        /// This corresponds to the XML definition's <c>Id</c> attribute.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRibbonControl{RibbonRef}"/> class.
        /// </summary>
        /// <param name="id">
        /// The Id of the Ribbon control. Must not be null or empty.
        /// Typically comes from the XML definition and is used for dynamic lookup.
        /// </param>
        /// <param name="target">
        /// A reference to the underlying Ribbon item instance.
        /// Must not be null.
        /// </param>
        /// <param name="source">
        /// A reference to the underlying RibbonXml item instance.
        /// Must not be null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="id"/> is null or empty, or if <paramref name="target"/> is null.
        /// </exception>
        public BaseRibbonControl(string id, RibbonRef target, BaseRibbonXml source)
        { 
            // Id can't be null or empty, since it was created in Transform method with valid Id
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "Ribbon control Id cannot be null or empty.");
            Target = target ?? throw new ArgumentNullException(nameof(target), 
                "Ribbon item reference cannot be null.");
            Source = source;
            Id = id;
        }
    }
}