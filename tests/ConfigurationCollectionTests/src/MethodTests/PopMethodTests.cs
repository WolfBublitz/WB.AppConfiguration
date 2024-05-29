using System;
using WB.AppConfiguration;

namespace ConfigurationCollectionTests.MethodTests.PopMethodTests;

[TestClass]
public class ThePopMethod
{
    [TestMethod]
    public void ShouldPopTheLastObject()
    {
        // Arrange
        Configuration configuration = new();
        configuration.Push(new { Key1 = "Value 1" });
        configuration.Push(new { Key2 = "Value 2" });
        configuration.Push(new { Key3 = "Value 3" });

        // Act
        configuration.Pop();

        // Assert
        configuration.Layers.Should().BeEquivalentTo(
            new object[]
            {
                new { Key1 = "Value 1" },
                new { Key2 = "Value 2" },
            });
    }

    [TestMethod]
    public void ShouldThrowInvalidOperationExceptionIfConfigurationIsEmpty()
    {
        // Arrange
        Configuration configuration = new();

        // Act
        Action act = () => configuration.Pop();

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}
