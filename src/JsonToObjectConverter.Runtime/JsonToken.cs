using System;

namespace JsonToObjectConverter.Runtime
{
    /// <summary>
    /// Represents a single token in a JSON document.
    /// </summary>
    public readonly struct JsonToken
    {
        /// <summary>
        /// Gets the type of this token.
        /// </summary>
        public JsonTokenType Type { get; }

        /// <summary>
        /// Gets the raw value of this token as it appears in the source.
        /// </summary>
        public ReadOnlyMemory<char> Value { get; }

        /// <summary>
        /// Gets the line number where this token appears (1-based).
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the column number where this token appears (1-based).
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonToken"/> struct.
        /// </summary>
        public JsonToken(JsonTokenType type, ReadOnlyMemory<char> value, int line, int column)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Returns a string representation of this token.
        /// </summary>
        public override string ToString()
        {
            return $"{Type} at {Line}:{Column} = '{Value.ToString()}'";
        }
    }
}
