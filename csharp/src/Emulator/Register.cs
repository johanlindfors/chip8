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
    /// Gets or sets the register value.
    /// </summary>
    /// <param name="index">Which register.</param>
    /// <returns>Return value.</returns>
    public byte this[int index]
    {
        get { return this.v[index]; }
        set { this.v[index] = value; }
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
