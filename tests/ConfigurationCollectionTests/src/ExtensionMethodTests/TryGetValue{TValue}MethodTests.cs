using System;
using TB.ComponentModel;
using WB.AppConfiguration;

namespace ConfigurationCollectionTests.ExtensionMethodTests.TryGetValue_TValue_MethodTests;

[TestClass]
public class TheTryGetValueMethod
{
    [TestMethod]
    public void ShouldDeliverTheValueAndReturnTrueIfTheValueExists()
    {
        // Arrange
        ConfigurationCollection configuration = new();
        configuration.Push(new { Key = "Some Value" });

        // Act
        bool result = configuration.TryGetValue("Key", out string value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be("Some Value");
    }

    [TestMethod]
    public void ShouldDeliverNullAndReturnFalseIfTheValueDoesNotExist()
    {
        // Arrange
        ConfigurationCollection configuration = new();

        // Act
        bool result = configuration.TryGetValue("Key", out string value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [TestMethod]
    public void ShouldThrowInvalidCastExceptionIfTheValueCouldNotBeConverted()
    {
        // Arrange
        ConfigurationCollection configuration = new();
        configuration.Push(new { Key = "Some Value" });

        // Act
        Action action = () => configuration.TryGetValue("Key", out bool value);

        // Assert
        action.Should().Throw<InvalidCastException>().WithInnerException<InvalidConversionException>(because: "The value 'Some Value' could not be converted to the target type 'System.Boolean'.");
    }
}
