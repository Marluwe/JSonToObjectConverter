using System;

namespace JsonToObjectConverter.Runtime
{
    /// <summary>
    /// Exception thrown when JSON parsing fails.
    /// </summary>
    public class JsonParseException : Exception
    {
        /// <summary>
        /// Gets the line number where the error occurred (1-based).
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the column number where the error occurred (1-based).
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Gets the context around the error position.
        /// </summary>
        public string? JsonContext { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonParseException"/> class.
        /// </summary>
        public JsonParseException(string message, int line, int column, string? context = null)
            : base(FormatMessage(message, line, column, context))
        {
            Line = line;
            Column = column;
            JsonContext = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonParseException"/> class.
        /// </summary>
        public JsonParseException(string message, int line, int column, Exception innerException)
            : base(FormatMessage(message, line, column, null), innerException)
        {
            Line = line;
            Column = column;
        }

        private static string FormatMessage(string message, int line, int column, string? context)
        {
            var formatted = $"{message} at line {line}, column {column}";
            if (!string.IsNullOrEmpty(context))
            {
                formatted += $"\nContext: {context}";
            }
            return formatted;
        }
    }
}
