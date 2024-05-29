using System;
using System.Collections.Generic;
using System.Reflection;
using WB.AppConfiguration;

namespace ConfigurationTests.PropertyTests.KeysPropertyTests;

internal sealed class NoLayerDataAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        yield return new object?[] { Array.Empty<object>(), Array.Empty<object?>() };
    }

    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => "no layers";
}

internal sealed class OneLayerDataAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        yield return new object?[]
        {
            new object[] { new Dictionary<string, object?> { ["Key1"] = "value 1", ["Key2"] = "value 2" } },
            new object?[] { "Key1", "Key2" }
        };
    }

    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => "one layer";
}

internal sealed class MultipleLayerDataAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        yield return new object?[]
        {
            new object[]
            {
                new Dictionary<string, object?> { ["Key1"] = "value 1", ["Key2"] = "value 2" },
                new Dictionary<string, object?> { ["Key2"] = "new value 2", ["Key3"] = "value 3" },
                new { Key3 = "new value 3" }
            },
            new object?[]{ "Key1", "Key2", "Key3"}
        };
    }

    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => "multiple layer";
}

[TestClass]
public class TheKeysProperty
{

    [DataTestMethod]
    [NoLayerData]
    [OneLayerData]
    [MultipleLayerData]
    public void ShouldProvideTheDistnictSetOfKeys(IEnumerable<object> layers, IEnumerable<object?> keys)
    {
        // Arrange
        ConfigurationCollection configuration = [];

        // Act
        foreach (object layer in layers)
        {
            configuration.Push(layer);
        }

        // Assert
        configuration.Keys.Should().BeEquivalentTo(keys);
    }
}
