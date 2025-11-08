# Development Tasks - JsonToObjectConverter

Diese Datei enthält alle Tasks für die Entwicklung des JsonToObjectConverter-Projekts. Jeder Task ist so konzipiert, dass er von einem einzelnen Agenten unabhängig abgearbeitet werden kann.

## Legende
- ⚡ = Priorität: Hoch
- 🔧 = Priorität: Mittel
- 📝 = Priorität: Niedrig
- 🧪 = Muss Unit-Tests beinhalten
- 🔗 = Abhängig von anderen Tasks

---

## Phase 1: Foundation & Setup

### TASK-001: Projekt-Setup und Solution-Struktur ⚡
**Geschätzte Zeit**: 1-2 Stunden
**Abhängigkeiten**: Keine

**Beschreibung**:
Erstelle die grundlegende Projektstruktur mit allen notwendigen .NET-Projekten.

**Acceptance Criteria**:
- [ ] Solution-Datei `JsonToObjectConverter.sln` erstellt
- [ ] Folgende Projekte erstellt:
  - [ ] `src/JsonToObjectConverter.Core` (netstandard2.0)
  - [ ] `src/JsonToObjectConverter.Runtime` (netstandard2.0)
  - [ ] `src/JsonToObjectConverter.SourceGenerator` (netstandard2.0)
  - [ ] `tests/JsonToObjectConverter.Core.Tests` (net8.0)
  - [ ] `tests/JsonToObjectConverter.Runtime.Tests` (net8.0)
  - [ ] `tests/JsonToObjectConverter.SourceGenerator.Tests` (net8.0)
  - [ ] `samples/JsonToObjectConverter.Sample` (net8.0)
- [ ] Alle Test-Projekte referenzieren xUnit
- [ ] Source Generator Projekt referenziert:
  - [ ] Microsoft.CodeAnalysis.CSharp (4.8.0+)
  - [ ] Microsoft.CodeAnalysis.Analyzers (3.3.4+)
- [ ] `.editorconfig` mit C# Coding-Standards
- [ ] `Directory.Build.props` mit gemeinsamen Properties
- [ ] `Directory.Build.targets` (falls benötigt)
- [ ] Alle Projekte kompilieren ohne Fehler

**Deliverables**:
- Solution mit kompilierbarer Projektstruktur
- Build erfolgreich via `dotnet build`

---

### TASK-002: JSON-Tokenizer - Basis-Implementation 🧪⚡
**Geschätzte Zeit**: 4-6 Stunden
**Abhängigkeiten**: TASK-001

**Beschreibung**:
Implementiere einen performanten JSON-Tokenizer der Standard-JSON-Tokens erkennt (ohne erweiterte Syntax).

**Acceptance Criteria**:
- [ ] `JsonTokenizer` Klasse in `JsonToObjectConverter.Runtime`
- [ ] Unterstützt folgende Token-Typen:
  - [ ] `{` und `}` (Object Start/End)
  - [ ] `[` und `]` (Array Start/End)
  - [ ] `:` (Colon)
  - [ ] `,` (Comma)
  - [ ] String-Literale (mit Escape-Sequenzen)
  - [ ] Number (int, long, double, decimal)
  - [ ] Boolean (`true`, `false`)
  - [ ] `null`
- [ ] `Span<char>` basierte API für zero-allocation
- [ ] `JsonToken` struct mit Type, Value, Position
- [ ] Fehlerbehandlung mit Zeilen/Spalten-Information
- [ ] Unit-Tests:
  - [ ] Alle Token-Typen einzeln
  - [ ] Verschachtelte Strukturen
  - [ ] Escape-Sequenzen in Strings
  - [ ] Edge-Cases (leere Arrays, leere Objekte)
  - [ ] Fehlerhafte Syntax wird erkannt
  - [ ] Performance-Test (>1MB JSON in <100ms)

**Deliverables**:
- `JsonTokenizer.cs` mit vollständiger Implementation
- `JsonToken.cs` mit Token-Definition
- Min. 30 Unit-Tests in `JsonTokenizerTests.cs`

---

### TASK-003: JSON-Tokenizer - Erweiterte Syntax (Kommentare) 🧪🔧
**Geschätzte Zeit**: 3-4 Stunden
**Abhängigkeiten**: TASK-002

**Beschreibung**:
Erweitere den Tokenizer um Unterstützung für Kommentare (`//` und `/* */`).

**Acceptance Criteria**:
- [ ] Single-Line Kommentare (`//`) werden erkannt und übersprungen
- [ ] Multi-Line Kommentare (`/* */`) werden erkannt und übersprungen
- [ ] Kommentare in Strings werden NICHT als Kommentare behandelt
- [ ] Verschachtelte `/* */` werden korrekt behandelt (oder Fehler)
- [ ] Fehlerbehandlung für nicht geschlossene Kommentare
- [ ] Unit-Tests:
  - [ ] Single-Line Kommentare an verschiedenen Positionen
  - [ ] Multi-Line Kommentare
  - [ ] Kommentare in Strings (sollten ignoriert werden)
  - [ ] Edge-Cases (Kommentar am Ende der Datei)
  - [ ] Fehlerhafte Kommentare

**Deliverables**:
- Erweiterte `JsonTokenizer.cs`
- Min. 15 zusätzliche Unit-Tests

---

### TASK-004: JSON-Tokenizer - Unquoted Keys 🧪🔧
**Geschätzte Zeit**: 2-3 Stunden
**Abhängigkeiten**: TASK-002

**Beschreibung**:
Erweitere den Tokenizer um Unterstützung für Property-Namen ohne Anführungszeichen.

**Acceptance Criteria**:
- [ ] Property-Namen ohne Quotes werden erkannt (z.B. `Name: "value"`)
- [ ] Gültige Identifier-Regeln (C#/JavaScript-like):
  - [ ] Beginnt mit Buchstabe oder `_`
  - [ ] Enthält Buchstaben, Zahlen, `_`
- [ ] Keywords wie `true`, `false`, `null` werden NICHT als Keys akzeptiert
- [ ] Fehlerbehandlung für ungültige Identifier
- [ ] Unit-Tests:
  - [ ] Einfache unquoted keys
  - [ ] Keys mit Unterstrichen und Zahlen
  - [ ] Mixed (quoted und unquoted)
  - [ ] Fehlerhafte Keys

**Deliverables**:
- Erweiterte `JsonTokenizer.cs`
- Min. 10 zusätzliche Unit-Tests

---

### TASK-005: JSON-Tokenizer - Multiline Strings 🧪🔧
**Geschätzte Zeit**: 3-4 Stunden
**Abhängigkeiten**: TASK-002

**Beschreibung**:
Erweitere den Tokenizer um Unterstützung für mehrzeilige String-Literale.

**Acceptance Criteria**:
- [ ] String-Literale können Zeilenumbrüche enthalten
- [ ] `\n` und tatsächliche Newlines werden beide unterstützt
- [ ] Position-Tracking über mehrere Zeilen hinweg korrekt
- [ ] Fehlerbehandlung für nicht geschlossene Strings
- [ ] Unit-Tests:
  - [ ] Einfache mehrzeilige Strings
  - [ ] Strings mit gemischten Newlines
  - [ ] Edge-Cases (leere Zeilen in Strings)
  - [ ] Fehlerhafte Strings (nicht geschlossen)

**Deliverables**:
- Erweiterte `JsonTokenizer.cs`
- Min. 8 zusätzliche Unit-Tests

---

## Phase 2: Core Attributes & Parser

### TASK-006: Core Attributes Definition 🧪⚡
**Geschätzte Zeit**: 2-3 Stunden
**Abhängigkeiten**: TASK-001

**Beschreibung**:
Definiere alle Attribute die Entwickler für die Konfiguration verwenden.

**Acceptance Criteria**:
- [ ] Attribute in `JsonToObjectConverter.Core`:
  - [ ] `[JsonSerializable]` - Markiert Klasse für Codegen
  - [ ] `[JsonPropertyName(string name)]` - Custom Property-Name
  - [ ] `[JsonIgnore]` - Ignoriert Property
  - [ ] `[JsonTypeDiscriminator(string propertyName)]` - Definiert Discriminator-Property
  - [ ] `[JsonTypeFactory(string discriminatorValue, Type type)]` - Registriert Factory
  - [ ] `[JsonConverter(Type converterType)]` - Custom Converter (optional)
- [ ] Alle Attribute mit XML-Dokumentation
- [ ] AttributeUsage korrekt definiert
- [ ] Unit-Tests:
  - [ ] Attribute können auf Klassen/Properties angewendet werden
  - [ ] Reflection-basierte Tests für Attribute-Werte

**Deliverables**:
- Alle Attribute in `src/JsonToObjectConverter.Core/Attributes/`
- Min. 10 Unit-Tests

---

### TASK-007: JSON-Parser - Basis-Implementation 🧪⚡
**Geschätzte Zeit**: 6-8 Stunden
**Abhängigkeiten**: TASK-002, TASK-003, TASK-004, TASK-005

**Beschreibung**:
Implementiere einen JSON-Parser der Token vom Tokenizer verarbeitet und ein Object-Model erstellt.

**Acceptance Criteria**:
- [ ] `JsonParser` Klasse in `JsonToObjectConverter.Runtime`
- [ ] Parse zu generischem Object-Model:
  - [ ] `JsonObject` (Dictionary-like)
  - [ ] `JsonArray` (List-like)
  - [ ] Primitive Types (string, numbers, bool, null)
- [ ] Span<T> basiert für Performance
- [ ] Fehlerbehandlung mit aussagekräftigen Messages
- [ ] Unit-Tests:
  - [ ] Einfache Objekte
  - [ ] Arrays
  - [ ] Verschachtelte Strukturen
  - [ ] Alle primitiven Typen
  - [ ] Edge-Cases (leere Objekte/Arrays)
  - [ ] Fehlerhafte Strukturen
  - [ ] Performance-Tests

**Deliverables**:
- `JsonParser.cs`
- `JsonObject.cs`, `JsonArray.cs`, `JsonValue.cs`
- Min. 40 Unit-Tests

---

### TASK-008: JsonReader API 🧪🔧
**Geschätzte Zeit**: 4-5 Stunden
**Abhängigkeiten**: TASK-007

**Beschreibung**:
Implementiere eine Low-Level `JsonReader` API die der Source Generator verwenden kann (ähnlich wie `Utf8JsonReader`).

**Acceptance Criteria**:
- [ ] `JsonReader` struct (ref struct für Span<char>)
- [ ] Methoden:
  - [ ] `Read()` - Nächstes Token
  - [ ] `GetString()`, `GetInt32()`, etc.
  - [ ] `Skip()` - Überspringe aktuelles Value
  - [ ] Property-Namen lesen
- [ ] State-Machine für Validierung
- [ ] Zero-allocation Design
- [ ] Unit-Tests:
  - [ ] Alle Read-Operationen
  - [ ] State-Transitions
  - [ ] Skip-Funktionalität
  - [ ] Error-Handling

**Deliverables**:
- `JsonReader.cs`
- Min. 25 Unit-Tests

---

## Phase 3: Source Generator

### TASK-009: Source Generator - Setup & Infrastructure ⚡🔧
**Geschätzte Zeit**: 4-6 Stunden
**Abhängigkeiten**: TASK-001, TASK-006

**Beschreibung**:
Erstelle die Basis-Infrastruktur für den Roslyn Source Generator.

**Acceptance Criteria**:
- [ ] `JsonSourceGenerator : IIncrementalGenerator`
- [ ] Syntax Receiver für `[JsonSerializable]` Attribute
- [ ] Semantic Model Analyse
- [ ] Code-Generierungs-Pipeline
- [ ] Diagnostics/Error Reporting
- [ ] Generator kann in Test-Projekt verwendet werden
- [ ] Unit-Tests:
  - [ ] Generator findet markierte Klassen
  - [ ] Generator ignoriert nicht-markierte Klassen
  - [ ] Error-Diagnostics werden generiert

**Deliverables**:
- `JsonSourceGenerator.cs`
- Generator-Test-Infrastructure
- Min. 10 Unit-Tests

---

### TASK-010: Source Generator - Einfache Klassen 🧪⚡🔗
**Geschätzte Zeit**: 8-10 Stunden
**Abhängigkeiten**: TASK-009

**Beschreibung**:
Generiere Deserializer-Code für Klassen mit primitiven Properties.

**Acceptance Criteria**:
- [ ] Generiert `partial class` Erweiterung mit `Deserialize(string json)` Methode
- [ ] Unterstützt primitive Typen:
  - [ ] string, int, long, float, double, decimal
  - [ ] bool
  - [ ] DateTime, DateTimeOffset, Guid
- [ ] Property-Mapping (Case-Insensitive Standard)
- [ ] Nutzt `JsonReader` API
- [ ] Generated Code:
  - [ ] Ist lesbar und debuggable
  - [ ] Enthält Kommentare
  - [ ] Nullable Reference Types Support
- [ ] Unit-Tests:
  - [ ] Verschiedene Klassen-Typen
  - [ ] Alle primitiven Typen
  - [ ] Optional/Required Properties
  - [ ] Generierter Code kompiliert
  - [ ] End-to-End: Deserialisierung funktioniert

**Deliverables**:
- Code-Generator für einfache Klassen
- Template-System für Code-Generation
- Min. 30 Unit-Tests

---

### TASK-011: Source Generator - Verschachtelte Objekte 🧪🔧🔗
**Geschätzte Zeit**: 6-8 Stunden
**Abhängigkeiten**: TASK-010

**Beschreibung**:
Erweitere Generator für verschachtelte Objekte und Collections.

**Acceptance Criteria**:
- [ ] Unterstützt verschachtelte Klassen
- [ ] Unterstützt Collections:
  - [ ] `List<T>`, `T[]`
  - [ ] `Dictionary<string, T>`
  - [ ] `IEnumerable<T>`, `ICollection<T>`, `IList<T>`
- [ ] Recursive Deserialisierung
- [ ] Circular-Reference Detection (optional)
- [ ] Unit-Tests:
  - [ ] Verschachtelte Objekte (mehrere Ebenen)
  - [ ] Arrays von Objekten
  - [ ] Dictionaries
  - [ ] Mixed Collections

**Deliverables**:
- Erweiterte Code-Generation
- Min. 20 Unit-Tests

---

### TASK-012: Source Generator - Partial Methods 🧪🔧🔗
**Geschätzte Zeit**: 5-6 Stunden
**Abhängigkeiten**: TASK-010

**Beschreibung**:
Generiere partial methods die Entwickler überschreiben können.

**Acceptance Criteria**:
- [ ] Generiert partial methods:
  - [ ] `partial void OnDeserialize{PropertyName}(ref T value, JsonToken token)`
  - [ ] `partial void OnDeserialized()`
  - [ ] `partial void OnDeserializing()`
- [ ] Hooks werden an richtigen Stellen aufgerufen
- [ ] Null-Pattern (Methoden sind optional)
- [ ] Unit-Tests:
  - [ ] Hooks werden aufgerufen
  - [ ] Hooks können Werte modifizieren
  - [ ] Hooks können Exceptions werfen

**Deliverables**:
- Erweiterte Code-Generation mit Hooks
- Min. 15 Unit-Tests

---

### TASK-013: Source Generator - Serialisierung 🧪🔧🔗
**Geschätzte Zeit**: 6-8 Stunden
**Abhängigkeiten**: TASK-010

**Beschreibung**:
Generiere Serialisierungs-Code (`Serialize()` Methode).

**Acceptance Criteria**:
- [ ] Generiert `Serialize()` Methode
- [ ] Unterstützt alle Typen aus TASK-010 und TASK-011
- [ ] Span<T> basiert für Performance
- [ ] StringBuilder oder ArrayPool für Buffer
- [ ] Pretty-Print Option (optional)
- [ ] Unit-Tests:
  - [ ] Roundtrip-Tests (Deserialize -> Serialize -> Deserialize)
  - [ ] Alle Typen
  - [ ] Edge-Cases (null, leere Collections)

**Deliverables**:
- Serialisierungs-Code-Generation
- Min. 25 Unit-Tests

---

## Phase 4: Advanced Features

### TASK-014: Factory Pattern - Infrastructure 🧪⚡🔗
**Geschätzte Zeit**: 6-8 Stunden
**Abhängigkeiten**: TASK-010, TASK-006

**Beschreibung**:
Implementiere Infrastructure für Factory-Pattern und Type-Discriminator.

**Acceptance Criteria**:
- [ ] `IJsonTypeFactory` Interface
- [ ] `JsonTypeRegistry` für Factory-Registration
- [ ] Generierter Code registriert Factories automatisch
- [ ] Discriminator-Property wird zuerst gelesen
- [ ] Factory wird basierend auf Discriminator-Wert aufgerufen
- [ ] Fehlerbehandlung für unbekannte Typen
- [ ] Unit-Tests:
  - [ ] Factory-Registration
  - [ ] Type-Resolution
  - [ ] Fehlerhafte Discriminators

**Deliverables**:
- Factory-Infrastructure
- Min. 20 Unit-Tests

---

### TASK-015: Factory Pattern - Code Generation 🧪🔧🔗
**Geschätzte Zeit**: 8-10 Stunden
**Abhängigkeiten**: TASK-014

**Beschreibung**:
Generiere Code für polymorphe Typen mit Factory-Pattern.

**Acceptance Criteria**:
- [ ] Erkennt `[JsonTypeDiscriminator]` auf Basis-Klasse
- [ ] Sammelt alle `[JsonTypeFactory]` Attribute
- [ ] Generiert Factory-Registry
- [ ] Generiert Deserializer der Registry verwendet
- [ ] Unterstützt Vererbungshierarchien
- [ ] Unit-Tests:
  - [ ] Einfache Polymorphie
  - [ ] Mehrere Ebenen
  - [ ] End-to-End Tests

**Deliverables**:
- Factory Code-Generation
- Min. 25 Unit-Tests

---

### TASK-016: Custom Converters 🧪📝🔗
**Geschätzte Zeit**: 6-8 Stunden
**Abhängigkeiten**: TASK-010

**Beschreibung**:
Implementiere Support für custom Type-Converter.

**Acceptance Criteria**:
- [ ] `IJsonConverter<T>` Interface
- [ ] `[JsonConverter(typeof(MyConverter))]` Attribut-Support
- [ ] Generierter Code verwendet Converter
- [ ] Built-in Converter für gängige Typen:
  - [ ] DateTime (verschiedene Formate)
  - [ ] Enum (string und numeric)
  - [ ] Guid
- [ ] Unit-Tests:
  - [ ] Custom Converters
  - [ ] Built-in Converters
  - [ ] Edge-Cases

**Deliverables**:
- Converter Infrastructure
- Built-in Converters
- Min. 20 Unit-Tests

---

## Phase 5: Performance & Polish

### TASK-017: Performance Optimierung 🧪🔧
**Geschätzte Zeit**: 8-12 Stunden
**Abhängigketies**: TASK-010, TASK-011, TASK-013

**Beschreibung**:
Optimiere Performance für Production-Workloads.

**Acceptance Criteria**:
- [ ] Benchmarks erstellt (BenchmarkDotNet):
  - [ ] vs System.Text.Json
  - [ ] vs Newtonsoft.Json
  - [ ] Verschiedene Payload-Größen
- [ ] Zero-allocation für Standard-Cases
- [ ] ArrayPool für große Strings
- [ ] Span<T> optimiert
- [ ] Generated Code Optimierungen:
  - [ ] Inlining-Hints
  - [ ] Branch-Prediction friendly
- [ ] Performance-Ziele:
  - [ ] Min. 80% der Geschwindigkeit von System.Text.Json
  - [ ] Max. 10% Overhead für erweiterte Syntax
- [ ] Dokumentierte Performance-Charakteristiken

**Deliverables**:
- Benchmark-Suite
- Performance-Optimierungen
- Performance-Dokumentation

---

### TASK-018: Error Handling & Diagnostics 🧪🔧
**Geschätzte Zeit**: 5-6 Stunden
**Abhängigkeiten**: TASK-007, TASK-009

**Beschreibung**:
Verbessere Error-Handling und Diagnostics.

**Acceptance Criteria**:
- [ ] Aussagekräftige Fehlermeldungen mit:
  - [ ] Zeile und Spalte
  - [ ] Kontext (umgebender Code)
  - [ ] Vorschläge zur Behebung
- [ ] Source Generator Diagnostics:
  - [ ] Warnings für suspicious Patterns
  - [ ] Errors für ungültige Konfigurationen
- [ ] Exception-Typen:
  - [ ] `JsonParseException`
  - [ ] `JsonTypeException`
  - [ ] Custom Exception mit allen Infos
- [ ] Unit-Tests:
  - [ ] Verschiedene Error-Szenarien
  - [ ] Diagnostics werden korrekt generiert

**Deliverables**:
- Verbessertes Error-Handling
- Custom Exceptions
- Min. 20 Unit-Tests

---

### TASK-019: NuGet Package Configuration ⚡
**Geschätzte Zeit**: 3-4 Stunden
**Abhängigkeiten**: Alle vorherigen Tasks

**Beschreibung**:
Konfiguriere NuGet-Package für Publishing.

**Acceptance Criteria**:
- [ ] `.nuspec` oder Project-Properties konfiguriert:
  - [ ] Package-ID: `JsonToObjectConverter`
  - [ ] Versioning (SemVer)
  - [ ] Authors, Description, Tags
  - [ ] License (MIT)
  - [ ] Repository-URL
  - [ ] Icon
- [ ] Multi-Targeting (netstandard2.0, net6.0, net8.0)
- [ ] Source Generator als Analyzer-Package
- [ ] Dependencies korrekt deklariert
- [ ] README.md included im Package
- [ ] Package-Validierung:
  - [ ] Lokaler Build funktioniert
  - [ ] Package kann installiert werden
  - [ ] Generated Code funktioniert in Consumer-Projekt

**Deliverables**:
- NuGet-Package Configuration
- Package-Build-Skript

---

### TASK-020: Documentation & Samples 📝
**Geschätzte Zeit**: 6-8 Stunden
**Abhängigkeiten**: Alle Feature-Tasks

**Beschreibung**:
Erstelle umfassende Dokumentation und Beispiele.

**Acceptance Criteria**:
- [ ] API-Dokumentation (`docs/API.md`):
  - [ ] Alle Attribute
  - [ ] Alle generierten Methoden
  - [ ] Extension Points
- [ ] Tutorials:
  - [ ] Getting Started
  - [ ] Advanced Features
  - [ ] Factory Pattern Guide
  - [ ] Performance Tips
- [ ] Sample-Projekt:
  - [ ] Einfache Beispiele
  - [ ] Komplexe Szenarien
  - [ ] Best Practices
- [ ] XML-Dokumentation für alle public APIs
- [ ] GitHub Wiki (optional)

**Deliverables**:
- Vollständige Dokumentation
- Funktionierendes Sample-Projekt

---

### TASK-021: Integration Tests 🧪🔧
**Geschätzte Zeit**: 6-8 Stunden
**Abhängigkeiten**: Alle Feature-Tasks

**Beschreibung**:
Erstelle umfassende Integration-Tests die reale Szenarien abdecken.

**Acceptance Criteria**:
- [ ] End-to-End Tests:
  - [ ] Realistische Datenmodelle
  - [ ] Komplexe Verschachtelungen
  - [ ] Alle Features kombiniert
- [ ] Test-Szenarien:
  - [ ] API-Responses
  - [ ] Configuration-Files
  - [ ] Data-Transfer-Objects
- [ ] Performance-Tests unter Last
- [ ] Memory-Leak Tests
- [ ] Min. 95% Code-Coverage gesamt

**Deliverables**:
- Integration-Test-Suite
- Coverage-Report

---

## Phase 6: Optional Enhancements

### TASK-022: Async Support 📝🔗
**Geschätzte Zeit**: 4-6 Stunden
**Abhängigkeiten**: TASK-013

**Beschreibung**:
Füge async Deserialisierung für Streams hinzu.

**Acceptance Criteria**:
- [ ] `DeserializeAsync(Stream stream)` Methode
- [ ] `SerializeAsync(Stream stream)` Methode
- [ ] Streaming-Parser für große Dateien
- [ ] CancellationToken Support
- [ ] Unit-Tests

**Deliverables**:
- Async API
- Min. 15 Unit-Tests

---

### TASK-023: Schema Validation 📝🔗
**Geschätzte Zeit**: 8-10 Stunden
**Abhängigkeiten**: TASK-010

**Beschreibung**:
Optionale Schema-Validierung zur Compile-Zeit.

**Acceptance Criteria**:
- [ ] `[JsonRequired]` Attribut
- [ ] `[JsonValidation]` Attribut mit Regex, Range, etc.
- [ ] Source Generator validiert zur Compile-Zeit (wo möglich)
- [ ] Runtime-Validierung
- [ ] Unit-Tests

**Deliverables**:
- Validation Infrastructure
- Min. 20 Unit-Tests

---

### TASK-024: JSON Schema Export 📝🔗
**Geschätzte Zeit**: 6-8 Stunden
**Abhängigkeiten**: TASK-010

**Beschreibung**:
Generiere JSON Schema aus C# Klassen.

**Acceptance Criteria**:
- [ ] Export zu JSON Schema (Draft 7+)
- [ ] Berücksichtigt alle Attribute
- [ ] CLI-Tool oder MSBuild-Task
- [ ] Unit-Tests

**Deliverables**:
- Schema-Export-Tool
- Min. 10 Unit-Tests

---

## Task-Dependencies Übersicht

```
TASK-001 (Setup)
├── TASK-002 (Tokenizer Basis) ⚡
│   ├── TASK-003 (Comments) 🔧
│   ├── TASK-004 (Unquoted Keys) 🔧
│   ├── TASK-005 (Multiline Strings) 🔧
│   └── TASK-007 (Parser) ⚡
│       └── TASK-008 (JsonReader) 🔧
│
├── TASK-006 (Attributes) ⚡
│   └── TASK-009 (Generator Setup) ⚡
│       ├── TASK-010 (Simple Classes) ⚡ [+TASK-008]
│       │   ├── TASK-011 (Nested Objects) 🔧
│       │   ├── TASK-012 (Partial Methods) 🔧
│       │   ├── TASK-013 (Serialization) 🔧
│       │   ├── TASK-014 (Factory Infrastructure) ⚡
│       │   │   └── TASK-015 (Factory Codegen) 🔧
│       │   └── TASK-016 (Custom Converters) 📝
│       └── TASK-017 (Performance) 🔧
│
└── [When all above done]
    ├── TASK-018 (Error Handling) 🔧
    ├── TASK-019 (NuGet Package) ⚡
    ├── TASK-020 (Documentation) 📝
    └── TASK-021 (Integration Tests) 🧪

Optional:
├── TASK-022 (Async) 📝
├── TASK-023 (Validation) 📝
└── TASK-024 (Schema Export) 📝
```

---

## Arbeitsweise für Agenten

### Vor dem Start eines Tasks:
1. Lese die Task-Beschreibung vollständig
2. Prüfe alle Abhängigkeiten (sind diese Tasks abgeschlossen?)
3. Setze Task-Status auf "In Progress"
4. Erstelle einen Branch (falls nicht vorhanden): `feature/task-XXX`

### Während der Arbeit:
1. Schreibe IMMER zuerst die Tests (TDD)
2. Implementiere dann die Funktionalität
3. Stelle sicher dass alle Tests grün sind
4. Code-Review der eigenen Arbeit
5. Dokumentiere alle public APIs

### Nach Abschluss:
1. Alle Acceptance Criteria erfüllt?
2. Alle Tests grün? (`dotnet test`)
3. Code kompiliert ohne Warnings? (`dotnet build`)
4. Commit mit aussagekräftiger Message
5. Setze Task-Status auf "Completed"

### Commit-Message Format:
```
[TASK-XXX] Kurze Beschreibung

- Detail 1
- Detail 2

Tests: XX tests added/modified
Coverage: XX%
```

---

## Test-Coverage Anforderungen

Jedes Projekt muss mindestens folgende Coverage haben:
- **Core**: >90% (meist Attribute, einfach zu testen)
- **Runtime**: >85% (Tokenizer, Parser)
- **SourceGenerator**: >80% (komplexer zu testen)
- **Gesamt**: >85%

---

## Hilfreiche Ressourcen

### Source Generators:
- https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview
- https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md

### Performance:
- https://learn.microsoft.com/en-us/dotnet/standard/memory-and-spans/
- https://benchmarkdotnet.org/

### Testing:
- https://xunit.net/
- https://github.com/AutoFixture/AutoFixture

---

## Fragen & Support

Bei Fragen zu einem Task:
1. Prüfe die Acceptance Criteria
2. Schaue in ähnliche Tasks
3. Erstelle ein Issue mit Tag `question:task-XXX`

Viel Erfolg! 🚀
