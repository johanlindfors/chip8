// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8;

/// <summary>
/// Implementation of random number generator.
/// </summary>
public class RandomNumberGenerator
    : IRandomNumberGenerator
{
    private readonly Random random;

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomNumberGenerator"/> class.
    /// </summary>
    public RandomNumberGenerator()
    {
        this.random = new Random();
    }

    /// <summary>
    /// The actual generator.
    /// </summary>
    /// <returns>A value between 0 and 255.</returns>
    public byte NextByte()
    {
        return (byte)(this.random.Next() % 256);
    }
}
