// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

public static class ObjectExtensions
{
    public static Option<T> When<T>(this T obj, bool condition) =>
        condition ? (Option<T>)new Some<T>(obj) : None.Value;

    public static Option<T> When<T>(this T obj, Func<T, bool> predicate) =>
        obj.When(predicate(obj));

    public static Option<T> NoneIfNull<T>(this T obj) =>
        obj.When(!object.ReferenceEquals(obj, null));
}