using System;

namespace JsonToObjectConverter
{
    /// <summary>
    /// Specifies a custom JSON property name for a class member.
    /// </summary>
    /// <example>
    /// <code>
    /// [JsonSerializable]
    /// public partial class Person
    /// {
    ///     [JsonPropertyName("full_name")]
    ///     public string Name { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class JsonPropertyNameAttribute : Attribute
    {
        /// <summary>
        /// Gets the custom name for the JSON property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPropertyNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The custom name for the JSON property.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is empty or whitespace.</exception>
        public JsonPropertyNameAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Property name cannot be empty or whitespace.", nameof(name));
            }

            Name = name;
        }
    }
}
