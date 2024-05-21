// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8;

/// <summary>
/// Interface for random.
/// </summary>
public interface IRandomNumberGenerator
{
    /// <summary>
    /// Generate a value between 0 and 255.
    /// </summary>
    /// <returns>Value between 0 and 255.</returns>
    byte NextByte();
}
