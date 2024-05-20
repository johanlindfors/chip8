// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8;

/// <summary>
/// The implementation of the memory module.
/// </summary>
public class Memory
{
    private byte[] memory = new byte[4096];

    /// <summary>
    /// Initializes a new instance of the <see cref="Memory"/> class.
    /// </summary>
    public Memory()
    {
        System.Array.Copy(Keyboard.FONTS, 0, this.memory, 0x50, 80);
    }

    /// <summary>
    /// Loads the memory with an array of data.
    /// </summary>
    /// <param name="data">Data to load.</param>
    public void LoadData(byte[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            this.SetByte(i + 512, (byte)(data[i] & 0xFF));
        }
    }

    /// <summary>
    /// Gets a byte of data from memory.
    /// </summary>
    /// <param name="index">Index of byte.</param>
    /// <returns>Data returned.</returns>
    public byte GetByte(int index)
    {
        return this.memory[index];
    }

    /// <summary>
    /// Sets a byte of data.
    /// </summary>
    /// <param name="index">Which byte.</param>
    /// <param name="value">What data.</param>
    public void SetByte(int index, byte value)
    {
        this.memory[index] = value;
    }

    /// <summary>
    /// Returns the size of the memory.
    /// </summary>
    /// <returns>Size of memory.</returns>
    public int Size() => this.memory.Length;

    /// <summary>
    /// Get data from memory as opcode.
    /// </summary>
    /// <param name="pc">The program counter.</param>
    /// <returns>Opcode returned.</returns>
    public int GetOpcode(int pc) => this.GetByte(pc) << 8 | this.GetByte(pc + 1);
}
