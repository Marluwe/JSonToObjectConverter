using System;

namespace JsonToObjectConverter.Sample
{
    [JsonSerializable]
    public partial class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public string Email { get; set; } = "";
    }

    [JsonSerializable]
    public partial class Product
    {
        public string Name { get; set; } = "";
        public double Price { get; set; }
        public bool InStock { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("JsonToObjectConverter Sample");
            Console.WriteLine("============================");
            Console.WriteLine();

            // Test 1: Simple deserialization
            Console.WriteLine("Test 1: Simple Person deserialization");
            var json1 = "{\"Name\":\"John Doe\",\"Age\":30,\"Email\":\"john@example.com\"}";
            var person = Person.Deserialize(json1);
            Console.WriteLine($"  Name: {person.Name}");
            Console.WriteLine($"  Age: {person.Age}");
            Console.WriteLine($"  Email: {person.Email}");
            Console.WriteLine();

            // Test 2: Serialization
            Console.WriteLine("Test 2: Serialization");
            var person2 = new Person { Name = "Jane Smith", Age = 25, Email = "jane@example.com" };
            var serialized = person2.Serialize();
            Console.WriteLine($"  JSON: {serialized}");
            Console.WriteLine();

            // Test 3: Extended JSON syntax (unquoted keys, comments)
            Console.WriteLine("Test 3: Extended JSON syntax");
            var json3 = @"{
    // Person information
    Name: ""Bob Johnson"",  // Unquoted property names
    Age: 35,
    Email: ""bob@example.com""
}";
            var person3 = Person.Deserialize(json3);
            Console.WriteLine($"  Name: {person3.Name}");
            Console.WriteLine($"  Age: {person3.Age}");
            Console.WriteLine();

            // Test 4: Product with different types
            Console.WriteLine("Test 4: Product deserialization");
            var json4 = "{\"Name\":\"Laptop\",\"Price\":999.99,\"InStock\":true}";
            var product = Product.Deserialize(json4);
            Console.WriteLine($"  Name: {product.Name}");
            Console.WriteLine($"  Price: ${product.Price}");
            Console.WriteLine($"  In Stock: {product.InStock}");
            Console.WriteLine();

            // Test 5: Round-trip
            Console.WriteLine("Test 5: Round-trip (Deserialize -> Serialize -> Deserialize)");
            var original = new Product { Name = "Keyboard", Price = 79.99, InStock = false };
            var json5 = original.Serialize();
            var deserialized = Product.Deserialize(json5);
            Console.WriteLine($"  Original: {original.Name}, ${original.Price}, InStock={original.InStock}");
            Console.WriteLine($"  After round-trip: {deserialized.Name}, ${deserialized.Price}, InStock={deserialized.InStock}");
            Console.WriteLine();

            Console.WriteLine("All tests completed successfully!");
        }
    }
}
