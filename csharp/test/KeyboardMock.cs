// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8.Tests;

/// <summary>
/// Mock implementation of a keyboard module.
/// </summary>
public class KeyboardMock : IKeyboard
{
    /// <summary>
    /// Gets all keys.
    /// </summary>
    /// <returns>All keys.</returns>
    public bool[] GetKeys()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks if key is pressed.
    /// </summary>
    /// <param name="index">Which key.</param>
    /// <returns>Is pressed.</returns>
    public bool IsPressed(int index)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Trigger an OnKeyPressed evejt.
    /// </summary>
    /// <param name="keyCode">With key.</param>
    public void OnKeyPressed(int keyCode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Trigger an OnKeyReleased event.
    /// </summary>
    /// <param name="keyCode">With key.</param>
    public void OnKeyReleased(int keyCode)
    {
        throw new NotImplementedException();
    }
}
