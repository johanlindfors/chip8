// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8;

/// <summary>
/// Implementation of the registers.
/// </summary>
public class Register
{
    private readonly byte[] v = new byte[16];

    /// <summary>
    /// Initializes a new instance of the <see cref="Register"/> class.
    /// </summary>
    public Register()
    {
        for (int i = 0; i < 16; i++)
        {
            this.v[i] = 0x00;
        }
    }

    /// <summary>
    /// Gets the register of index.
    /// </summary>
    /// <param name="index">The register.</param>
    /// <returns>The data.</returns>
    public byte Get(int index) => this.v[index];

    /// <summary>
    /// Sets the register.
    /// </summary>
    /// <param name="index">The register.</param>
    /// <param name="value">The data.</param>
    public void Set(int index, byte value)
    {
        this.v[index] = value;
    }

    /// <summary>
    /// Applies a function to a register.
    /// </summary>
    /// <param name="index">The register.</param>
    /// <param name="func">The function.</param>
    public void Apply(int index, Func<byte, byte> func)
    {
        this.v[index] = func(this.v[index]);
    }
}
