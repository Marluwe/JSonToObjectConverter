using Xunit;

namespace JsonToObjectConverter.Runtime.Tests
{
    public class JsonTokenizerCommentsTests
    {
        [Fact]
        public void Tokenize_SingleLineComment_IsSkipped()
        {
            var json = "// This is a comment\n42";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
            Assert.Equal("42", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_SingleLineCommentAtEnd_IsSkipped()
        {
            var json = "42 // Comment at end";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_MultipleSingleLineComments_AreSkipped()
        {
            var json = @"// Comment 1
// Comment 2
42
// Comment 3";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_SingleLineCommentInObject_IsSkipped()
        {
            var json = @"{
    // This is a property
    ""name"": ""John""
}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(JsonTokenType.ObjectStart, tokens[0].Type);
            Assert.Equal(JsonTokenType.String, tokens[1].Type);
        }

        [Fact]
        public void Tokenize_MultiLineComment_IsSkipped()
        {
            var json = "/* This is a comment */ 42";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
            Assert.Equal("42", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_MultiLineCommentSpanningLines_IsSkipped()
        {
            var json = @"/* This is a
multi-line
comment */
42";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_MultiLineCommentWithAsterisks_IsSkipped()
        {
            var json = @"/* This has ** asterisks *** inside */ 42";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Number, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_MultiLineCommentInObject_IsSkipped()
        {
            var json = @"{
    /* Property comment */
    ""name"": /* Value comment */ ""John""
}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(JsonTokenType.ObjectStart, tokens[0].Type);
            Assert.Equal(JsonTokenType.String, tokens[1].Type);
        }

        [Fact]
        public void Tokenize_MixedComments_AreSkipped()
        {
            var json = @"// Single line comment
{
    /* Multi-line
       comment */
    ""name"": ""John"", // Inline comment
    /* Another one */ ""age"": 30
}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(9, tokens.Count);
            Assert.Equal(JsonTokenType.ObjectStart, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_CommentInString_IsNotTreatedAsComment()
        {
            var json = "\"This // is not a comment\"";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_MultiLineCommentMarkersInString_IsNotTreatedAsComment()
        {
            var json = "\"This /* is */ not a comment\"";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.String, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_UnterminatedMultiLineComment_ThrowsException()
        {
            var json = "/* This comment is not closed 42";
            var tokenizer = new JsonTokenizer(json);

            var ex = Assert.Throws<JsonParseException>(() => tokenizer.Tokenize());
            Assert.Contains("Unterminated multi-line comment", ex.Message);
            Assert.Equal(1, ex.Line);
            Assert.Equal(1, ex.Column);
        }

        [Fact]
        public void Tokenize_UnterminatedMultiLineCommentSpanningLines_ThrowsException()
        {
            var json = @"/* This comment
spans multiple lines
but is not closed";
            var tokenizer = new JsonTokenizer(json);

            var ex = Assert.Throws<JsonParseException>(() => tokenizer.Tokenize());
            Assert.Contains("Unterminated multi-line comment", ex.Message);
        }

        [Fact]
        public void Tokenize_CommentAtStartOfFile_IsSkipped()
        {
            var json = "/* Header comment */ {\"x\":1}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(JsonTokenType.ObjectStart, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_CommentAtEndOfFile_IsSkipped()
        {
            var json = "{\"x\":1} // End comment";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(JsonTokenType.ObjectEnd, tokens[4].Type);
        }

        [Fact]
        public void Tokenize_OnlyComments_ReturnsEmptyList()
        {
            var json = @"// Just comments
/* Nothing
   else */
// More comments";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Empty(tokens);
        }

        [Fact]
        public void Tokenize_SlashNotFollowedByCommentMarker_IsNotComment()
        {
            var json = "1/2"; // This would be invalid JSON, but should not be treated as comment

            var tokenizer = new JsonTokenizer(json);

            // This will fail because '/' is an unexpected character after the number
            Assert.Throws<JsonParseException>(() => tokenizer.Tokenize());
        }
    }
}
