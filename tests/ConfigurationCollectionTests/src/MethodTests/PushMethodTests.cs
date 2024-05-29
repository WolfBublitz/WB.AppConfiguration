using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using WB.AppConfiguration;

namespace ConfigurationCollectionTests.MethodTests.PushMethodTests;

internal sealed class DictionaryDataTest : Attribute, ITestDataSource
{
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        yield return new object?[]
        {
            (IDictionary)new Dictionary<string, object?> { { "Key1", "Value1" } },
        };
    }

    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => $"{nameof(IDictionary)} test";
}

internal sealed class ObjectDataTest : Attribute, ITestDataSource
{
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        yield return new object?[]
        {
            new { Key1 = "Value1" },
        };
    }

    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data)
        => $"object test";
}


[TestClass]
public class ThePushMethod
{
    [DataTestMethod]
    [DictionaryDataTest]
    public void ShouldPushOnObject(object @object)
    {
        // Arrange
        ConfigurationCollection configuration = new();

        // Act
        configuration.Push(@object);

        // Assert
        configuration["Key1"].Should().Be("Value1");
    }

    [TestMethod]
    public void ShouldThrowArgumentNullExceptionIfObjectIsNull()
    {
        // Arrange
        ConfigurationCollection configuration = new();

        // Act
        Action act = () => configuration.Push(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();

    }

    [TestMethod]
    public void ShouldThrowNotSupportedExeptionIfLayerIsAnIList()
    {
        // Arrange
        ConfigurationCollection configuration = new();

        // Act
        Action act = () => configuration.Push(new List<int>());

        // Assert
        act.Should().Throw<NotSupportedException>();

    }

    [TestMethod]
    public void ShouldReturnAnIDisposable()
    {
        // Arrange
        ConfigurationCollection configuration = new();

        // Act
        IDisposable disposable = configuration.Push(new { Key1 = "Value1" });

        // Assert
        disposable.Should().NotBeNull();
    }

    [TestMethod]
    public void ShouldReturnAnIDisposableThatRemovesTheLayerOnDispose()
    {
        // Arrange
        ConfigurationCollection configuration = new();

        // Act
        IDisposable disposable1 = configuration.Push(new { Key1 = "Value1" });
        IDisposable disposable2 = configuration.Push(new { Key1 = "Value2" });

        // Assert
        configuration["Key1"].Should().Be("Value2");
        configuration.Layers.Should().HaveCount(2);

        // Act
        disposable2.Dispose();

        // Assert
        configuration["Key1"].Should().Be("Value1");
        configuration.Layers.Should().ContainSingle();

        // Act
        disposable1.Dispose();

        // Assert
        configuration.Should().BeEmpty();
        configuration.Layers.Should().BeEmpty();
    }
}
