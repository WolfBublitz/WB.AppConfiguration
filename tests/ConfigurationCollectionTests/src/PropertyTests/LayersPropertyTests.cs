using System;
using System.Collections.Generic;
using System.Reflection;
using WB.AppConfiguration;

namespace ConfigurationCollectionTests.PropertyTests.CountPropertyTests;

[TestClass]
public class TheLayersProperty
{
    [TestMethod]
    public void ShouldBeEmptyAtDefault()
    {
        // Assert
        Configuration configuration = new();

        // Act
        IEnumerable<object> layers = configuration.Layers;

        // Assert
        layers.Should().BeEmpty();
    }

    [TestMethod]
    public void ShouldProvideTheListOfPushedLayers()
    {
        // Assert
        Configuration configuration = new();
        object layer1 = new { Key = "Value" };
        object layer2 = new { Key = "Another Value" };

        configuration.Push(layer1);
        configuration.Push(layer2);

        // Act
        IEnumerable<object> layers = configuration.Layers;

        // Assert
        layers.Should().BeEquivalentTo(new[] { layer1, layer2 });
    }
}
