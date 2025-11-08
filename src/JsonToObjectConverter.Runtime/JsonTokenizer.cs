using System;
using System.Collections.Generic;
using System.Text;

namespace JsonToObjectConverter.Runtime
{
    /// <summary>
    /// Tokenizes JSON input into a sequence of tokens.
    /// Supports standard JSON syntax plus extended features:
    /// - Single-line comments (//)
    /// - Multi-line comments (/* */)
    /// - Unquoted property names (identifiers)
    /// - Multi-line string literals (strings can contain actual newlines)
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
                case 'f':
                case 'n':
                    // Could be true/false/null or an identifier
                    return ReadKeywordOrIdentifier(tokenLine, tokenColumn);

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
                    // Check if it's the start of an identifier (unquoted key)
                    if (IsIdentifierStart(ch))
                    {
                        return ReadIdentifier(tokenLine, tokenColumn);
                    }

                    throw new JsonParseException(
                        $"Unexpected character '{ch}'",
                        tokenLine,
                        tokenColumn,
                        GetContext());
            }
        }

        private JsonToken ReadKeywordOrIdentifier(int startLine, int startColumn)
        {
            var start = _position;

            // Try to match keywords
            if (TryMatchKeyword("true", JsonTokenType.True, out var token))
            {
                return token.Value;
            }
            if (TryMatchKeyword("false", JsonTokenType.False, out token))
            {
                return token.Value;
            }
            if (TryMatchKeyword("null", JsonTokenType.Null, out token))
            {
                return token.Value;
            }

            // Not a keyword, read as identifier
            _position = start; // Reset position
            _line = startLine;
            _column = startColumn;
            return ReadIdentifier(startLine, startColumn);
        }

        private bool TryMatchKeyword(string keyword, JsonTokenType tokenType, out JsonToken? token)
        {
            var start = _position;
            var startLine = _line;
            var startColumn = _column;

            // Check if we have enough characters
            if (_position + keyword.Length > _json.Length)
            {
                token = null;
                return false;
            }

            // Check if it matches
            for (var i = 0; i < keyword.Length; i++)
            {
                if (_json[_position + i] != keyword[i])
                {
                    token = null;
                    return false;
                }
            }

            // Check that it's not followed by an identifier character
            // (to avoid matching "trueValue" as "true")
            if (_position + keyword.Length < _json.Length)
            {
                var nextCh = _json[_position + keyword.Length];
                if (IsIdentifierPart(nextCh))
                {
                    // It's part of a longer identifier
                    token = null;
                    return false;
                }
            }

            // It's a valid keyword
            _position += keyword.Length;
            _column += keyword.Length;

            token = new JsonToken(tokenType, keyword.AsMemory(), startLine, startColumn);
            return true;
        }

        private JsonToken ReadIdentifier(int startLine, int startColumn)
        {
            var start = _position;

            // First character must be a letter or underscore
            if (!IsIdentifierStart(_json[_position]))
            {
                throw new JsonParseException(
                    $"Invalid identifier start character '{_json[_position]}'",
                    startLine,
                    startColumn,
                    GetContext());
            }

            Advance();

            // Read remaining identifier characters
            while (_position < _json.Length && IsIdentifierPart(_json[_position]))
            {
                Advance();
            }

            var value = _json.AsMemory(start, _position - start);

            // Return as PropertyName token (unquoted property names)
            return new JsonToken(JsonTokenType.PropertyName, value, startLine, startColumn);
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

                // Skip whitespace characters
                if (ch == ' ' || ch == '\t' || ch == '\r' || ch == '\n')
                {
                    Advance();
                    continue;
                }

                // Check for comments
                if (ch == '/')
                {
                    if (_position + 1 < _json.Length)
                    {
                        var nextCh = _json[_position + 1];

                        if (nextCh == '/')
                        {
                            // Single-line comment
                            SkipSingleLineComment();
                            continue;
                        }
                        else if (nextCh == '*')
                        {
                            // Multi-line comment
                            SkipMultiLineComment();
                            continue;
                        }
                    }
                }

                // No more whitespace or comments
                break;
            }
        }

        private void SkipSingleLineComment()
        {
            // Skip the "//"
            Advance();
            Advance();

            // Skip until end of line or end of input
            while (_position < _json.Length)
            {
                var ch = _json[_position];
                if (ch == '\n')
                {
                    Advance(); // Skip the newline
                    break;
                }
                Advance();
            }
        }

        private void SkipMultiLineComment()
        {
            var startLine = _line;
            var startColumn = _column;

            // Skip the "/*"
            Advance();
            Advance();

            // Skip until we find "*/"
            while (_position < _json.Length)
            {
                var ch = _json[_position];

                if (ch == '*' && _position + 1 < _json.Length && _json[_position + 1] == '/')
                {
                    // Found the end of the comment
                    Advance(); // Skip '*'
                    Advance(); // Skip '/'
                    return;
                }

                Advance();
            }

            // Reached end of input without closing the comment
            throw new JsonParseException(
                "Unterminated multi-line comment",
                startLine,
                startColumn,
                GetContext());
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

        private static bool IsIdentifierStart(char ch)
        {
            return (ch >= 'a' && ch <= 'z') ||
                   (ch >= 'A' && ch <= 'Z') ||
                   ch == '_' ||
                   ch == '$'; // Allow $ for JavaScript compatibility
        }

        private static bool IsIdentifierPart(char ch)
        {
            return IsIdentifierStart(ch) || IsDigit(ch);
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
