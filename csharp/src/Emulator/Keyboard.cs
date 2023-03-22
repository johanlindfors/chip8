namespace Chip8;

public class Keyboard : IKeyboard
{
   private bool[] keys = new bool[16];

    public static byte[] FONTS = {
            0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
            0x20, 0x60, 0x20, 0x20, 0x70, // 1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
            0x90, 0x90, 0xF0, 0x10, 0x10, // 4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
            0xF0, 0x10, 0x20, 0x40, 0x40, // 7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
            0xF0, 0x90, 0xF0, 0x90, 0x90, // A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
            0xF0, 0x80, 0x80, 0x80, 0xF0, // C
            0xE0, 0x90, 0x90, 0x90, 0xE0, // D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
            0xF0, 0x80, 0xF0, 0x80, 0x80  // F
    };

    public void OnKeyPressed(int keyCode) {
        switch ((VKeys)keyCode) {
            case VKeys.KEY_1:
                keys[1] = true;
                break;
            case VKeys.KEY_2:
                keys[2] = true;
                break;
            case VKeys.KEY_3:
                keys[3] = true;
                break;
            case VKeys.KEY_4:
                keys[0xC] = true;
                break;
            case VKeys.KEY_Q:
                keys[4] = true;
                break;
            case VKeys.KEY_W:
                keys[5] = true;
                break;
            case VKeys.KEY_E:
                keys[6] = true;
                break;
            case VKeys.KEY_R:
                keys[0xD] = true;
                break;
            case VKeys.KEY_A:
                keys[7] = true;
                break;
            case VKeys.KEY_S:
                keys[8] = true;
                break;
            case VKeys.KEY_D:
                keys[9] = true;
                break;
            case VKeys.KEY_F:
                keys[0xE] = true;
                break;
            case VKeys.KEY_Z:
                keys[0xA] = true;
                break;
            case VKeys.KEY_X:
                keys[0] = true;
                break;
            case VKeys.KEY_C:
                keys[0xB] = true;
                break;
            case VKeys.KEY_V:
                keys[0xF] = true;
                break;
            default:
                break;
        }
    }

    public void OnKeyReleased(int keyCode) {
        switch ((VKeys)keyCode) {
            case VKeys.KEY_1:
                keys[1] = false;
                break;
            case VKeys.KEY_2:
                keys[2] = false;
                break;
            case VKeys.KEY_3:
                keys[3] = false;
                break;
            case VKeys.KEY_4:
                keys[0xC] = false;
                break;
            case VKeys.KEY_Q:
                keys[4] = false;
                break;
            case VKeys.KEY_W:
                keys[5] = false;
                break;
            case VKeys.KEY_E:
                keys[6] = false;
                break;
            case VKeys.KEY_R:
                keys[0xD] = false;
                break;
            case VKeys.KEY_A:
                keys[7] = false;
                break;
            case VKeys.KEY_S:
                keys[8] = false;
                break;
            case VKeys.KEY_D:
                keys[9] = false;
                break;
            case VKeys.KEY_F:
                keys[0xE] = false;
                break;
            case VKeys.KEY_Z:
                keys[0xA] = false;
                break;
            case VKeys.KEY_X:
                keys[0] = false;
                break;
            case VKeys.KEY_C:
                keys[0xB] = false;
                break;
            case VKeys.KEY_V:
                keys[0xF] = false;
                break;
            default:
                break;
        }
    }

    public bool this[int index] {
        get => keys[index];
    }

    public int Length {
        get => keys.Length;
    }
}