using System;
using WB.AppConfiguration;

namespace ConfigurationCollectionTests.MethodTests.ContainsKeyMethodTests;

[TestClass]
public class ThePushMethod
{
    [TestMethod]
    public void ShouldPushOnObject()
    {
        // Arrange
        ConfigurationCollection configuration = new();

        // Assert
        configuration.Should().NotContainKey("Key");

        // Act
        configuration.Push(new { Key = "Some Value" });

        // Assert
        configuration.Should().ContainKey("Key");
    }

    [TestMethod]
    public void ShouldThrowArgumentNullExceptionIfKeyIsNull()
    {
        // Arrange
        ConfigurationCollection configuration = new();

        // Act
        Action act = () => configuration.ContainsKey(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
