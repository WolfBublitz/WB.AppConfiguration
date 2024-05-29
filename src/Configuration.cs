using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace WB.AppConfiguration
{
    /// <summary>
    /// A layered configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The configurations are arranged in layers like a stack. The top-most layer has the highest priority.
    /// </para>
    /// <para>
    /// Each layer that is pushed onto the configuration can be a <see cref="IDictionary"/> or an object with properties.
    /// </para>
    /// <para>
    /// If there are layers with the same key, the value of the top-most layer is returned.
    /// </para>
    /// </remarks>
    public sealed class Configuration : IConfiguration
    {
        // ┌────────────────────────────────────────────────────────────────────────────────┐
        // │ Private Fields                                                                 │
        // └────────────────────────────────────────────────────────────────────────────────┘
        private readonly Dictionary<object, IDisposable> configurationLayers = [];

        private readonly Dictionary<object, List<Func<object?>>> valueProviders = [];

        // ┌────────────────────────────────────────────────────────────────────────────────┐
        // │ Public Indexers                                                                │
        // └────────────────────────────────────────────────────────────────────────────────┘

        /// <summary>
        /// Gets or sets the value associated with the specified <paramref name="key"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the <paramref name="key"/> is not found, a <see cref="KeyNotFoundException"/> is thrown.
        /// </para>
        /// </remarks>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified <paramref name="key"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when the <paramref name="key"/> is not found in the configuration.</exception>
        public object? this[object key]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(key, nameof(key));

                if (TryGetValue(key, out object? value))
                {
                    return value;
                }
                else
                {
                    throw new KeyNotFoundException($"The key '{key}' was not found in the configuration.");
                }
            }
        }

        // ┌────────────────────────────────────────────────────────────────────────────────┐
        // │ Public Properties                                                              │
        // └────────────────────────────────────────────────────────────────────────────────┘

        /// <summary>
        /// Gets the keys the distinct set of keys in the <see cref="Configuration"/> over all layers.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The keys are ordered alphabetically.
        /// </para>
        /// </remarks>
        public IEnumerable<object> Keys => valueProviders.Keys;

        /// <summary>
        /// Gets the top-level values in the <see cref="Configuration"/> for all keys.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are ordered alphabetically by key.
        /// </para>
        /// </remarks>
        public IEnumerable<object?> Values
        {
            get
            {
                foreach (object key in Keys)
                {
                    List<Func<object?>> valueProvider = valueProviders[key];

                    yield return valueProvider.Last()();
                }
            }
        }

        /// <inheritdoc/>
        public int Count => valueProviders.Keys.Count;

        /// <summary>
        /// Gets the list of layers in the configuration.
        /// </summary>
        public IEnumerable<object> Layers => configurationLayers.Keys;

        // ┌────────────────────────────────────────────────────────────────────────────────┐
        // │ Public Methods                                                                 │
        // └────────────────────────────────────────────────────────────────────────────────┘

        /// <inheritdoc/>
        public bool ContainsKey(object key)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));

            return valueProviders.ContainsKey(key);
        }

        /// <inheritdoc/>
        public bool TryGetValue(object key, [MaybeNullWhen(false)] out object? value)
        {
            if (valueProviders.TryGetValue(key, out List<Func<object?>>? valueProvider))
            {
                value = valueProvider.Last()()!;

                return true;
            }
            else
            {
                value = default;

                return false;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<object, object?>> GetEnumerator()
        {
            foreach (object key in Keys)
            {
                List<Func<object?>> valueProvider = valueProviders[key];

                yield return new KeyValuePair<object, object?>(key, valueProvider.Last()());
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configurationLayer"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="configurationLayer"/> is a <see cref="IList"/>.</exception>
        public IDisposable Push(object configurationLayer)
        {
            ArgumentNullException.ThrowIfNull(configurationLayer, nameof(configurationLayer));

            DisposableCollection disposableCollection = new();

            if (configurationLayer is IDictionary dictionary)
            {
                foreach (object key in dictionary.Keys)
                {
                    object? valueProvider() => dictionary[key];

                    AddValueProvider(key, valueProvider);

                    disposableCollection.Add(new ActionDisposer(GetRemoveAction(key, valueProvider)));
                }
            }
            else if (configurationLayer is IList list)
            {
                throw new NotSupportedException("Lists are not supported as configuration layers.");
            }
            else
            {
                foreach (PropertyInfo property in configurationLayer.GetType().GetProperties())
                {
                    string key = property.Name;

                    object? valueProvider() => property.GetValue(configurationLayer);

                    AddValueProvider(key, valueProvider);

                    disposableCollection.Add(new ActionDisposer(GetRemoveAction(key, valueProvider)));
                }
            }

            disposableCollection.Add(new ActionDisposer(() => configurationLayers.Remove(configurationLayer)));

            configurationLayers[configurationLayer] = disposableCollection;

            return disposableCollection;
        }

        /// <inheritdoc/>
        /// <exception cref="InvalidOperationException">Thrown when the configuration is empty.</exception>
        public object Pop()
        {
            if (configurationLayers.Count == 0)
            {
                throw new InvalidOperationException("The configuration is empty.");
            }

            KeyValuePair<object, IDisposable> topLayer = configurationLayers.Last();

            topLayer.Value.Dispose();

            return topLayer.Key;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configurationLayer"/> is <see langword="null"/>.</exception>
        public void Remove(object configurationLayer)
        {
            ArgumentNullException.ThrowIfNull(configurationLayer, nameof(configurationLayer));

            configurationLayers[configurationLayer].Dispose();
        }

        // ┌────────────────────────────────────────────────────────────────────────────────┐
        // │ Private Methods                                                                │
        // └────────────────────────────────────────────────────────────────────────────────┘
        private void AddValueProvider(object key, Func<object?> valueProvider)
        {
            if (valueProviders.TryGetValue(key, out List<Func<object?>>? value))
            {
                value.Add(valueProvider);
            }
            else
            {
                valueProviders[key] = [];
                valueProviders[key].Add(valueProvider);
            }
        }

        private Action GetRemoveAction(object key, Func<object?> valueProvider)
        {
            return () =>
            {
                valueProviders[key].Remove(valueProvider);

                if (valueProviders[key].Count == 0)
                {
                    valueProviders.Remove(key);
                }
            };
        }
    }
}
