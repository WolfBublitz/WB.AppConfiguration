using System;
using System.Collections.Generic;
using System.Reflection;
using TB.ComponentModel;
using WB.AppConfiguration;

namespace ConfigurationTests.ExtensionMethodTests.Get_TValue_MethodTests;

public class TestConfiguration
{
    public string? StringProperty { get; set; }

    public int IntProperty { get; set; }

    public bool BoolProperty { get; set; }

    public IReadOnlyList<string>? StringListProperty { get; set; }
}

internal sealed class EmptyConfigurationDataAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        yield return new object[]
        {
            Array.Empty<object>(),
            new TestConfiguration(),
        };
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => "Empty configuration";
}

internal sealed class SimpleTypesDataAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        yield return new object[]
        {
            new object[]
            {
                new Dictionary<string, object?>
                {
                    { "StringProperty", "Some Value" },
                    { "IntProperty", 42 },
                    { "BoolProperty", true },
                },
            },
            new TestConfiguration()
            {
                StringProperty = "Some Value",
                IntProperty = 42,
                BoolProperty = true,
            },
        };
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => "Configuration with simple types only";
}

internal sealed class SimpleTypesWithConversionDataAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        yield return new object[]
        {
            new object[]
            {
                new Dictionary<string, object?>
                {
                    { "StringProperty", 42 },
                    { "IntProperty", "42" },
                    { "BoolProperty", "true" },
                },
            },
            new TestConfiguration()
            {
                StringProperty = "42",
                IntProperty = 42,
                BoolProperty = true,
            },
        };
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => "Configuration with simple types that must be converted";
}

internal sealed class IReadOnlyListFromArrayDataAttribute : Attribute, ITestDataSource
{
    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        yield return new object[]
        {
            new object[]
            {
                new Dictionary<string, object?>
                {
                    { "StringListProperty", new string[] { "Value1", "Value2" } },
                },
            },
            new TestConfiguration()
            {
                StringListProperty = ["Value1", "Value2"],
            },
        };
    }

    public string GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => "Configuration IReadOnlyList<string> property";
}

[TestClass]
public class TheGetMethod
{
    [DataTestMethod]
    [EmptyConfigurationData]
    [SimpleTypesData]
    [SimpleTypesWithConversionData]
    public void ShouldReturnTheConfigurationValue(IEnumerable<object> layers, TestConfiguration expectedConfiguration)
    {
        // Arrange
        ConfigurationCollection configuration = new();
        configuration.Push(new { Key = "Some Value" });

        foreach (object layer in layers)
        {
            configuration.Push(layer);
        }

        // Act
        TestConfiguration testConfiguration = configuration.Get<TestConfiguration>();

        // Assert
        testConfiguration.Should().BeEquivalentTo(expectedConfiguration);
    }

    [TestMethod]
    public void ShouldThrowInvalidCastExceptionIfTheValueCouldNotBeConverted()
    {
        // Arrange
        ConfigurationCollection configuration = new();
        configuration.Push(new { BoolProperty = "Some Value" });

        // Act
        Action act = () => configuration.Get<TestConfiguration>();

        // Assert
        act.Should().Throw<InvalidCastException>().WithInnerException<InvalidConversionException>(because: "The value 'Some Value' could not be converted to a boolean.");
    }
}
