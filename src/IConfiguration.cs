using System;
using System.Collections;
using System.Collections.Generic;

namespace WB.AppConfiguration;

/// <summary>
/// A collection of configurations oranized in layers and accessible by key.
/// </summary>
/// <remarks>
/// <para>
/// The configurations are arranged in layers like a stack. The top-most layer has the highest priority.
/// </para>
/// <para>
/// Each layer that is pushed onto the configuration can be a <see cref="IDictionary{TKey, TValue}"/> or an <see cref="object"/> with properties.
/// </para>
/// <para>
/// If there are layers with the same key, the value of the top-most layer is returned.
/// </para>
/// </remarks>
public interface IConfiguration : IReadOnlyDictionary<object, object?>
{
    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Public Properties                                                              │
    // └────────────────────────────────────────────────────────────────────────────────┘

    /// <summary>
    /// Gets the list of layers in the configuration.
    /// </summary>
    public IEnumerable<object> Layers { get; }

    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Public Methods                                                                 │
    // └────────────────────────────────────────────────────────────────────────────────┘

    /// <summary>
    /// Pushes the <paramref name="configurationLayer"/> onto the configuration.
    /// </summary>
    /// <param name="configurationLayer">The configuration layer to push onto the configuration.</param>
    /// <returns>A disposable that pops the <paramref name="configurationLayer"/> from the configuration.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configurationLayer"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown when the <paramref name="configurationLayer"/> is a <see cref="IList"/>.</exception>
    public IDisposable Push(object configurationLayer);

    /// <summary>
    /// Pops and returns the top-most layer from the configuration.
    /// </summary>
    /// <returns>The top-most layer from the configuration.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration is empty.</exception>
    public object Pop();

    /// <summary>
    /// Removes the <paramref name="configurationLayer"/> from the configuration.
    /// </summary>
    /// <param name="configurationLayer">The configuration layer to remove from the configuration.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configurationLayer"/> is <see langword="null"/>.</exception>
    public void Remove(object configurationLayer);
}
