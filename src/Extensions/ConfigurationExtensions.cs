using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TB.ComponentModel;

namespace WB.AppConfiguration;

/// <summary>
/// Provides extension methods for the <see cref="ConfigurationCollection"/> class.
/// </summary>
public static class ConfigurationExtensions
{
    // ┌────────────────────────────────────────────────────────────────────────────────┐
    // │ Public Methods                                                                 │
    // └────────────────────────────────────────────────────────────────────────────────┘

    /// <summary>
    /// Tries to get a value associated with the specified <paramref name="key"/> and
    /// returns it as <paramref name="value"/> of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method delivers the value via the out parameter <paramref name="value"/> and
    /// returns <see langword="true"/> if the <paramref name="key"/> was found in the configuration.
    /// </para>
    /// <para>
    /// This method returns <see langword="false"/> if the <paramref name="key"/> is not found in the configuration. The
    /// <paramref name="value"/> parameter is set to the default value of <typeparamref name="TValue"/>.
    /// </para>
    /// </remarks>
    /// <param name="this">The configuration to get the value from.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">The value associated with the specified <paramref name="key"/>.</param>
    /// <typeparam name="TValue">The <see cref="Type"/> of the value to get.</typeparam>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="key"/> was found in the configuration;otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="this"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value for the <paramref name="key"/> could not be converted to the target type <typeparamref name="TValue"/>.</exception>
    /// <seealso cref="TryGetValue(ConfigurationCollection, string, Type, out object?)"/>
    public static bool TryGetValue<TValue>(this ConfigurationCollection @this, string key, out TValue value)
    {
        ArgumentNullException.ThrowIfNull(@this, nameof(@this));

        if (@this.TryGetValue(key, out object? value_))
        {
            try
            {
                value = value_!.To<TValue>();
            }
            catch (InvalidConversionException invalidConversionException)
            {
                throw new InvalidCastException($"The value '{value_}' for the key '{key}' which is of type {value_!.GetType()} could not be converted to the target type '{typeof(TValue)}'.", invalidConversionException);
            }

            return true;
        }
        else
        {
            value = default!;

            return false;
        }
    }

    /// <summary>
    /// Tries to get a value associated with the specified <paramref name="key"/> and
    /// returns it as <paramref name="value"/> of type <paramref name="type"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method delivers the value via the out parameter <paramref name="value"/> and
    /// returns <see langword="true"/> if the <paramref name="key"/> was found in the configuration.
    /// </para>
    /// <para>
    /// This method returns <see langword="false"/> if the <paramref name="key"/> is not found in the configuration. The
    /// <paramref name="value"/> parameter is set to <see langword="null"/>.
    /// </para>
    /// </remarks>
    /// <param name="this">The configuration to get the value from.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="type">The <see cref="Type"/> of the value to get.</param>
    /// <param name="value">The value associated with the specified <paramref name="key"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="key"/> was found in the configuration;otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="this"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">Thrown when the value for the <paramref name="key"/> could not be converted to the target type <paramref name="type"/>.</exception>
    /// <seealso cref="TryGetValue{TValue}(ConfigurationCollection, string, out TValue)"/>
    public static bool TryGetValue(this ConfigurationCollection @this, string key, Type type, out object? value)
    {
        ArgumentNullException.ThrowIfNull(@this, nameof(@this));

        if (@this.TryGetValue(key, out object? value_))
        {
            try
            {
                value = value_!.To(type);
            }
            catch (InvalidConversionException invalidConversionException)
            {
                throw new InvalidCastException($"The value '{value_}' for the key '{key}' which is of type {value_!.GetType()} could not be converted to the target type '{type}'.", invalidConversionException);
            }

            return true;
        }
        else
        {
            value = null;

            return false;
        }
    }

    /// <summary>
    /// Gets the value associated with the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="this">The configuration to get the value from.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified <paramref name="key"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="this"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the <paramref name="key"/> is not found in the configuration.</exception>
    /// <seealso cref="GetValue(ConfigurationCollection, string, Type)"/>
    /// <seealso cref="GetValue{TValue}(ConfigurationCollection, string)"/>
    public static object? GetValue(this ConfigurationCollection @this, string key)
    {
        ArgumentNullException.ThrowIfNull(@this, nameof(@this));

        return @this[key];
    }

    /// <summary>
    /// Gets the value associated with the specified <paramref name="key"/> converted to <paramref name="type"/>.
    /// </summary>
    /// <param name="this">The configuration to get the value from.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="type">The <see cref="Type"/> of the value to get.</param>
    /// <returns>The value associated with the specified <paramref name="key"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="this"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the <paramref name="key"/> is not found in the configuration.</exception>
    /// <seealso cref="GetValue(ConfigurationCollection, string, Type)"/>
    /// <seealso cref="GetValue{TValue}(ConfigurationCollection, string)"/>
    public static object? GetValue(this ConfigurationCollection @this, string key, Type type)
    {
        ArgumentNullException.ThrowIfNull(@this, nameof(@this));

        object? value = @this[key];

        if (value is null)
        {
            return null;
        }

        try
        {
            return value.To(type);
        }
        catch (InvalidConversionException invalidConversionException)
        {
            throw new InvalidCastException($"The value '{@this[key]}' for the key '{key}' which is of type {value.GetType()} could not be converted to the target type '{type}'.", invalidConversionException);
        }
    }

    /// <summary>
    /// Gets the value associated with the specified <paramref name="key"/> converted to <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The <see cref="Type"/> of the value to get.</typeparam>
    /// <param name="this">The configuration to get the value from.</param>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified <paramref name="key"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="this"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the <paramref name="key"/> is not found in the configuration.</exception>
    /// <seealso cref="GetValue(ConfigurationCollection, string, Type)"/>
    /// <seealso cref="GetValue{TValue}(ConfigurationCollection, string)"/>
    public static TValue GetValue<TValue>(this ConfigurationCollection @this, string key)
    {
        ArgumentNullException.ThrowIfNull(@this, nameof(@this));

        object? value = @this[key] ?? throw new InvalidCastException($"The value '{@this[key]}' for the key '{key}' is null and cannot be converted to the target type '{typeof(TValue)}'.");

        try
        {
            return value.To<TValue>();
        }
        catch (InvalidConversionException invalidConversionException)
        {
            throw new InvalidCastException($"The value '{@this[key]}' for the key '{key}' which is of type {value.GetType()} could not be converted to the target type '{typeof(TValue)}'.", invalidConversionException);
        }
    }

    /// <summary>
    /// Gets a new instance of <typeparamref name="TTarget"/> populated with the values from the <see cref="ConfigurationCollection"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method creates a new instance of <typeparamref name="TTarget"/> and populates all public writable properties
    /// with the values from the <see cref="ConfigurationCollection"/>. If the property type does not match the value type
    /// in the <see cref="ConfigurationCollection"/> the value is converted.
    /// An <see cref="InvalidCastException"/> is thrown if the value could not be converted.
    /// </para>
    /// </remarks>
    /// <typeparam name="TTarget">The type of the target object to get.</typeparam>
    /// <param name="this">The configuration to get the values from.</param>
    /// <returns>A new instance of <typeparamref name="TTarget"/> populated with the values from the <see cref="ConfigurationCollection"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="this"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">Thrown when a value could not be converted to the target type.</exception>
    public static TTarget Get<TTarget>(this ConfigurationCollection @this)
        where TTarget : new()
    {
        ArgumentNullException.ThrowIfNull(@this, nameof(@this));

        TTarget target = new();

        foreach (PropertyInfo property in typeof(TTarget).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(pi => pi.CanWrite))
        {
            if (@this.TryGetValue(property.Name, out object? sourceValue))
            {
                // if the target and source type matches in can be set without conversion
                if (sourceValue!.GetType() == property.PropertyType)
                {
                    property.SetValue(target, sourceValue);
                }

                // otherwise it must be converted
                else
                {
                    try
                    {
                        property.SetValue(target, sourceValue!.To(property.PropertyType));
                    }
                    catch (InvalidConversionException invalidConversionException)
                    {
                        throw new InvalidCastException($"The value '{sourceValue}' for the key '{property.Name}' which is of type {sourceValue.GetType()} could not be converted to the target type '{property.PropertyType}'.", invalidConversionException);
                    }
                }
            }
        }

        return target;
    }
}
