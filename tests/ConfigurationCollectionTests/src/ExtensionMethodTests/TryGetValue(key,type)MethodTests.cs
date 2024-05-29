using System;
using TB.ComponentModel;
using WB.AppConfiguration;

namespace ConfigurationCollectionTests.ExtensionMethodTests.TryGetValue__key_type__MethodTests;

[TestClass]
public class TheTryGetValueMethod
{
    [TestMethod]
    public void ShouldDeliverTheValueAndReturnTrueIfTheValueExists()
    {
        // Arrange
        Configuration configuration = new();
        configuration.Push(new { Key = "Some Value" });

        // Act
        bool result = configuration.TryGetValue("Key", typeof(string), out object? value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be("Some Value");
    }

    [TestMethod]
    public void ShouldDeliverNullAndReturnFalseIfTheValueDoesNotExist()
    {
        // Arrange
        Configuration configuration = new();

        // Act
        bool result = configuration.TryGetValue("Key", typeof(string), out object? value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [TestMethod]
    public void ShouldThrowInvalidCastExceptionIfTheValueCouldNotBeConverted()
    {
        // Arrange
        Configuration configuration = new();
        configuration.Push(new { Key = "Some Value" });

        // Act
        Action action = () => configuration.TryGetValue("Key", typeof(bool), out object? value);

        // Assert
        action.Should().Throw<InvalidCastException>().WithInnerException<InvalidConversionException>(because: "The value 'Some Value' could not be converted to the target type 'System.Boolean'.");
    }
}
