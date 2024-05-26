// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8.Tests;

/// <summary>
/// Mock implementation of the screen.
/// </summary>
public class ScreenMock : IScreen
{
    /// <summary>
    /// Gets the width.
    /// </summary>
    public int Width => throw new NotImplementedException();

    /// <summary>
    /// Gets the height.
    /// </summary>
    public int Height => throw new NotImplementedException();

    /// <summary>
    /// Clears the screen.
    /// </summary>
    public void Clear()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks the pixel at coordinate.
    /// </summary>
    /// <param name="xCoord">X.</param>
    /// <param name="yCoord">Y.</param>
    /// <returns>Value.</returns>
    public uint GetPixel(int xCoord, int yCoord)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Enables the pixel at coordinate.
    /// </summary>
    /// <param name="xCoord">X.</param>
    /// <param name="yCoord">Y.</param>
    public void SetPixel(int xCoord, int yCoord)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Sets the draw flag.
    /// </summary>
    public void SetDrawFlag()
    {
        throw new NotImplementedException();
    }
}
