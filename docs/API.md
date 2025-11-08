# API Documentation - JsonToObjectConverter

## Overview

This document provides detailed API documentation for the JsonToObjectConverter library.

## Core Attributes

### JsonSerializableAttribute

Marks a class or struct for JSON serialization code generation.

```csharp
namespace JsonToObjectConverter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class JsonSerializableAttribute : Attribute
{
}
```

**Example:**
```csharp
[JsonSerializable]
public partial class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}
```

---

### JsonPropertyNameAttribute

Specifies a custom JSON property name for a class member.

```csharp
namespace JsonToObjectConverter;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class JsonPropertyNameAttribute : Attribute
{
    public string Name { get; }

    public JsonPropertyNameAttribute(string name);
}
```

**Example:**
```csharp
[JsonSerializable]
public partial class Person
{
    [JsonPropertyName("full_name")]
    public string Name { get; set; }
}
```

---

### JsonIgnoreAttribute

Excludes a property or field from serialization and deserialization.

```csharp
namespace JsonToObjectConverter;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class JsonIgnoreAttribute : Attribute
{
}
```

**Example:**
```csharp
[JsonSerializable]
public partial class Person
{
    public string Name { get; set; }

    [JsonIgnore]
    public string InternalId { get; set; }
}
```

---

### JsonTypeDiscriminatorAttribute

Specifies the property name used to discriminate between polymorphic types.

```csharp
namespace JsonToObjectConverter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public sealed class JsonTypeDiscriminatorAttribute : Attribute
{
    public string PropertyName { get; }

    public JsonTypeDiscriminatorAttribute(string propertyName);
}
```

**Example:**
```csharp
[JsonSerializable]
[JsonTypeDiscriminator("type")]
public abstract partial class Shape
{
    public string Type { get; set; }
}
```

---

### JsonTypeFactoryAttribute

Registers a factory for creating instances based on discriminator values.

```csharp
namespace JsonToObjectConverter;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class JsonTypeFactoryAttribute : Attribute
{
    public string DiscriminatorValue { get; }
    public Type TargetType { get; }

    public JsonTypeFactoryAttribute(string discriminatorValue, Type targetType);
}
```

**Example:**
```csharp
[JsonSerializable]
[JsonTypeFactory("circle", typeof(Circle))]
[JsonTypeFactory("rectangle", typeof(Rectangle))]
public abstract partial class Shape
{
    // ...
}
```

---

### JsonConverterAttribute

Specifies a custom converter for a type or property.

```csharp
namespace JsonToObjectConverter;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class JsonConverterAttribute : Attribute
{
    public Type ConverterType { get; }

    public JsonConverterAttribute(Type converterType);
}
```

**Example:**
```csharp
[JsonSerializable]
public partial class Event
{
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime Timestamp { get; set; }
}
```

---

## Generated Methods

When a class is marked with `[JsonSerializable]`, the source generator creates the following methods:

### Deserialize

```csharp
public static T Deserialize(string json);
public static T Deserialize(ReadOnlySpan<char> json);
```

Deserializes a JSON string into an instance of type T.

**Parameters:**
- `json`: The JSON string to deserialize

**Returns:** A new instance of T populated from the JSON

**Throws:**
- `JsonParseException`: If the JSON is malformed
- `JsonTypeException`: If types don't match

**Example:**
```csharp
var json = "{Name: \"John\", Age: 30}";
var person = Person.Deserialize(json);
```

---

### Serialize

```csharp
public string Serialize();
public void Serialize(StringBuilder builder);
```

Serializes the current instance to a JSON string.

**Returns:** A JSON string representation

**Example:**
```csharp
var person = new Person { Name = "John", Age = 30 };
var json = person.Serialize();
// Result: {"Name":"John","Age":30}
```

---

## Extension Points (Partial Methods)

The generator creates partial methods that you can implement to customize behavior:

### OnDeserializing

Called before deserialization begins.

```csharp
partial void OnDeserializing();
```

**Example:**
```csharp
[JsonSerializable]
public partial class Person
{
    partial void OnDeserializing()
    {
        // Initialize defaults
        Age = 0;
    }
}
```

---

### OnDeserialized

Called after deserialization completes.

```csharp
partial void OnDeserialized();
```

**Example:**
```csharp
[JsonSerializable]
public partial class Person
{
    partial void OnDeserialized()
    {
        // Validate data
        if (Age < 0) throw new ArgumentException("Age must be positive");
    }
}
```

---

### OnDeserialize{PropertyName}

Called when deserializing a specific property, allowing value transformation.

```csharp
partial void OnDeserialize{PropertyName}(ref T value, JsonToken token);
```

**Parameters:**
- `value`: Reference to the deserialized value (can be modified)
- `token`: The JSON token containing the value

**Example:**
```csharp
[JsonSerializable]
public partial class Person
{
    public string Name { get; set; }

    partial void OnDeserializeName(ref string value, JsonToken token)
    {
        // Normalize to uppercase
        value = value?.ToUpperInvariant();
    }
}
```

---

### OnSerializing

Called before serialization begins.

```csharp
partial void OnSerializing();
```

---

### OnSerialized

Called after serialization completes.

```csharp
partial void OnSerialized();
```

---

## Runtime API

### JsonReader

A low-level, forward-only reader for JSON data.

```csharp
namespace JsonToObjectConverter.Runtime;

public ref struct JsonReader
{
    public JsonReader(ReadOnlySpan<char> json);

    public bool Read();
    public JsonTokenType TokenType { get; }
    public ReadOnlySpan<char> ValueSpan { get; }

    public string GetString();
    public int GetInt32();
    public long GetInt64();
    public double GetDouble();
    public bool GetBoolean();

    public void Skip();
}
```

**Example:**
```csharp
var json = "{\"name\":\"John\",\"age\":30}";
var reader = new JsonReader(json);

while (reader.Read())
{
    if (reader.TokenType == JsonTokenType.PropertyName)
    {
        var propName = reader.GetString();
        reader.Read();
        // Process value...
    }
}
```

---

### JsonToken

Represents a single token in a JSON document.

```csharp
namespace JsonToObjectConverter.Runtime;

public readonly struct JsonToken
{
    public JsonTokenType Type { get; }
    public ReadOnlySpan<char> Value { get; }
    public int Line { get; }
    public int Column { get; }
}
```

---

### JsonTokenType

Enumeration of JSON token types.

```csharp
namespace JsonToObjectConverter.Runtime;

public enum JsonTokenType
{
    None,
    ObjectStart,      // {
    ObjectEnd,        // }
    ArrayStart,       // [
    ArrayEnd,         // ]
    PropertyName,     // "key" or key
    String,           // "value"
    Number,           // 123, 45.67
    True,             // true
    False,            // false
    Null,             // null
    Colon,            // :
    Comma,            // ,
}
```

---

## Custom Converters

Implement `IJsonConverter<T>` to create custom type converters.

```csharp
namespace JsonToObjectConverter;

public interface IJsonConverter<T>
{
    T Read(ref JsonReader reader);
    void Write(StringBuilder builder, T value);
}
```

**Example:**
```csharp
public class CustomDateTimeConverter : IJsonConverter<DateTime>
{
    public DateTime Read(ref JsonReader reader)
    {
        var str = reader.GetString();
        return DateTime.ParseExact(str, "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    public void Write(StringBuilder builder, DateTime value)
    {
        builder.Append('"');
        builder.Append(value.ToString("yyyy-MM-dd"));
        builder.Append('"');
    }
}
```

---

## Exceptions

### JsonParseException

Thrown when JSON parsing fails.

```csharp
namespace JsonToObjectConverter;

public class JsonParseException : Exception
{
    public int Line { get; }
    public int Column { get; }
    public string JsonContext { get; }

    public JsonParseException(string message, int line, int column, string context);
}
```

---

### JsonTypeException

Thrown when type conversion fails.

```csharp
namespace JsonToObjectConverter;

public class JsonTypeException : Exception
{
    public Type ExpectedType { get; }
    public Type ActualType { get; }

    public JsonTypeException(string message, Type expectedType, Type actualType);
}
```

---

## Advanced Features

### Extended JSON Syntax

The parser supports the following non-standard JSON features:

#### Comments

Single-line and multi-line comments are supported:

```json
{
    // This is a single-line comment
    "name": "John",

    /* This is a
       multi-line comment */
    "age": 30
}
```

#### Unquoted Keys

Property names can be unquoted if they are valid identifiers:

```json
{
    name: "John",
    age: 30,
    home_address: {
        street: "Main St"
    }
}
```

#### Multiline Strings

String values can span multiple lines:

```json
{
    "description": "This is a
multiline
string"
}
```

---

## Performance Considerations

### Zero-Allocation Design

The library is designed to minimize allocations:
- Uses `Span<T>` and `ReadOnlySpan<T>` where possible
- Stackalloc for small buffers
- ArrayPool for larger temporary buffers

### Benchmark Results

(To be added after TASK-017)

---

## Migration Guide

### From System.Text.Json

```csharp
// System.Text.Json
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var person = JsonSerializer.Deserialize<Person>(json, options);

// JsonToObjectConverter
var person = Person.Deserialize(json); // Generated method
```

### From Newtonsoft.Json

```csharp
// Newtonsoft.Json
var person = JsonConvert.DeserializeObject<Person>(json);

// JsonToObjectConverter
var person = Person.Deserialize(json); // Generated method
```

---

## Troubleshooting

### Generator Not Running

Ensure your project references the generator correctly:

```xml
<ProjectReference Include="path/to/JsonToObjectConverter.SourceGenerator.csproj"
                  OutputItemType="Analyzer"
                  ReferenceOutputAssembly="false" />
```

### Compilation Errors

Check that:
1. The class is marked `partial`
2. The class is `public` or `internal`
3. All property types are supported

### Performance Issues

1. Use `ReadOnlySpan<char>` overload for large inputs
2. Enable release mode optimizations
3. Check for boxing of value types

---

## Examples

See the `samples/` directory for complete working examples.

---

## Version History

### 0.1.0 (Current)
- Initial release
- Basic serialization/deserialization
- Extended JSON syntax support
- Source generator implementation

---

*Last updated: 2025-01-08*
