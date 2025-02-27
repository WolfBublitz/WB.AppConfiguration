using System;
using WB.AppConfiguration;

namespace ConfigurationCollectionTests.MethodTests.TryGetValueMethodTests;

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
        bool result = configuration.TryGetValue("Key", out object? value);

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
        bool result = configuration.TryGetValue("Key", out object? value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }
}
