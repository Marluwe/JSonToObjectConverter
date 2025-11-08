# JsonToObjectConverter

Ein hochperformantes, dependency-freies NuGet-Package für .NET, das Source Generators verwendet, um JSON-(De-)Serialisierung zur Compile-Zeit zu generieren.

## Features

### Kernfunktionalität
- **Zero-Dependency**: Keine externen Abhängigkeiten zur Laufzeit - nur die Runtime-Komponenten (Parser/Tokenizer) werden benötigt
  - Attribute werden vom Source Generator im Consumer-Code generiert (keine Core-Abhängigkeit!)
  - Source Generator ist nur zur Compile-Zeit aktiv (keine Runtime-Abhängigkeit)
- **Source Generator basiert**: Generiert Deserializer zur Compile-Zeit für maximale Performance
- **Type-Safe**: Vollständig typsicher durch Code-Generierung
- **Erweiterbar**: Entwickler können generierte Methoden überschreiben (partial classes)

### JSON-Parser Features
- **Erweiterte JSON-Syntax**:
  - Kommentare (`//` und `/* */`)
  - Schlüssel ohne Anführungszeichen
  - Mehrzeilige String-Literale
- **Factory-Pattern Support**: Dynamische Typ-Auflösung basierend auf JSON-Properties
- **Type-Discriminator**: Automatisches Scanning von JSON-Blöcken nach Typ-Informationen

## Projektstruktur

```
JsonToObjectConverter/
├── src/
│   ├── JsonToObjectConverter.Core/          # Core-Bibliothek mit Attributen und Basisklassen
│   ├── JsonToObjectConverter.SourceGenerator/ # Roslyn Source Generator
│   └── JsonToObjectConverter.Runtime/        # Runtime-Komponenten (Parser, etc.)
├── tests/
│   ├── JsonToObjectConverter.Core.Tests/
│   ├── JsonToObjectConverter.SourceGenerator.Tests/
│   └── JsonToObjectConverter.Runtime.Tests/
├── samples/
│   └── JsonToObjectConverter.Sample/         # Beispiel-Anwendungen
└── docs/
    └── API.md                                 # API-Dokumentation
```

## Schnellstart

### Installation

```bash
dotnet add package JsonToObjectConverter
```

### Dependency-Architektur (Zero-Dependency!)

Das Projekt verwendet eine innovative Architektur, um **echte Zero-Dependency** zu erreichen:

**Zur Compile-Zeit:**
- Der Source Generator analysiert Ihren Code und findet alle `[JsonSerializable]`-Attribute
- Der Generator **erzeugt die Attribute selbst** in Ihrem Code (keine Core-Library nötig!)
- Der Generator erstellt die Deserialize/Serialize-Methoden für Ihre Klassen

**Zur Laufzeit:**
- **Nur** die Runtime-Komponenten sind erforderlich: `JsonReader`, `JsonTokenizer`, `JsonToken`
- Keine Abhängigkeit zur Core-Library (Attribute sind bereits generiert!)
- Keine Abhängigkeit zum Source Generator (nur Compile-Zeit!)

**Beispiel .csproj:**
```xml
<ItemGroup>
  <!-- Runtime-Komponenten - die einzige Laufzeit-Abhängigkeit! -->
  <ProjectReference Include="JsonToObjectConverter.Runtime" />

  <!-- Source Generator - NUR Compile-Zeit, keine Runtime-Abhängigkeit! -->
  <ProjectReference Include="JsonToObjectConverter.SourceGenerator"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

### Basis-Verwendung

```csharp
using JsonToObjectConverter;

[JsonSerializable]
public partial class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public Address HomeAddress { get; set; }
}

[JsonSerializable]
public partial class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

// Verwendung
var json = @"{
    Name: ""John Doe"",  // Schlüssel ohne Anführungszeichen
    Age: 30,
    HomeAddress: {
        Street: ""Main Street 123"",
        City: ""New York""
    }
}";

var person = Person.Deserialize(json);
var serialized = person.Serialize();
```

### Factory-Pattern für polymorphe Typen

```csharp
[JsonSerializable]
[JsonTypeDiscriminator("Kind")] // Liest "Kind"-Property für Typ-Auflösung
public abstract partial class Shape
{
    public string Kind { get; set; }
}

[JsonTypeFactory("Circle", typeof(Circle))]
public partial class Circle : Shape
{
    public double Radius { get; set; }
}

[JsonTypeFactory("Rectangle", typeof(Rectangle))]
public partial class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }
}

// JSON mit Type-Discriminator
var json = @"{
    Kind: ""Circle"",
    Radius: 5.0
}";

var shape = Shape.Deserialize(json); // Gibt Circle-Instanz zurück
```

### Methoden überschreiben

```csharp
[JsonSerializable]
public partial class CustomPerson
{
    public string Name { get; set; }
    public int Age { get; set; }

    // Überschreibe generierte Deserialisierungs-Logik für Name
    partial void OnDeserializeName(ref string value, JsonToken token)
    {
        value = value?.ToUpperInvariant();
    }

    // Custom Validierung nach Deserialisierung
    partial void OnDeserialized()
    {
        if (Age < 0) throw new ArgumentException("Age must be positive");
    }
}
```

### Erweiterte JSON-Syntax

```csharp
var json = @"
{
    // Dies ist ein Kommentar
    Name: ""John"",  // Inline-Kommentar

    /* Multi-line
       Comment */

    Description: ""Dies ist ein
mehrzeiliger
String"",

    Age: 30
}";

var person = Person.Deserialize(json);
```

## Architektur

### 1. Source Generator (Compile-Zeit)
Der Source Generator ist das Herzstück und arbeitet zur Compile-Zeit:

**Generiert Attribute in Ihrem Code:**
- `[JsonSerializable]`: Markiert Klassen für Code-Generierung
- `[JsonTypeDiscriminator]`: Definiert Property für Typ-Auflösung
- `[JsonTypeFactory]`: Registriert Factory für polymorphe Typen
- `[JsonPropertyName]`: Custom Property-Namen im JSON
- `[JsonIgnore]`: Ignoriert Properties bei (De-)Serialisierung

**Generiert Code zur Compile-Zeit:**
- Deserializer-Methoden für jede public Property
- Serializer-Methoden
- Partial Methods für Erweiterungspunkte
- Factory-Registrierung und Type-Resolution

### 2. Runtime (Laufzeit - einzige Abhängigkeit!)
Die Runtime-Komponenten sind die **einzige Laufzeit-Abhängigkeit**:
- JSON-Tokenizer (unterstützt erweiterte Syntax)
- JSON-Parser (zero-allocation wo möglich)
- JsonReader (forward-only Reader für generierten Code)
- Buffer-Management
- Error-Handling

### 3. Core-Bibliothek (Optional - nur für Entwicklung)
Die Core-Bibliothek wird **nicht** zur Laufzeit benötigt!
- Wird nur für die Entwicklung des Source Generators verwendet
- Consumer-Code benötigt keine Referenz darauf
- Alle Attribute werden vom Generator im Consumer-Code erzeugt

## Design-Prinzipien

1. **Performance First**: Zero-allocation Parser, Span<T> basiert
2. **Developer Experience**: Intuitive API, gute Fehlermeldungen
3. **Flexibilität**: Erweiterbar durch partial methods und Factories
4. **Sicherheit**: Type-safe, keine Reflection zur Laufzeit
5. **Testbarkeit**: 100% Unit-Test Coverage

## Entwicklungs-Roadmap

Siehe [TASKS.md](TASKS.md) für detaillierte Aufgabenbeschreibungen.

### Phase 1: Foundation ✅ COMPLETED
- [x] Projekt-Setup und Struktur
- [x] JSON-Tokenizer mit erweiterter Syntax (Kommentare, unquoted keys, multiline strings)
- [x] JSON-Parser und JsonReader API

### Phase 2: Source Generator ✅ WORKING!
- [x] Roslyn Source Generator Setup
- [x] Code-Generierung für einfache Typen (string, int, long, double, decimal, bool)
- [x] Property-Mapping und Deserialisierung
- [x] Serialisierung für primitive Typen
- [ ] Verschachtelte Objekte und Collections

### Phase 3: Advanced Features (In Progress)
- [ ] Factory-Pattern Support
- [ ] Type-Discriminator
- [ ] Partial Methods für Erweiterungen
- [ ] Custom Converters

### Phase 4: Polish
- [ ] Performance-Optimierung
- [ ] Dokumentation
- [ ] NuGet-Package Publishing

## Aktueller Status

✅ **WORKING END-TO-END!** Das Projekt hat einen funktionierenden Source Generator, der:
- Klassen mit `[JsonSerializable]` automatisch findet
- `Deserialize(string json)` und `Serialize()` Methoden generiert
- Erweiterte JSON-Syntax unterstützt (Kommentare, unquoted keys)
- Primitive Typen vollständig unterstützt
- 150+ Unit-Tests (>90% Coverage)

Testen Sie es selbst mit dem Sample-Projekt in `samples/JsonToObjectConverter.Sample`!

## Lizenz

MIT License

## Contributing

Contributions sind willkommen! Bitte beachten Sie die einzelnen Tasks in [TASKS.md](TASKS.md).

## Support

Bei Fragen oder Problemen erstellen Sie bitte ein Issue im GitHub Repository