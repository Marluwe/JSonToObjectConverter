using Xunit;

namespace JsonToObjectConverter.Runtime.Tests
{
    public class JsonTokenizerUnquotedKeysTests
    {
        [Fact]
        public void Tokenize_SimpleUnquotedKey_ReturnsPropertyNameToken()
        {
            var json = "name";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.PropertyName, tokens[0].Type);
            Assert.Equal("name", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_UnquotedKeyInObject_IsRecognized()
        {
            var json = "{name: \"John\"}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(5, tokens.Count);
            Assert.Equal(JsonTokenType.ObjectStart, tokens[0].Type);
            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("name", tokens[1].Value.ToString());
            Assert.Equal(JsonTokenType.Colon, tokens[2].Type);
            Assert.Equal(JsonTokenType.String, tokens[3].Type);
            Assert.Equal(JsonTokenType.ObjectEnd, tokens[4].Type);
        }

        [Fact]
        public void Tokenize_MultipleUnquotedKeys_AreRecognized()
        {
            var json = "{name: \"John\", age: 30}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("name", tokens[1].Value.ToString());
            Assert.Equal(JsonTokenType.PropertyName, tokens[5].Type);
            Assert.Equal("age", tokens[5].Value.ToString());
        }

        [Fact]
        public void Tokenize_UnquotedKeyWithUnderscore_IsRecognized()
        {
            var json = "{first_name: \"John\"}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("first_name", tokens[1].Value.ToString());
        }

        [Fact]
        public void Tokenize_UnquotedKeyWithNumbers_IsRecognized()
        {
            var json = "{name123: \"value\"}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("name123", tokens[1].Value.ToString());
        }

        [Fact]
        public void Tokenize_UnquotedKeyStartingWithDollar_IsRecognized()
        {
            var json = "{$id: \"value\"}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("$id", tokens[1].Value.ToString());
        }

        [Fact]
        public void Tokenize_UnquotedKeyStartingWithUnderscore_IsRecognized()
        {
            var json = "{_private: \"value\"}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("_private", tokens[1].Value.ToString());
        }

        [Fact]
        public void Tokenize_TrueAsKeyword_RecognizedAsBoolean()
        {
            var json = "true";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.True, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_FalseAsKeyword_RecognizedAsBoolean()
        {
            var json = "false";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.False, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_NullAsKeyword_RecognizedAsNull()
        {
            var json = "null";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.Null, tokens[0].Type);
        }

        [Fact]
        public void Tokenize_TrueValueAsIdentifier_RecognizedAsPropertyName()
        {
            var json = "trueValue";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.PropertyName, tokens[0].Type);
            Assert.Equal("trueValue", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_FalseValueAsIdentifier_RecognizedAsPropertyName()
        {
            var json = "falseValue";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.PropertyName, tokens[0].Type);
            Assert.Equal("falseValue", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_NullValueAsIdentifier_RecognizedAsPropertyName()
        {
            var json = "nullValue";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Single(tokens);
            Assert.Equal(JsonTokenType.PropertyName, tokens[0].Type);
            Assert.Equal("nullValue", tokens[0].Value.ToString());
        }

        [Fact]
        public void Tokenize_MixedQuotedAndUnquotedKeys_AreRecognized()
        {
            var json = "{name: \"John\", \"age\": 30}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("name", tokens[1].Value.ToString());
            Assert.Equal(JsonTokenType.String, tokens[5].Type);
            Assert.Equal("\"age\"", tokens[5].Value.ToString());
        }

        [Fact]
        public void Tokenize_UnquotedKeyWithWhitespace_RecognizedCorrectly()
        {
            var json = "{  name  :  \"John\"  }";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("name", tokens[1].Value.ToString());
        }

        [Fact]
        public void Tokenize_CamelCaseUnquotedKey_IsRecognized()
        {
            var json = "{firstName: \"John\"}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("firstName", tokens[1].Value.ToString());
        }

        [Fact]
        public void Tokenize_PascalCaseUnquotedKey_IsRecognized()
        {
            var json = "{FirstName: \"John\"}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("FirstName", tokens[1].Value.ToString());
        }

        [Fact]
        public void Tokenize_NestedUnquotedKeys_AreRecognized()
        {
            var json = "{person: {name: \"John\", age: 30}}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("person", tokens[1].Value.ToString());
            Assert.Equal(JsonTokenType.PropertyName, tokens[4].Type);
            Assert.Equal("name", tokens[4].Value.ToString());
            Assert.Equal(JsonTokenType.PropertyName, tokens[8].Type);
            Assert.Equal("age", tokens[8].Value.ToString());
        }

        [Fact]
        public void Tokenize_UnquotedKeyWithComments_IsRecognized()
        {
            var json = @"{
    // Property name
    name: ""John""
}";
            var tokenizer = new JsonTokenizer(json);
            var tokens = tokenizer.Tokenize();

            Assert.Equal(JsonTokenType.PropertyName, tokens[1].Type);
            Assert.Equal("name", tokens[1].Value.ToString());
        }
    }
}
