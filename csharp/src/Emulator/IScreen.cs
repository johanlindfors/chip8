// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8;

/// <summary>
/// The interface for a screen implementation.
/// </summary>
public interface IScreen
{
    /// <summary>
    /// Gets the width of the screen.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Gets the height of the screen.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Clears the screen.
    /// </summary>
    void Clear();

    /// <summary>
    /// Sets the pixel a the specified coordinate.
    /// </summary>
    /// <param name="xCoord">The x-coordinate.</param>
    /// <param name="yCoord">The y-coordinate.</param>
    void SetPixel(int xCoord, int yCoord);

    /// <summary>
    /// Gets the pixel a the specified coordinate.
    /// </summary>
    /// <param name="xCoord">The x-coordinate.</param>
    /// <param name="yCoord">The y-coordinate.</param>
    /// <returns>The value at the coordinate.</returns>
    uint GetPixel(int xCoord, int yCoord);
}
