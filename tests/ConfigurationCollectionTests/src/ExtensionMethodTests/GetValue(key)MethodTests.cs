using System;
using System.Collections.Generic;
using WB.AppConfiguration;

namespace ConfigurationCollectionTests.ExtensionMethodTests.GetValue_key_MethodTests;

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
        object? value = configuration.GetValue("Key");

        // Assert
        value.Should().Be("Some Value");
    }

    [TestMethod]
    public void ShouldThrowKeyNotFoundExceptionIfTheKeyDoesNotExist()
    {
        // Arrange
        Configuration configuration = new();

        // Act
        Action act = () => configuration.GetValue("Key");

        // Assert
        act.Should().Throw<KeyNotFoundException>();
    }
}
