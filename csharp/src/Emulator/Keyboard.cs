// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8;

/// <summary>
/// A keyboard implementation.
/// </summary>
public class Keyboard : IKeyboard
{
    /// <summary>
    /// Array that builds the current font.
    /// </summary>
    public static readonly byte[] FONTS =
    {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0      0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1      5
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2     10
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3     15
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4     20
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5     25
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6     30
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7     35
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8     40
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80, // F
    };

    private bool[] keys = new bool[16];

    /// <summary>
    /// Gets if a key is pressed.
    /// </summary>
    /// <param name="index">Which key.</param>
    /// <returns>Is pressed.</returns>
    public bool IsPressed(int index) => this.keys[index];

    /// <summary>
    /// Gets all the keys.
    /// </summary>
    /// <returns>All the keys.</returns>
    public bool[] GetKeys() => this.keys;

    /// <summary>
    /// Sets a key pressed.
    /// </summary>
    /// <param name="keyCode">The key pressed.</param>
    public void OnKeyPressed(int keyCode)
    {
        switch ((VKeys)keyCode)
        {
            case VKeys.KEY_1:
                this.keys[1] = true;
                break;

            case VKeys.KEY_2:
                this.keys[2] = true;
                break;

            case VKeys.KEY_3:
                this.keys[3] = true;
                break;

            case VKeys.KEY_4:
                this.keys[0xC] = true;
                break;

            case VKeys.KEY_Q:
                this.keys[4] = true;
                break;

            case VKeys.KEY_W:
                this.keys[5] = true;
                break;

            case VKeys.KEY_E:
                this.keys[6] = true;
                break;

            case VKeys.KEY_R:
                this.keys[0xD] = true;
                break;

            case VKeys.KEY_A:
                this.keys[7] = true;
                break;

            case VKeys.KEY_S:
                this.keys[8] = true;
                break;

            case VKeys.KEY_D:
                this.keys[9] = true;
                break;

            case VKeys.KEY_F:
                this.keys[0xE] = true;
                break;

            case VKeys.KEY_Z:
                this.keys[0xA] = true;
                break;

            case VKeys.KEY_X:
                this.keys[0] = true;
                break;

            case VKeys.KEY_C:
                this.keys[0xB] = true;
                break;

            case VKeys.KEY_V:
                this.keys[0xF] = true;
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Sets a key released.
    /// </summary>
    /// <param name="keyCode">The key released.</param>
    public void OnKeyReleased(int keyCode)
    {
        switch ((VKeys)keyCode)
        {
            case VKeys.KEY_1:
                this.keys[1] = false;
                break;

            case VKeys.KEY_2:
                this.keys[2] = false;
                break;

            case VKeys.KEY_3:
                this.keys[3] = false;
                break;

            case VKeys.KEY_4:
                this.keys[0xC] = false;
                break;

            case VKeys.KEY_Q:
                this.keys[4] = false;
                break;

            case VKeys.KEY_W:
                this.keys[5] = false;
                break;

            case VKeys.KEY_E:
                this.keys[6] = false;
                break;

            case VKeys.KEY_R:
                this.keys[0xD] = false;
                break;

            case VKeys.KEY_A:
                this.keys[7] = false;
                break;

            case VKeys.KEY_S:
                this.keys[8] = false;
                break;

            case VKeys.KEY_D:
                this.keys[9] = false;
                break;

            case VKeys.KEY_F:
                this.keys[0xE] = false;
                break;

            case VKeys.KEY_Z:
                this.keys[0xA] = false;
                break;

            case VKeys.KEY_X:
                this.keys[0] = false;
                break;

            case VKeys.KEY_C:
                this.keys[0xB] = false;
                break;

            case VKeys.KEY_V:
                this.keys[0xF] = false;
                break;

            default:
                break;
        }
    }
}
