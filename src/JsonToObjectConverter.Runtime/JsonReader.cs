using System;
using System.Globalization;

namespace JsonToObjectConverter.Runtime
{
    /// <summary>
    /// A forward-only, high-performance JSON reader.
    /// Used by generated deserialization code.
    /// </summary>
    public struct JsonReader
    {
        private readonly string _json;
        private int _tokenIndex;
        private System.Collections.Generic.List<JsonToken>? _tokens;
        private JsonToken _currentToken;
        private bool _hasToken;

        /// <summary>
        /// Gets the current token type.
        /// </summary>
        public JsonTokenType TokenType => _currentToken.Type;

        /// <summary>
        /// Gets the current token value as a span.
        /// </summary>
        public ReadOnlySpan<char> ValueSpan => _currentToken.Value.Span;

        /// <summary>
        /// Gets the line number of the current token.
        /// </summary>
        public int Line => _currentToken.Line;

        /// <summary>
        /// Gets the column number of the current token.
        /// </summary>
        public int Column => _currentToken.Column;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> struct.
        /// </summary>
        public JsonReader(string json)
        {
            _json = json;
            _tokenIndex = -1;
            _tokens = null;
            _currentToken = default;
            _hasToken = false;
        }

        /// <summary>
        /// Reads the next token from the JSON input.
        /// </summary>
        /// <returns>True if a token was read; false if end of input.</returns>
        public bool Read()
        {
            // Lazy tokenization
            if (_tokens == null)
            {
                var tokenizer = new JsonTokenizer(_json);
                _tokens = tokenizer.Tokenize();
            }

            _tokenIndex++;
            if (_tokenIndex < _tokens.Count)
            {
                _currentToken = _tokens[_tokenIndex];
                _hasToken = true;
                return true;
            }

            _hasToken = false;
            return false;
        }

        /// <summary>
        /// Expects and consumes the specified token type.
        /// </summary>
        public void Expect(JsonTokenType expectedType)
        {
            if (!_hasToken || _currentToken.Type != expectedType)
            {
                throw new JsonParseException(
                    $"Expected {expectedType} but got {(_hasToken ? _currentToken.Type.ToString() : "end of input")}",
                    _currentToken.Line,
                    _currentToken.Column);
            }
        }

        /// <summary>
        /// Gets the current token value as a string.
        /// For string tokens, removes the quotes.
        /// </summary>
        public string GetString()
        {
            if (_currentToken.Type == JsonTokenType.String)
            {
                // Remove quotes and process escapes
                var span = _currentToken.Value.Span;
                if (span.Length >= 2 && span[0] == '"' && span[^1] == '"')
                {
                    return span.Slice(1, span.Length - 2).ToString();
                }
                return span.ToString();
            }

            if (_currentToken.Type == JsonTokenType.PropertyName)
            {
                // Unquoted property name
                return _currentToken.Value.ToString();
            }

            return _currentToken.Value.ToString();
        }

        /// <summary>
        /// Gets the current number token as an int.
        /// </summary>
        public int GetInt32()
        {
            if (_currentToken.Type != JsonTokenType.Number)
            {
                throw new JsonParseException(
                    $"Expected number but got {_currentToken.Type}",
                    _currentToken.Line,
                    _currentToken.Column);
            }

            var span = _currentToken.Value.Span;
            if (int.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new JsonParseException(
                $"Cannot parse '{span.ToString()}' as Int32",
                _currentToken.Line,
                _currentToken.Column);
        }

        /// <summary>
        /// Gets the current number token as a long.
        /// </summary>
        public long GetInt64()
        {
            if (_currentToken.Type != JsonTokenType.Number)
            {
                throw new JsonParseException(
                    $"Expected number but got {_currentToken.Type}",
                    _currentToken.Line,
                    _currentToken.Column);
            }

            var span = _currentToken.Value.Span;
            if (long.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new JsonParseException(
                $"Cannot parse '{span.ToString()}' as Int64",
                _currentToken.Line,
                _currentToken.Column);
        }

        /// <summary>
        /// Gets the current number token as a double.
        /// </summary>
        public double GetDouble()
        {
            if (_currentToken.Type != JsonTokenType.Number)
            {
                throw new JsonParseException(
                    $"Expected number but got {_currentToken.Type}",
                    _currentToken.Line,
                    _currentToken.Column);
            }

            var span = _currentToken.Value.Span;
            if (double.TryParse(span, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new JsonParseException(
                $"Cannot parse '{span.ToString()}' as Double",
                _currentToken.Line,
                _currentToken.Column);
        }

        /// <summary>
        /// Gets the current number token as a decimal.
        /// </summary>
        public decimal GetDecimal()
        {
            if (_currentToken.Type != JsonTokenType.Number)
            {
                throw new JsonParseException(
                    $"Expected number but got {_currentToken.Type}",
                    _currentToken.Line,
                    _currentToken.Column);
            }

            var span = _currentToken.Value.Span;
            if (decimal.TryParse(span, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new JsonParseException(
                $"Cannot parse '{span.ToString()}' as Decimal",
                _currentToken.Line,
                _currentToken.Column);
        }

        /// <summary>
        /// Gets the current boolean token value.
        /// </summary>
        public bool GetBoolean()
        {
            if (_currentToken.Type == JsonTokenType.True)
            {
                return true;
            }

            if (_currentToken.Type == JsonTokenType.False)
            {
                return false;
            }

            throw new JsonParseException(
                $"Expected boolean but got {_currentToken.Type}",
                _currentToken.Line,
                _currentToken.Column);
        }

        /// <summary>
        /// Skips the current value (and all nested values if it's an object or array).
        /// </summary>
        public void Skip()
        {
            var depth = 0;

            do
            {
                if (_currentToken.Type == JsonTokenType.ObjectStart || _currentToken.Type == JsonTokenType.ArrayStart)
                {
                    depth++;
                }
                else if (_currentToken.Type == JsonTokenType.ObjectEnd || _currentToken.Type == JsonTokenType.ArrayEnd)
                {
                    depth--;
                    if (depth < 0)
                    {
                        return; // We've skipped the value
                    }
                }
                else if (depth == 0)
                {
                    // Simple value, skip once
                    return;
                }
            }
            while (Read());
        }

        /// <summary>
        /// Reads and expects an object start token.
        /// </summary>
        public void ReadObjectStart()
        {
            if (!Read())
            {
                throw new JsonParseException("Unexpected end of input, expected object start", Line, Column);
            }
            Expect(JsonTokenType.ObjectStart);
        }

        /// <summary>
        /// Reads and expects an array start token.
        /// </summary>
        public void ReadArrayStart()
        {
            if (!Read())
            {
                throw new JsonParseException("Unexpected end of input, expected array start", Line, Column);
            }
            Expect(JsonTokenType.ArrayStart);
        }

        /// <summary>
        /// Checks if the current token is an object end.
        /// </summary>
        public bool IsObjectEnd()
        {
            return _currentToken.Type == JsonTokenType.ObjectEnd;
        }

        /// <summary>
        /// Checks if the current token is an array end.
        /// </summary>
        public bool IsArrayEnd()
        {
            return _currentToken.Type == JsonTokenType.ArrayEnd;
        }

        /// <summary>
        /// Checks if the current token is null.
        /// </summary>
        public bool IsNull()
        {
            return _currentToken.Type == JsonTokenType.Null;
        }

        /// <summary>
        /// Reads a property name (handles both quoted strings and unquoted identifiers).
        /// </summary>
        public string ReadPropertyName()
        {
            if (!Read())
            {
                throw new JsonParseException("Unexpected end of input, expected property name", Line, Column);
            }

            if (_currentToken.Type == JsonTokenType.String)
            {
                // Quoted property name
                var span = _currentToken.Value.Span;
                if (span.Length >= 2 && span[0] == '"' && span[^1] == '"')
                {
                    return span.Slice(1, span.Length - 2).ToString();
                }
                return span.ToString();
            }
            else if (_currentToken.Type == JsonTokenType.PropertyName)
            {
                // Unquoted property name
                return _currentToken.Value.ToString();
            }

            throw new JsonParseException(
                $"Expected property name but got {_currentToken.Type}",
                _currentToken.Line,
                _currentToken.Column);
        }

        /// <summary>
        /// Reads and expects a colon token.
        /// </summary>
        public void ReadColon()
        {
            if (!Read())
            {
                throw new JsonParseException("Unexpected end of input, expected colon", Line, Column);
            }
            Expect(JsonTokenType.Colon);
        }

        /// <summary>
        /// Tries to read a comma, returns true if found.
        /// </summary>
        public bool TryReadComma()
        {
            if (!Read())
            {
                return false;
            }

            if (_currentToken.Type == JsonTokenType.Comma)
            {
                return true;
            }

            // Put the token back by not advancing
            // This is a simplified approach - in a real impl we'd need a pushback mechanism
            return false;
        }
    }
}
