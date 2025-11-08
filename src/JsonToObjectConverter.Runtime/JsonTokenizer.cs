using System;
using System.Collections.Generic;
using System.Text;

namespace JsonToObjectConverter.Runtime
{
    /// <summary>
    /// Tokenizes JSON input into a sequence of tokens.
    /// Supports standard JSON syntax.
    /// </summary>
    public class JsonTokenizer
    {
        private readonly string _json;
        private int _position;
        private int _line;
        private int _column;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTokenizer"/> class.
        /// </summary>
        /// <param name="json">The JSON string to tokenize.</param>
        public JsonTokenizer(string json)
        {
            _json = json ?? throw new ArgumentNullException(nameof(json));
            _position = 0;
            _line = 1;
            _column = 1;
        }

        /// <summary>
        /// Tokenizes the entire JSON string and returns all tokens.
        /// </summary>
        public List<JsonToken> Tokenize()
        {
            var tokens = new List<JsonToken>();

            while (_position < _json.Length)
            {
                SkipWhitespace();

                if (_position >= _json.Length)
                {
                    break;
                }

                var token = ReadNextToken();
                tokens.Add(token);
            }

            return tokens;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the tokens.
        /// </summary>
        public IEnumerable<JsonToken> EnumerateTokens()
        {
            while (_position < _json.Length)
            {
                SkipWhitespace();

                if (_position >= _json.Length)
                {
                    yield break;
                }

                yield return ReadNextToken();
            }
        }

        private JsonToken ReadNextToken()
        {
            if (_position >= _json.Length)
            {
                throw new JsonParseException("Unexpected end of input", _line, _column);
            }

            var ch = _json[_position];
            var tokenLine = _line;
            var tokenColumn = _column;

            switch (ch)
            {
                case '{':
                    Advance();
                    return new JsonToken(JsonTokenType.ObjectStart, "{".AsMemory(), tokenLine, tokenColumn);

                case '}':
                    Advance();
                    return new JsonToken(JsonTokenType.ObjectEnd, "}".AsMemory(), tokenLine, tokenColumn);

                case '[':
                    Advance();
                    return new JsonToken(JsonTokenType.ArrayStart, "[".AsMemory(), tokenLine, tokenColumn);

                case ']':
                    Advance();
                    return new JsonToken(JsonTokenType.ArrayEnd, "]".AsMemory(), tokenLine, tokenColumn);

                case ':':
                    Advance();
                    return new JsonToken(JsonTokenType.Colon, ":".AsMemory(), tokenLine, tokenColumn);

                case ',':
                    Advance();
                    return new JsonToken(JsonTokenType.Comma, ",".AsMemory(), tokenLine, tokenColumn);

                case '"':
                    return ReadString(tokenLine, tokenColumn);

                case 't':
                    return ReadLiteral("true", JsonTokenType.True, tokenLine, tokenColumn);

                case 'f':
                    return ReadLiteral("false", JsonTokenType.False, tokenLine, tokenColumn);

                case 'n':
                    return ReadLiteral("null", JsonTokenType.Null, tokenLine, tokenColumn);

                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return ReadNumber(tokenLine, tokenColumn);

                default:
                    throw new JsonParseException(
                        $"Unexpected character '{ch}'",
                        tokenLine,
                        tokenColumn,
                        GetContext());
            }
        }

        private JsonToken ReadString(int startLine, int startColumn)
        {
            var start = _position;
            Advance(); // Skip opening quote

            var sb = new StringBuilder();
            var hasEscapes = false;

            while (_position < _json.Length)
            {
                var ch = _json[_position];

                if (ch == '"')
                {
                    // End of string
                    Advance();
                    var rawValue = _json.AsMemory(start, _position - start);

                    // If no escapes, we can use the raw value
                    if (!hasEscapes)
                    {
                        return new JsonToken(JsonTokenType.String, rawValue, startLine, startColumn);
                    }

                    // Otherwise return the processed string
                    return new JsonToken(
                        JsonTokenType.String,
                        sb.ToString().AsMemory(),
                        startLine,
                        startColumn);
                }

                if (ch == '\\')
                {
                    hasEscapes = true;
                    Advance();

                    if (_position >= _json.Length)
                    {
                        throw new JsonParseException(
                            "Unterminated string - escape at end of input",
                            _line,
                            _column);
                    }

                    var escapeChar = _json[_position];
                    switch (escapeChar)
                    {
                        case '"':
                            sb.Append('"');
                            break;
                        case '\\':
                            sb.Append('\\');
                            break;
                        case '/':
                            sb.Append('/');
                            break;
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'u':
                            // Unicode escape sequence \uXXXX
                            Advance();
                            var hexStart = _position;
                            if (_position + 4 > _json.Length)
                            {
                                throw new JsonParseException(
                                    "Invalid unicode escape sequence",
                                    _line,
                                    _column);
                            }

                            var hex = _json.Substring(_position, 4);
                            if (!int.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out var codePoint))
                            {
                                throw new JsonParseException(
                                    $"Invalid unicode escape sequence '\\u{hex}'",
                                    _line,
                                    _column);
                            }

                            sb.Append((char)codePoint);
                            _position += 3; // Will be advanced once more below
                            _column += 3;
                            break;

                        default:
                            throw new JsonParseException(
                                $"Invalid escape sequence '\\{escapeChar}'",
                                _line,
                                _column);
                    }

                    Advance();
                }
                else
                {
                    if (hasEscapes)
                    {
                        sb.Append(ch);
                    }
                    Advance();
                }
            }

            throw new JsonParseException(
                "Unterminated string",
                startLine,
                startColumn,
                GetContext());
        }

        private JsonToken ReadNumber(int startLine, int startColumn)
        {
            var start = _position;

            // Optional minus sign
            if (_position < _json.Length && _json[_position] == '-')
            {
                Advance();
            }

            // Integer part
            if (_position >= _json.Length || !IsDigit(_json[_position]))
            {
                throw new JsonParseException(
                    "Invalid number format",
                    _line,
                    _column,
                    GetContext());
            }

            // Leading zero or multiple digits
            if (_json[_position] == '0')
            {
                Advance();
            }
            else
            {
                while (_position < _json.Length && IsDigit(_json[_position]))
                {
                    Advance();
                }
            }

            // Optional fractional part
            if (_position < _json.Length && _json[_position] == '.')
            {
                Advance();

                if (_position >= _json.Length || !IsDigit(_json[_position]))
                {
                    throw new JsonParseException(
                        "Invalid number format - expected digit after decimal point",
                        _line,
                        _column,
                        GetContext());
                }

                while (_position < _json.Length && IsDigit(_json[_position]))
                {
                    Advance();
                }
            }

            // Optional exponent part
            if (_position < _json.Length && (_json[_position] == 'e' || _json[_position] == 'E'))
            {
                Advance();

                if (_position < _json.Length && (_json[_position] == '+' || _json[_position] == '-'))
                {
                    Advance();
                }

                if (_position >= _json.Length || !IsDigit(_json[_position]))
                {
                    throw new JsonParseException(
                        "Invalid number format - expected digit in exponent",
                        _line,
                        _column,
                        GetContext());
                }

                while (_position < _json.Length && IsDigit(_json[_position]))
                {
                    Advance();
                }
            }

            var value = _json.AsMemory(start, _position - start);
            return new JsonToken(JsonTokenType.Number, value, startLine, startColumn);
        }

        private JsonToken ReadLiteral(string literal, JsonTokenType tokenType, int startLine, int startColumn)
        {
            var start = _position;

            for (var i = 0; i < literal.Length; i++)
            {
                if (_position >= _json.Length || _json[_position] != literal[i])
                {
                    throw new JsonParseException(
                        $"Invalid literal - expected '{literal}'",
                        startLine,
                        startColumn,
                        GetContext());
                }
                Advance();
            }

            var value = _json.AsMemory(start, literal.Length);
            return new JsonToken(tokenType, value, startLine, startColumn);
        }

        private void SkipWhitespace()
        {
            while (_position < _json.Length)
            {
                var ch = _json[_position];
                if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
                {
                    Advance();
                }
                else
                {
                    break;
                }
            }
        }

        private void Advance()
        {
            if (_position < _json.Length)
            {
                var ch = _json[_position];
                _position++;

                if (ch == '\n')
                {
                    _line++;
                    _column = 1;
                }
                else
                {
                    _column++;
                }
            }
        }

        private static bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private string GetContext()
        {
            var contextStart = Math.Max(0, _position - 20);
            var contextEnd = Math.Min(_json.Length, _position + 20);
            var contextLength = contextEnd - contextStart;

            if (contextLength == 0)
            {
                return string.Empty;
            }

            var context = _json.Substring(contextStart, contextLength);
            var markerPos = _position - contextStart;

            if (markerPos >= 0 && markerPos < context.Length)
            {
                return context.Substring(0, markerPos) + ">>>" + context.Substring(markerPos);
            }

            return context;
        }
    }
}
