// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8;

/// <summary>
/// Some utilities.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Load a file as memory bytes.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <returns>An array of bytes.</returns>
    public static Option<byte[]> Load(string filename)
    {
        try
        {
            return new Some<byte[]>(File.ReadAllBytes(filename));
        }
        catch (Exception)
        {
            return None.Value;
        }
    }

    /// <summary>
    /// Bit shifts a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="bitIndex">Bits to shift.</param>
    /// <returns>The returned value.</returns>
    public static int GetBitValue(int value, int bitIndex) => value & (0x80 >> bitIndex);
}
