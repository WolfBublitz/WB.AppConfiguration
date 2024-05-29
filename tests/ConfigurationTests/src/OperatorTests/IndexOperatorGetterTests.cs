using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualBasic;
using WB.AppConfiguration;

namespace ConfigurationTests.OperatorTests.IndexOperatorGetterTests;

internal sealed class OneLayerDataAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        yield return new object?[]
        {
            new object[] { new Dictionary<string, object?> { ["Key1"] = "value 1", ["Key2"] = "value 2" } },
            "Key1",
            "value 1"
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
            "Key2",
            "new value 2"
        };
    }

    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => "multiple layer";
}

[TestClass]
public class TheGetterOfTheIndexOperator
{

    [DataTestMethod]
    [OneLayerData]
    [MultipleLayerData]
    public void ShouldReturnTheTopMonstValueForAKey(IEnumerable<object> layers, string key, object? expectedValue)
    {
        // Arrange
        ConfigurationCollection configuration = new();

        foreach (object layer in layers)
        {
            configuration.Push(layer);
        }

        // Act
        object? actualValue = configuration[key];

        // Assert
        actualValue.Should().Be(expectedValue);
    }

    [TestMethod]
    public void ShouldThrowArgumentNullExceptionIfKeyIsNull()
    {
        // Arrange
        ConfigurationCollection configuration = [];

        // Act
        Action action = () =>
        {
            object? value = configuration[null!];
        };

        // Assert
        action.Should().Throw<ArgumentNullException>();
    }

    [TestMethod]
    public void ShouldThrowKeyNotFoundExceptionWhenKeyIsNotFound()
    {
        // Arrange
        ConfigurationCollection configuration = [];

        // Act
        Action action = () =>
        {
            object? value = configuration["Key"];
        };

        // Assert
        action.Should().Throw<KeyNotFoundException>();
    }
}
