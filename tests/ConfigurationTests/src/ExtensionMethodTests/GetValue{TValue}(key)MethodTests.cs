using System;
using System.Collections.Generic;
using WB.AppConfiguration;

namespace ConfigurationTests.ExtensionMethodTests.GetValue_TValue_key_MethodTests;

[TestClass]
public class TheGetValueMethod
{
    [TestMethod]
    public void ShouldReturnTheValueCastedToType()
    {
        // Arrange
        ConfigurationCollection configuration = new();
        configuration.Push(new { Key = "Some Value" });

        // Act
        string value = configuration.GetValue<string>("Key");

        // Assert
        value.Should().Be("Some Value");
    }

    [TestMethod]
    public void ShouldThrowKeyNotFoundExceptionIfTheKeyDoesNotExist()
    {
        // Arrange
        ConfigurationCollection configuration = new();

        // Act
        Action act = () => configuration.GetValue<string>("Key");

        // Assert
        act.Should().Throw<KeyNotFoundException>();
    }

    [TestMethod]
    public void ShouldThrowInvalidCastExceptinIfTheValueCouldNotBeConverted()
    {
        // Arrange
        ConfigurationCollection configuration = new();
        configuration.Push(new { Key = "Some Value" });

        // Act
        Action act = () => configuration.GetValue<int>("Key");

        // Assert
        act.Should().Throw<InvalidCastException>();
    }

    [TestMethod]
    public void ShouldThrowInvalidCastExceptinIfTheValueIsNull()
    {
        // Arrange
        ConfigurationCollection configuration = new();
        configuration.Push(new Dictionary<string, object?> { ["Key"] = null });

        // Act
        Action act = () => configuration.GetValue<int>("Key");

        // Assert
        act.Should().Throw<InvalidCastException>();
    }
}
