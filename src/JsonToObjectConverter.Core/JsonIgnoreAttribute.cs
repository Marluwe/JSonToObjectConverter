using System;

namespace JsonToObjectConverter
{
    /// <summary>
    /// Excludes a property or field from JSON serialization and deserialization.
    /// </summary>
    /// <example>
    /// <code>
    /// [JsonSerializable]
    /// public partial class Person
    /// {
    ///     public string Name { get; set; }
    ///
    ///     [JsonIgnore]
    ///     public string InternalId { get; set; }
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class JsonIgnoreAttribute : Attribute
    {
    }
}
