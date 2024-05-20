// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8.Emulator;

/// <summary>
/// A generic option implementation.
/// </summary>
/// <typeparam name="T">Internal representation.</typeparam>
public abstract class Option<T>
{
    /// <summary>
    /// Gets a new Some of type.
    /// </summary>
    /// <param name="value">Value to be contained.</param>
    public static implicit operator Option<T>(T value) =>
        new Some<T>(value);

    /// <summary>
    /// Get a new None of type.
    /// </summary>
    /// <param name="none">Value to be contained.</param>
    public static implicit operator Option<T>(None none) =>
        new None<T>();

    /// <summary>
    /// Map functionality.
    /// </summary>
    /// <typeparam name="TResult">The result.</typeparam>
    /// <param name="map">The map function.</param>
    /// <returns>An option of result.</returns>
    public abstract Option<TResult> Map<TResult>(Func<T, TResult> map);

    /// <summary>
    /// Map functionality.
    /// </summary>
    /// <typeparam name="TResult">The result.</typeparam>
    /// <param name="map">The map function.</param>
    /// <returns>An option of result.</returns>
    public abstract Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map);

    /// <summary>
    /// Reduce functionality.
    /// </summary>
    /// <param name="whenNone">Type to return when none.</param>
    /// <returns>Return type.</returns>
    public abstract T Reduce(T whenNone);

    /// <summary>
    /// Reduce functionality.
    /// </summary>
    /// <param name="whenNone">Type to return when none.</param>
    /// <returns>Return type.</returns>
    public abstract T Reduce(Func<T> whenNone);
}

/// <summary>
/// Some implementation.
/// </summary>
/// <typeparam name="T">Type contained.</typeparam>
public sealed class Some<T>(T value)
    : Option<T>
{
    /// <summary>
    /// Gets the actual content.
    /// </summary>
    public T Content { get; } = value;

    /// <summary>
    /// Gets the Content.
    /// </summary>
    /// <param name="some">Some to be contained.</param>
    public static implicit operator T(Some<T> some) =>
        some.Content;

    /// <summary>
    /// Map functionality.
    /// </summary>
    /// <typeparam name="TResult">Type to return.</typeparam>
    /// <param name="map">Map function.</param>
    /// <returns>An option of type.</returns>
    public override Option<TResult> Map<TResult>(Func<T, TResult> map) =>
        map(this.Content);

    /// <summary>
    /// Map functionality.
    /// </summary>
    /// <typeparam name="TResult">Type to return.</typeparam>
    /// <param name="map">Map function.</param>
    /// <returns>An option of type.</returns>
    public override Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) =>
        map(this.Content);

    /// <summary>
    /// Reduce functionality.
    /// </summary>
    /// <param name="whenNone">Type to return when none.</param>
    /// <returns>Returned type.</returns>
    public override T Reduce(T whenNone) =>
        this.Content;

    /// <summary>
    /// Reduce functionality.
    /// </summary>
    /// <param name="whenNone">Type to return when none.</param>
    /// <returns>Returned type.</returns>
    public override T Reduce(Func<T> whenNone) =>
        this.Content;
}

/// <summary>
/// None implementation.
/// </summary>
/// <typeparam name="T">Type to be contained.</typeparam>
public sealed class None<T> : Option<T>
{
    /// <summary>
    /// Map functionality.
    /// </summary>
    /// <typeparam name="TResult">Type contained.</typeparam>
    /// <param name="map">Map function.</param>
    /// <returns>An option of type.</returns>
    public override Option<TResult> Map<TResult>(Func<T, TResult> map) =>
        None.Value;

    /// <summary>
    /// Map functionality.
    /// </summary>
    /// <typeparam name="TResult">Type contained.</typeparam>
    /// <param name="map">Map function.</param>
    /// <returns>An option of type.</returns>
    public override Option<TResult> MapOptional<TResult>(Func<T, Option<TResult>> map) =>
        None.Value;

    /// <summary>
    /// Reduce functionality.
    /// </summary>
    /// <param name="whenNone">Type when none.</param>
    /// <returns>Type returned.</returns>
    public override T Reduce(T whenNone) =>
        whenNone;

    /// <summary>
    /// Reduce functionality.
    /// </summary>
    /// <param name="whenNone">Type when none.</param>
    /// <returns>Type returned.</returns>
    public override T Reduce(Func<T> whenNone) =>
        whenNone();
}

/// <summary>
/// None implementation.
/// </summary>
public sealed class None()
{
    /// <summary>
    /// Gets an empty none.
    /// </summary>
    public static None Value { get; } = new None();
}
