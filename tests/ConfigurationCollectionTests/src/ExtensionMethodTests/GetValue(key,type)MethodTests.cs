using System;
using System.Collections.Generic;
using WB.AppConfiguration;

namespace ConfigurationCollectionTests.ExtensionMethodTests.GetValue_key_type_MethodTests;

[TestClass]
public class TheGetValueMethod
{
    [TestMethod]
    public void ShouldReturnTheValueCastedToType()
    {
        // Arrange
        Configuration configuration = new();
        configuration.Push(new { Key = "Some Value" });

        // Act
        object? value = configuration.GetValue("Key", typeof(string));

        // Assert
        value.Should().BeOfType<string>();
        value.Should().Be("Some Value");
    }

    [TestMethod]
    public void ShouldReturnNullIfTheValueIsNull()
    {
        // Arrange
        Configuration configuration = new();
        configuration.Push(new Dictionary<string, object?> { ["Key"] = null });

        // Act
        object? value = configuration.GetValue("Key", typeof(int));

        // Assert
        value.Should().BeNull();
    }

    [TestMethod]
    public void ShouldThrowKeyNotFoundExceptionIfTheKeyDoesNotExist()
    {
        // Arrange
        Configuration configuration = new();

        // Act
        Action act = () => configuration.GetValue("Key", typeof(string));

        // Assert
        act.Should().Throw<KeyNotFoundException>();
    }

    [TestMethod]
    public void ShouldThrowInvalidCastExceptinIfTheValueCouldNotBeConverted()
    {
        // Arrange
        Configuration configuration = new();
        configuration.Push(new { Key = "Some Value" });

        // Act
        Action act = () => configuration.GetValue("Key", typeof(int));

        // Assert
        act.Should().Throw<InvalidCastException>();
    }
}
