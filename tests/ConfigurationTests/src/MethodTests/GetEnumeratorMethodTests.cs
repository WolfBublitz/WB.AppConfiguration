using System;
using System.Collections;
using System.Collections.Generic;
using WB.AppConfiguration;

namespace ConfigurationTests.MethodTests.GetEnumeratorMethodTests;

[TestClass]
public class TheGetEnumeratorMethod
{
    [TestMethod]
    public void ShouldProvideAnEnumerator()
    {
        // Arrange
        ConfigurationCollection configuration = new();
        configuration.Push(new { Key1 = "Value 1" });
        configuration.Push(new { Key2 = "Value 2" });
        configuration.Push(new { Key3 = "Value 3" });

        List<KeyValuePair<object, object?>> keyValuePairs = [];

        // Act
        foreach (KeyValuePair<object, object?> pair in configuration)
        {
            keyValuePairs.Add(pair);
        }

        // Assert
        keyValuePairs.Should().BeEquivalentTo(
            new[]
            {
                new KeyValuePair<string, object?>("Key1", "Value 1"),
                new KeyValuePair<string, object?>("Key2", "Value 2"),
                new KeyValuePair<string, object?>("Key3", "Value 3"),
            });
    }

    [TestMethod]
    public void ShouldProvideAnEnumeratorAsIEnumerable()
    {
        // Arrange
        ConfigurationCollection configuration = new();
        configuration.Push(new { Key1 = "Value 1" });
        configuration.Push(new { Key2 = "Value 2" });
        configuration.Push(new { Key3 = "Value 3" });

        List<KeyValuePair<object, object?>> keyValuePairs = [];

        // Act
        foreach (KeyValuePair<object, object?> pair in (IEnumerable)configuration)
        {
            keyValuePairs.Add(pair);
        }

        // Assert
        keyValuePairs.Should().BeEquivalentTo(
            new[]
            {
                new KeyValuePair<string, object?>("Key1", "Value 1"),
                new KeyValuePair<string, object?>("Key2", "Value 2"),
                new KeyValuePair<string, object?>("Key3", "Value 3"),
            });
    }
}
