using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace JsonToObjectConverter.Core.Tests
{
    public class JsonSerializableAttributeTests
    {
        [Fact]
        public void CanApplyToClass()
        {
            var attribute = typeof(TestClass).GetCustomAttribute<JsonSerializableAttribute>();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CanApplyToStruct()
        {
            var attribute = typeof(TestStruct).GetCustomAttribute<JsonSerializableAttribute>();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void AttributeUsage_AllowsClassAndStruct()
        {
            var attributeUsage = typeof(JsonSerializableAttribute)
                .GetCustomAttribute<AttributeUsageAttribute>();

            Assert.NotNull(attributeUsage);
            Assert.Equal(AttributeTargets.Class | AttributeTargets.Struct, attributeUsage.ValidOn);
            Assert.False(attributeUsage.AllowMultiple);
            Assert.False(attributeUsage.Inherited);
        }

        [JsonSerializable]
        private class TestClass { }

        [JsonSerializable]
        private struct TestStruct { }
    }

    public class JsonPropertyNameAttributeTests
    {
        [Fact]
        public void Constructor_SetsName()
        {
            var attribute = new JsonPropertyNameAttribute("custom_name");
            Assert.Equal("custom_name", attribute.Name);
        }

        [Fact]
        public void Constructor_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonPropertyNameAttribute(null!));
        }

        [Fact]
        public void Constructor_ThrowsOnEmpty()
        {
            Assert.Throws<ArgumentException>(() => new JsonPropertyNameAttribute(""));
        }

        [Fact]
        public void Constructor_ThrowsOnWhitespace()
        {
            Assert.Throws<ArgumentException>(() => new JsonPropertyNameAttribute("   "));
        }

        [Fact]
        public void CanApplyToProperty()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty));
            var attribute = property!.GetCustomAttribute<JsonPropertyNameAttribute>();

            Assert.NotNull(attribute);
            Assert.Equal("test_prop", attribute.Name);
        }

        [Fact]
        public void AttributeUsage_AllowsPropertyAndField()
        {
            var attributeUsage = typeof(JsonPropertyNameAttribute)
                .GetCustomAttribute<AttributeUsageAttribute>();

            Assert.NotNull(attributeUsage);
            Assert.Equal(AttributeTargets.Property | AttributeTargets.Field, attributeUsage.ValidOn);
            Assert.False(attributeUsage.AllowMultiple);
            Assert.True(attributeUsage.Inherited);
        }

        private class TestClass
        {
            [JsonPropertyName("test_prop")]
            public string TestProperty { get; set; } = "";
        }
    }

    public class JsonIgnoreAttributeTests
    {
        [Fact]
        public void CanApplyToProperty()
        {
            var property = typeof(TestClass).GetProperty(nameof(TestClass.IgnoredProperty));
            var attribute = property!.GetCustomAttribute<JsonIgnoreAttribute>();

            Assert.NotNull(attribute);
        }

        [Fact]
        public void CanApplyToField()
        {
            var field = typeof(TestClass).GetField(nameof(TestClass.IgnoredField));
            var attribute = field!.GetCustomAttribute<JsonIgnoreAttribute>();

            Assert.NotNull(attribute);
        }

        [Fact]
        public void AttributeUsage_AllowsPropertyAndField()
        {
            var attributeUsage = typeof(JsonIgnoreAttribute)
                .GetCustomAttribute<AttributeUsageAttribute>();

            Assert.NotNull(attributeUsage);
            Assert.Equal(AttributeTargets.Property | AttributeTargets.Field, attributeUsage.ValidOn);
            Assert.False(attributeUsage.AllowMultiple);
            Assert.True(attributeUsage.Inherited);
        }

        private class TestClass
        {
            [JsonIgnore]
            public string IgnoredProperty { get; set; } = "";

            [JsonIgnore]
            public string IgnoredField = "";
        }
    }

    public class JsonTypeDiscriminatorAttributeTests
    {
        [Fact]
        public void Constructor_SetsPropertyName()
        {
            var attribute = new JsonTypeDiscriminatorAttribute("type");
            Assert.Equal("type", attribute.PropertyName);
        }

        [Fact]
        public void Constructor_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonTypeDiscriminatorAttribute(null!));
        }

        [Fact]
        public void Constructor_ThrowsOnEmpty()
        {
            Assert.Throws<ArgumentException>(() => new JsonTypeDiscriminatorAttribute(""));
        }

        [Fact]
        public void Constructor_ThrowsOnWhitespace()
        {
            Assert.Throws<ArgumentException>(() => new JsonTypeDiscriminatorAttribute("   "));
        }

        [Fact]
        public void CanApplyToClass()
        {
            var attribute = typeof(TestClass).GetCustomAttribute<JsonTypeDiscriminatorAttribute>();
            Assert.NotNull(attribute);
            Assert.Equal("kind", attribute.PropertyName);
        }

        [Fact]
        public void AttributeUsage_AllowsClassAndInterface()
        {
            var attributeUsage = typeof(JsonTypeDiscriminatorAttribute)
                .GetCustomAttribute<AttributeUsageAttribute>();

            Assert.NotNull(attributeUsage);
            Assert.Equal(AttributeTargets.Class | AttributeTargets.Interface, attributeUsage.ValidOn);
            Assert.False(attributeUsage.AllowMultiple);
            Assert.False(attributeUsage.Inherited);
        }

        [JsonTypeDiscriminator("kind")]
        private abstract class TestClass { }
    }

    public class JsonTypeFactoryAttributeTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            var attribute = new JsonTypeFactoryAttribute("circle", typeof(string));
            Assert.Equal("circle", attribute.DiscriminatorValue);
            Assert.Equal(typeof(string), attribute.TargetType);
        }

        [Fact]
        public void Constructor_ThrowsOnNullValue()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonTypeFactoryAttribute(null!, typeof(string)));
        }

        [Fact]
        public void Constructor_ThrowsOnEmptyValue()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonTypeFactoryAttribute("", typeof(string)));
        }

        [Fact]
        public void Constructor_ThrowsOnWhitespaceValue()
        {
            Assert.Throws<ArgumentException>(() =>
                new JsonTypeFactoryAttribute("   ", typeof(string)));
        }

        [Fact]
        public void Constructor_ThrowsOnNullType()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new JsonTypeFactoryAttribute("circle", null!));
        }

        [Fact]
        public void CanApplyMultipleTimes()
        {
            var attributes = typeof(TestClass).GetCustomAttributes<JsonTypeFactoryAttribute>().ToArray();
            Assert.Equal(2, attributes.Length);

            Assert.Contains(attributes, a => a.DiscriminatorValue == "circle");
            Assert.Contains(attributes, a => a.DiscriminatorValue == "square");
        }

        [Fact]
        public void AttributeUsage_AllowsMultiple()
        {
            var attributeUsage = typeof(JsonTypeFactoryAttribute)
                .GetCustomAttribute<AttributeUsageAttribute>();

            Assert.NotNull(attributeUsage);
            Assert.Equal(AttributeTargets.Class | AttributeTargets.Interface, attributeUsage.ValidOn);
            Assert.True(attributeUsage.AllowMultiple);
            Assert.False(attributeUsage.Inherited);
        }

        [JsonTypeFactory("circle", typeof(Circle))]
        [JsonTypeFactory("square", typeof(Square))]
        private abstract class TestClass { }

        private class Circle : TestClass { }
        private class Square : TestClass { }
    }

    public class JsonConverterAttributeTests
    {
        [Fact]
        public void Constructor_SetsConverterType()
        {
            var attribute = new JsonConverterAttribute(typeof(string));
            Assert.Equal(typeof(string), attribute.ConverterType);
        }

        [Fact]
        public void Constructor_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonConverterAttribute(null!));
        }

        [Fact]
        public void CanApplyToClass()
        {
            var attribute = typeof(TestClass).GetCustomAttribute<JsonConverterAttribute>();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void CanApplyToProperty()
        {
            var property = typeof(TestClass2).GetProperty(nameof(TestClass2.Value));
            var attribute = property!.GetCustomAttribute<JsonConverterAttribute>();

            Assert.NotNull(attribute);
        }

        [Fact]
        public void AttributeUsage_AllowsClassStructPropertyField()
        {
            var attributeUsage = typeof(JsonConverterAttribute)
                .GetCustomAttribute<AttributeUsageAttribute>();

            Assert.NotNull(attributeUsage);
            Assert.Equal(
                AttributeTargets.Class | AttributeTargets.Struct |
                AttributeTargets.Property | AttributeTargets.Field,
                attributeUsage.ValidOn);
            Assert.False(attributeUsage.AllowMultiple);
            Assert.False(attributeUsage.Inherited);
        }

        [JsonConverter(typeof(string))]
        private class TestClass { }

        private class TestClass2
        {
            [JsonConverter(typeof(int))]
            public string Value { get; set; } = "";
        }
    }
}
