using System;

namespace JsonToObjectConverter
{
    /// <summary>
    /// Specifies the property name used to discriminate between polymorphic types.
    /// This attribute should be placed on the base class or interface.
    /// </summary>
    /// <example>
    /// <code>
    /// [JsonSerializable]
    /// [JsonTypeDiscriminator("type")]
    /// public abstract partial class Shape
    /// {
    ///     public string Type { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class JsonTypeDiscriminatorAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the property used for type discrimination.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTypeDiscriminatorAttribute"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property used for type discrimination.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="propertyName"/> is empty or whitespace.</exception>
        public JsonTypeDiscriminatorAttribute(string propertyName)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException("Property name cannot be empty or whitespace.", nameof(propertyName));
            }

            PropertyName = propertyName;
        }
    }
}
