using System;

namespace JsonToObjectConverter
{
    /// <summary>
    /// Specifies a custom converter for a type or property.
    /// The converter type must implement <see cref="IJsonConverter{T}"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// [JsonSerializable]
    /// public partial class Event
    /// {
    ///     [JsonConverter(typeof(CustomDateTimeConverter))]
    ///     public DateTime Timestamp { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class JsonConverterAttribute : Attribute
    {
        /// <summary>
        /// Gets the type of the converter.
        /// </summary>
        public Type ConverterType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConverterAttribute"/> class.
        /// </summary>
        /// <param name="converterType">The type of the converter. Must implement IJsonConverter{T}.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="converterType"/> is null.</exception>
        public JsonConverterAttribute(Type converterType)
        {
            if (converterType == null)
            {
                throw new ArgumentNullException(nameof(converterType));
            }

            ConverterType = converterType;
        }
    }
}
