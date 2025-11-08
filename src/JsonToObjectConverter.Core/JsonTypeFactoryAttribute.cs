using System;

namespace JsonToObjectConverter
{
    /// <summary>
    /// Registers a factory for creating instances of a specific type based on a discriminator value.
    /// This attribute can be applied multiple times to register different concrete types.
    /// </summary>
    /// <example>
    /// <code>
    /// [JsonSerializable]
    /// [JsonTypeDiscriminator("kind")]
    /// [JsonTypeFactory("circle", typeof(Circle))]
    /// [JsonTypeFactory("rectangle", typeof(Rectangle))]
    /// public abstract partial class Shape
    /// {
    ///     public string Kind { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public sealed class JsonTypeFactoryAttribute : Attribute
    {
        /// <summary>
        /// Gets the discriminator value that maps to this type.
        /// </summary>
        public string DiscriminatorValue { get; }

        /// <summary>
        /// Gets the target type to create when the discriminator value matches.
        /// </summary>
        public Type TargetType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTypeFactoryAttribute"/> class.
        /// </summary>
        /// <param name="discriminatorValue">The discriminator value that maps to this type.</param>
        /// <param name="targetType">The target type to create when the discriminator value matches.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="discriminatorValue"/> or <paramref name="targetType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="discriminatorValue"/> is empty or whitespace.</exception>
        public JsonTypeFactoryAttribute(string discriminatorValue, Type targetType)
        {
            if (discriminatorValue == null)
            {
                throw new ArgumentNullException(nameof(discriminatorValue));
            }

            if (string.IsNullOrWhiteSpace(discriminatorValue))
            {
                throw new ArgumentException("Discriminator value cannot be empty or whitespace.", nameof(discriminatorValue));
            }

            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            DiscriminatorValue = discriminatorValue;
            TargetType = targetType;
        }
    }
}
