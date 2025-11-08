using System;

namespace JsonToObjectConverter
{
    /// <summary>
    /// Marks a class or struct for JSON serialization code generation.
    /// The type must be declared as partial to allow the source generator to add methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class JsonSerializableAttribute : Attribute
    {
    }
}
