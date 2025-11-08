using Xunit;

namespace JsonToObjectConverter.Runtime.Tests
{
    public class JsonTokenizerMultilineStringsTests
    {
        [Fact]
        public void Tokenize_StringWithActualNewline_IsRecognized()
        {
            var json = @"""Hello
World""";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_MultilineStringInObject_IsRecognized()
        {
            var json = @"{
    ""description"": ""This is a
multi-line
string""
}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(JsonTokenType.String, tokens[1].Type);
            Assert.Equal(JsonTokenType.String, tokens[3].Type);
        }

        [Fact]
        public void Tokenize_StringWithMultipleNewlines_IsRecognized()
        {
            var json = @"""Line 1
Line 2
Line 3
Line 4""";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_StringWithEmptyLines_IsRecognized()
        {
            var json = @"""Line 1

Line 3""";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_MultilineStringTracksLineNumbers()
        {
            var json = @"{
""description"": ""This is
a multi-line
string"",
""age"": 30
}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(2, tokens[1].Line); // "description" on line 2
            Assert.Equal(2, tokens[3].Line); // string starts on line 2
            Assert.Equal(5, tokens[5].Line); // "age" on line 5
        }

        [Fact]
        public void Tokenize_MixedEscapedAndActualNewlines_IsRecognized()
        {
            var json = @"""Line 1\nLine 2
Actual newline""";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_MultilineStringWithTabs_IsRecognized()
        {
            var json = "\"Line 1\n\tIndented line\n\t\tDouble indented\"";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_MultilineStringWithCarriageReturn_IsRecognized()
        {
            var json = "\"Line 1\r\nLine 2\r\nLine 3\"";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_MultilineStringWithUnicodeCharacters_IsRecognized()
        {
            var json = @"""Hello
Wörld
こんにちは""";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_ArrayWithMultilineStrings_IsRecognized()
        {
            var json = @"[
    ""First
    multi-line
    string"",
    ""Second
    multi-line
    string""
]";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(JsonTokenType.ArrayStart, tokens[0].Type);
            Assert.Equal(JsonTokenType.String, tokens[1].Type);
            Assert.Equal(JsonTokenType.Comma, tokens[2].Type);
            Assert.Equal(JsonTokenType.String, tokens[3].Type);
            Assert.Equal(JsonTokenType.ArrayEnd, tokens[4].Type);
        }

        [Fact]
        public void Tokenize_MultilineStringAfterComment_IsRecognized()
        {
            var json = @"// Comment
{
    ""text"": ""Multi
line
string""
}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.String, tokens[3].Type);
        }
    }
}
