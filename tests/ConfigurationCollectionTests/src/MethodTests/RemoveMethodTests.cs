using System;
using WB.AppConfiguration;

namespace ConfigurationCollectionTests.MethodTests.RemoveMethodTests;

[TestClass]
public class TheRemoveMethod
{
    [TestMethod]
    public void ShouldRemoveTheLayer()
    {
        // Arrange
        Configuration configuration = new();
        object layer1 = new { Key1 = "Value 1" };
        object layer2 = new { Key2 = "Value 2" };
        object layer3 = new { Key3 = "Value 3" };
        configuration.Push(layer1);
        configuration.Push(layer2);
        configuration.Push(layer3);

        // Act
        configuration.Remove(layer2);

        // Assert
        configuration.Layers.Should().BeEquivalentTo(
            new object[]
            {
                layer1,
                layer3,
            });
    }

    [TestMethod]
    public void ShouldThrowArgumentNullExceptionIfLayerIsNull()
    {
        // Arrange
        Configuration configuration = new();

        // Act
        Action act = () => configuration.Remove(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
