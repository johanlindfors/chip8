// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8;

/// <summary>
/// The interface for a keyboard implementation.
/// </summary>
public interface IKeyboard
{
    /// <summary>
    /// Event triggered on key pressed.
    /// </summary>
    /// <param name="keyCode">The key pressed.</param>
    void OnKeyPressed(int keyCode);

    /// <summary>
    /// Event triggered on key released.
    /// </summary>
    /// <param name="keyCode">The key released.</param>
    void OnKeyReleased(int keyCode);

    /// <summary>
    /// Returns if a key is pressed.
    /// </summary>
    /// <param name="keyCode">The key to check if pressed.</param>
    /// <returns>If the key is pressed.</returns>
    bool IsPressed(int keyCode);

    /// <summary>
    /// Returns all the keys.
    /// </summary>
    /// <returns>The boolean values of all the keys.</returns>
    bool[] GetKeys();
}
