using Xunit;

namespace JsonToObjectConverter.Runtime.Tests
{
    public class JsonReaderTests
    {
        [Fact]
        public void Read_SimpleObject_ReadsAllTokens()
        {
            var json = "{\"name\":\"John\"}";
            var reader = new JsonReader(json);

            Assert.True(reader.Read());
            Assert.Equal(JsonTokenType.ObjectStart, reader.TokenType);

            Assert.True(reader.Read());
            Assert.Equal(JsonTokenType.String, reader.TokenType);

            Assert.True(reader.Read());
            Assert.Equal(JsonTokenType.Colon, reader.TokenType);

            Assert.True(reader.Read());
            Assert.Equal(JsonTokenType.String, reader.TokenType);

            Assert.True(reader.Read());
            Assert.Equal(JsonTokenType.ObjectEnd, reader.TokenType);

            Assert.False(reader.Read());
        }

        [Fact]
        public void GetString_StringToken_ReturnsValueWithoutQuotes()
        {
            var json = "\"hello\"";
            var reader = new JsonReader(json);

            reader.Read();
            var value = reader.GetString();

            Assert.Equal("hello", value);
        }

        [Fact]
        public void GetInt32_NumberToken_ReturnsInteger()
        {
            var json = "42";
            var reader = new JsonReader(json);

            reader.Read();
            var value = reader.GetInt32();

            Assert.Equal(42, value);
        }

        [Fact]
        public void GetDouble_NumberToken_ReturnsDouble()
        {
            var json = "3.14";
            var reader = new JsonReader(json);

            reader.Read();
            var value = reader.GetDouble();

            Assert.Equal(3.14, value, precision: 2);
        }

        [Fact]
        public void GetBoolean_TrueToken_ReturnsTrue()
        {
            var json = "true";
            var reader = new JsonReader(json);

            reader.Read();
            var value = reader.GetBoolean();

            Assert.True(value);
        }

        [Fact]
        public void GetBoolean_FalseToken_ReturnsFalse()
        {
            var json = "false";
            var reader = new JsonReader(json);

            reader.Read();
            var value = reader.GetBoolean();

            Assert.False(value);
        }

        [Fact]
        public void ReadPropertyName_QuotedName_ReturnsNameWithoutQuotes()
        {
            var json = "{\"name\":\"value\"}";
            var reader = new JsonReader(json);

            reader.ReadObjectStart();
            var propertyName = reader.ReadPropertyName();

            Assert.Equal("name", propertyName);
        }

        [Fact]
        public void ReadPropertyName_UnquotedName_ReturnsName()
        {
            var json = "{name:\"value\"}";
            var reader = new JsonReader(json);

            reader.ReadObjectStart();
            var propertyName = reader.ReadPropertyName();

            Assert.Equal("name", propertyName);
        }

        [Fact]
        public void IsNull_NullToken_ReturnsTrue()
        {
            var json = "null";
            var reader = new JsonReader(json);

            reader.Read();

            Assert.True(reader.IsNull());
        }

        [Fact]
        public void IsObjectEnd_ObjectEndToken_ReturnsTrue()
        {
            var json = "{}";
            var reader = new JsonReader(json);

            reader.Read(); // {
            reader.Read(); // }

            Assert.True(reader.IsObjectEnd());
        }

        [Fact]
        public void Skip_SimpleValue_SkipsValue()
        {
            var json = "[1,2,3]";
            var reader = new JsonReader(json);

            reader.ReadArrayStart();
            reader.Read(); // 1
            reader.Skip();
            reader.Read(); // Should be at comma
            Assert.Equal(JsonTokenType.Comma, reader.TokenType);
        }

        [Fact]
        public void Skip_NestedObject_SkipsEntireObject()
        {
            var json = "[{\"a\":1,\"b\":2},3]";
            var reader = new JsonReader(json);

            reader.ReadArrayStart();
            reader.Read(); // {
            reader.Skip(); // Should skip entire object
            reader.Read(); // Should be at comma
            Assert.Equal(JsonTokenType.Comma, reader.TokenType);
        }
    }
}
