using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace JsonToObjectConverter.Runtime.Tests
{
    public class JsonTokenizerBasicTests
    {
        [Fact]
        public void Tokenize_EmptyObject_ReturnsCorrectTokens()
        {
            var tokenizer = new JsonTokenizer("{}");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(2, tokens.Count);
            Assert.Equal(JsonTokenType.ObjectStart, tokens[0].Type);
            Assert.Equal(JsonTokenType.ObjectEnd, tokens[1].Type);
        }

        [Fact]
        public void Tokenize_EmptyArray_ReturnsCorrectTokens()
        {
            var tokenizer = new JsonTokenizer("[]");
            var tokens = tokenizer.Tokenize();

            Assert.Equal(2, tokens.Count);
            Assert.Equal(JsonTokenType.ArrayStart, tokens[0].Type);
            Assert.Equal(JsonTokenType.ArrayEnd, tokens[1].Type);
        }

        [Fact]
        public void Tokenize_SimpleString_ReturnsStringToken()
        {
            var tokenizer = new JsonTokenizer("\"hello\"");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
            Assert.Equal("\"hello\"", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_StringWithEscapes_ProcessesEscapeSequences()
        {
            var tokenizer = new JsonTokenizer("\"hello\\nworld\\t!\"");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_StringWithAllEscapes_ProcessesCorrectly()
        {
            var tokenizer = new JsonTokenizer("\"\\\" \\\\ \\/ \\b \\f \\n \\r \\t\"");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_StringWithUnicodeEscape_ProcessesCorrectly()
        {
            var tokenizer = new JsonTokenizer("\"\\u0048\\u0065\\u006C\\u006C\\u006F\""); // "Hello"
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_Integer_ReturnsNumberToken()
        {
            var tokenizer = new JsonTokenizer("42");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
            Assert.Equal("42", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_NegativeInteger_ReturnsNumberToken()
        {
            var tokenizer = new JsonTokenizer("-123");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
            Assert.Equal("-123", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_Decimal_ReturnsNumberToken()
        {
            var tokenizer = new JsonTokenizer("3.14159");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
            Assert.Equal("3.14159", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_NumberWithExponent_ReturnsNumberToken()
        {
            var tokenizer = new JsonTokenizer("1.5e10");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
            Assert.Equal("1.5e10", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_NumberWithNegativeExponent_ReturnsNumberToken()
        {
            var tokenizer = new JsonTokenizer("2.5e-3");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
            Assert.Equal("2.5e-3", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_NumberWithPositiveExponent_ReturnsNumberToken()
        {
            var tokenizer = new JsonTokenizer("1E+5");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
            Assert.Equal("1E+5", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_Zero_ReturnsNumberToken()
        {
            var tokenizer = new JsonTokenizer("0");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
            Assert.Equal("0", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_True_ReturnsTrueToken()
        {
            var tokenizer = new JsonTokenizer("true");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.True, tokens[0].Type);
            Assert.Equal("true", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_False_ReturnsFalseToken()
        {
            var tokenizer = new JsonTokenizer("false");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.False, tokens[0].Type);
            Assert.Equal("false", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_Null_ReturnsNullToken()
        {
            var tokenizer = new JsonTokenizer("null");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Null, tokens[0].Type);
            Assert.Equal("null", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_Colon_ReturnsColonToken()
        {
            var tokenizer = new JsonTokenizer(":");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Colon, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_Comma_ReturnsCommaToken()
        {
            var tokenizer = new JsonTokenizer(",");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Comma, tokens[0].Type);
        }
    }

    public class JsonTokenizerComplexTests
    {
        [Fact]
        public void Tokenize_SimpleObject_ReturnsAllTokens()
        {
            var json = "{\"name\":\"John\"}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(JsonTokenType.ObjectStart, tokens[0].Type);
            Assert.Equal(JsonTokenType.String, tokens[1].Type);
            Assert.Equal(JsonTokenType.Colon, tokens[2].Type);
            Assert.Equal(JsonTokenType.String, tokens[3].Type);
            Assert.Equal(JsonTokenType.ObjectEnd, tokens[4].Type);
        }

        [Fact]
        public void Tokenize_ObjectWithMultipleProperties_ReturnsAllTokens()
        {
            var json = "{\"name\":\"John\",\"age\":30}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(9, tokens.Count);
            Assert.Equal(JsonTokenType.ObjectStart, tokens[0].Type);
            Assert.Equal(JsonTokenType.String, tokens[1].Type); // "name"
            Assert.Equal(JsonTokenType.Colon, tokens[2].Type);
            Assert.Equal(JsonTokenType.String, tokens[3].Type); // "John"
            Assert.Equal(JsonTokenType.Comma, tokens[4].Type);
            Assert.Equal(JsonTokenType.String, tokens[5].Type); // "age"
            Assert.Equal(JsonTokenType.Colon, tokens[6].Type);
            Assert.Equal(JsonTokenType.Number, tokens[7].Type); // 30
            Assert.Equal(JsonTokenType.ObjectEnd, tokens[8].Type);
        }

        [Fact]
        public void Tokenize_SimpleArray_ReturnsAllTokens()
        {
            var json = "[1,2,3]";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(7, tokens.Count);
            Assert.Equal(JsonTokenType.ArrayStart, tokens[0].Type);
            Assert.Equal(JsonTokenType.Number, tokens[1].Type);
            Assert.Equal(JsonTokenType.Comma, tokens[2].Type);
            Assert.Equal(JsonTokenType.Number, tokens[3].Type);
            Assert.Equal(JsonTokenType.Comma, tokens[4].Type);
            Assert.Equal(JsonTokenType.Number, tokens[5].Type);
            Assert.Equal(JsonTokenType.ArrayEnd, tokens[6].Type);
        }

        [Fact]
        public void Tokenize_NestedObject_ReturnsAllTokens()
        {
            var json = "{\"person\":{\"name\":\"John\"}}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(9, tokens.Count);
            Assert.Equal(JsonTokenType.ObjectStart, tokens[0].Type);
            Assert.Equal(JsonTokenType.String, tokens[1].Type); // "person"
            Assert.Equal(JsonTokenType.Colon, tokens[2].Type);
            Assert.Equal(JsonTokenType.ObjectStart, tokens[3].Type);
            Assert.Equal(JsonTokenType.String, tokens[4].Type); // "name"
            Assert.Equal(JsonTokenType.Colon, tokens[5].Type);
            Assert.Equal(JsonTokenType.String, tokens[6].Type); // "John"
            Assert.Equal(JsonTokenType.ObjectEnd, tokens[7].Type);
            Assert.Equal(JsonTokenType.ObjectEnd, tokens[8].Type);
        }

        [Fact]
        public void Tokenize_ArrayOfObjects_ReturnsAllTokens()
        {
            var json = "[{\"x\":1},{\"x\":2}]";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(13, tokens.Count);
        }

        [Fact]
        public void Tokenize_WithWhitespace_IgnoresWhitespace()
        {
            var json = "  {  \"name\"  :  \"John\"  }  ";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(JsonTokenType.ObjectStart, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_WithNewlines_TracksLineNumbers()
        {
            var json = "{\n\"name\": \"John\"\n}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(1, tokens[0].Line); // {
            Assert.Equal(2, tokens[1].Line); // "name"
            Assert.Equal(3, tokens[4].Line); // }
        }

        [Fact]
        public void Tokenize_TracksColumnNumbers()
        {
            var json = "{\"name\":\"John\"}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(1, tokens[0].Column); // {
            Assert.Equal(2, tokens[1].Column); // "name"
        }
    }

    public class JsonTokenizerEdgeCasesTests
    {
        [Fact]
        public void Tokenize_EmptyString_ReturnsEmptyList()
        {
            var tokenizer = new JsonTokenizer("");
            var tokens = tokenizer.Tokenize();

            Assert.Empty(tokens);
        }

        [Fact]
        public void Tokenize_WhitespaceOnly_ReturnsEmptyList()
        {
            var tokenizer = new JsonTokenizer("   \t\n\r  ");
            var tokens = tokenizer.Tokenize();

            Assert.Empty(tokens);
        }

        [Fact]
        public void Tokenize_EmptyString_InString_ReturnsStringToken()
        {
            var tokenizer = new JsonTokenizer("\"\"");
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Constructor_NullInput_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonTokenizer(null!));
        }
    }

    public class JsonTokenizerErrorHandlingTests
    {
        [Fact]
        public void Tokenize_UnterminatedString_ThrowsJsonParseException()
        {
            var tokenizer = new JsonTokenizer("\"hello");

            var ex = Assert.Throws<JsonParseException>(() => tokenizer.Tokenize());
            Assert.Contains("Unterminated string", ex.Message);
            Assert.Equal(1, ex.Line);
        }

        [Fact]
        public void Tokenize_InvalidEscapeSequence_ThrowsJsonParseException()
        {
            var tokenizer = new JsonTokenizer("\"hello\\x\"");

            var ex = Assert.Throws<JsonParseException>(() => tokenizer.Tokenize());
            Assert.Contains("Invalid escape sequence", ex.Message);
        }

        [Fact]
        public void Tokenize_InvalidUnicodeEscape_ThrowsJsonParseException()
        {
            var tokenizer = new JsonTokenizer("\"\\uXYZW\"");

            var ex = Assert.Throws<JsonParseException>(() => tokenizer.Tokenize());
            Assert.Contains("unicode", ex.Message.ToLower());
        }

        [Fact]
        public void Tokenize_InvalidNumber_ThrowsJsonParseException()
        {
            var tokenizer = new JsonTokenizer("123.456.789");

            var ex = Assert.Throws<JsonParseException>(() => tokenizer.Tokenize());
            Assert.NotNull(ex);
        }

        [Fact]
        public void Tokenize_InvalidLiteral_ThrowsJsonParseException()
        {
            var tokenizer = new JsonTokenizer("tru");

            var ex = Assert.Throws<JsonParseException>(() => tokenizer.Tokenize());
            Assert.Contains("Invalid literal", ex.Message);
        }

        [Fact]
        public void Tokenize_UnexpectedCharacter_ThrowsJsonParseException()
        {
            var tokenizer = new JsonTokenizer("@");

            var ex = Assert.Throws<JsonParseException>(() => tokenizer.Tokenize());
            Assert.Contains("Unexpected character", ex.Message);
        }

        [Fact]
        public void JsonParseException_IncludesLineAndColumn()
        {
            var tokenizer = new JsonTokenizer("{\"name\":\n\"John\n}");

            var ex = Assert.Throws<JsonParseException>(() => tokenizer.Tokenize());
            Assert.True(ex.Line > 0);
            Assert.True(ex.Column > 0);
        }
    }

    public class JsonTokenizerPerformanceTests
    {
        [Fact]
        public void Tokenize_LargeJson_CompletesInReasonableTime()
        {
            // Generate a large JSON array
            var sb = new StringBuilder();
            sb.Append('[');
            for (var i = 0; i < 10000; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append($"{{\"id\":{i},\"name\":\"Item{i}\",\"value\":{i * 1.5}}}");
            }
            sb.Append(']');

            var json = sb.ToString();
            var tokenizer = new JsonTokenizer(json);

            var sw = Stopwatch.StartNew();
            var tokens = tokenizer.Tokenize();
            sw.Stop();

            // Should complete in less than 100ms for 1MB+ of JSON
            Assert.True(sw.ElapsedMilliseconds < 100, $"Tokenization took {sw.ElapsedMilliseconds}ms");
            Assert.True(tokens.Count > 0);
        }

        [Fact]
        public void EnumerateTokens_SupportsLazyEvaluation()
        {
            var json = "{\"name\":\"John\",\"age\":30}";
            var tokenizer = new JsonTokenizer(json);

            var count = 0;
            foreach (var token in tokenizer.EnumerateTokens())
            {
                count++;
                Assert.NotEqual(JsonTokenType.None, token.Type);
            }

            Assert.Equal(9, count);
        }
    }

    public class JsonTokenTests
    {
        [Fact]
        public void JsonToken_Constructor_SetsAllProperties()
        {
            var token = new JsonToken(
                JsonTokenType.String,
                "hello".AsMemory(),
                10,
                5);

            Assert.Equal(JsonTokenType.String, token.Type);
            Assert.Equal("hello", token.Value.ToString());
            Assert.Equal(10, token.Line);
            Assert.Equal(5, token.Column);
        }

        [Fact]
        public void JsonToken_ToString_ReturnsFormattedString()
        {
            var token = new JsonToken(
                JsonTokenType.Number,
                "42".AsMemory(),
                1,
                1);

            var str = token.ToString();

            Assert.Contains("Number", str);
            Assert.Contains("1:1", str);
            Assert.Contains("42", str);
        }
    }
}
