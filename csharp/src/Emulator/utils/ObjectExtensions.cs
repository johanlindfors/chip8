// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8.Emulator;

/// <summary>
/// Extensions for objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// When functionality.
    /// </summary>
    /// <typeparam name="T">Type to contain.</typeparam>
    /// <param name="obj">Instance.</param>
    /// <param name="condition">Condition.</param>
    /// <returns>Option of type.</returns>
    public static Option<T> When<T>(this T obj, bool condition) =>
        condition ? (Option<T>)new Some<T>(obj) : None.Value;

    /// <summary>
    /// When functionality.
    /// </summary>
    /// <typeparam name="T">Type to contain.</typeparam>
    /// <param name="obj">Instance.</param>
    /// <param name="predicate">Predicate function.</param>
    /// <returns>Option of type.</returns>
    public static Option<T> When<T>(this T obj, Func<T, bool> predicate) =>
        obj.When(predicate(obj));

    /// <summary>
    /// None if null.
    /// </summary>
    /// <typeparam name="T">Type contained.</typeparam>
    /// <param name="obj">Instance.</param>
    /// <returns>Option of type.</returns>
    public static Option<T> NoneIfNull<T>(this T obj) =>
        obj.When(!object.ReferenceEquals(obj, null));
}
