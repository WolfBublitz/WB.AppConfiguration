using System;
using System.Collections.Generic;
using WB.AppConfiguration;

namespace ConfigurationTests.ExtensionMethodTests.GetValue_key_MethodTests;

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
        object? value = configuration.GetValue("Key");

        // Assert
        value.Should().Be("Some Value");
    }

    [TestMethod]
    public void ShouldThrowKeyNotFoundExceptionIfTheKeyDoesNotExist()
    {
        // Arrange
        ConfigurationCollection configuration = new();

        // Act
        Action act = () => configuration.GetValue("Key");

        // Assert
        act.Should().Throw<KeyNotFoundException>();
    }
}
