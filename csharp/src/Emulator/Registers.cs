namespace Chip8;

public class Registers
{
    private byte[] values = new byte[16];

    public Registers() {
        for (int i = 0; i < 16; i++) {
            values[i] = 0x00;
        }
    }

    public byte this[int index]
    {
        get => values[index];
        set => values[index] = value;
    }

    public void Apply(int index, Func<byte,byte> func) {
        values[index] = func(values[index]);
    }
}
