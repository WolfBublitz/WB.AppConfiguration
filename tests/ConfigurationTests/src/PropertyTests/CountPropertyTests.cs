using System;
using System.Collections.Generic;
using System.Reflection;
using WB.AppConfiguration;

namespace ConfigurationTests.PropertyTests.CountPropertyTests;

internal sealed class NoLayerDataAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        yield return new object?[] { Array.Empty<object>(), 0 };
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
            new object[]
            {
                new Dictionary<string, object?> { {"Key1", "Value1"} },
            },
            1,
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
                new Dictionary<string, object?> { { "Key1", "Value1" }, { "Key2", "Value2" } },
                new Dictionary<string, object?> { { "Key1", "Value1" }, { "Key3", "Value3" } },
                new { Key3 = "Value3"}
            },
            3,
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
    public void ShouldCountTheNumberOfKeys(IEnumerable<object> layers, int expectedCount)
    {
        // Arrange
        ConfigurationCollection configuration = [];

        // Act
        foreach (object layer in layers)
        {
            configuration.Push(layer);
        }

        // Assert
        configuration.Count.Should().Be(expectedCount);
    }
}
