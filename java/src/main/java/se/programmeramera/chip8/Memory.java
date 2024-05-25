package se.programmeramera.chip8;

import java.nio.charset.Charset;
import java.util.HexFormat;

public class Memory {
    private final byte[] FONTS = HexFormat.ofDelimiter(":")
        .parseHex("F0:90:90:90:F0:" + // 0      0
            "20:60:20:20:70:" + // 1      5
            "F0:10:F0:80:F0:" + // 2     10
            "F0:10:F0:10:F0:" + // 3     15
            "90:90:F0:10:10:" + // 4     20
            "F0:80:F0:10:F0:" + // 5     25
            "F0:80:F0:90:F0:" + // 6     30
            "F0:10:20:40:40:" + // 7     35
            "F0:90:F0:90:F0:" + // 8     40
            "F0:90:F0:10:F0:" + // 9
            "F0:90:F0:90:90:" + // A
            "E0:90:E0:90:E0:" + // B
            "F0:80:80:80:F0:" + // C
            "E0:90:90:90:E0:" + // D
            "F0:80:F0:80:F0:" + // E
            "F0:80:F0:80:80"); // F

    private final byte[] IBM = "AOCiKmAMYQjQH3AJojnQH6JIcAjQH3AEolfQH3AIombQH3AIonXQHxIo/wD/ADwAPAA8ADwA/wD//wD/ADgAPwA/ADgA/wD/gADgAOAAgACAAOAA4ACA+AD8AD4APwA7ADkA+AD4AwAHAA8AvwD7APMA4wBD4ADgAIAAgACAAIAA4ADg".getBytes(Charset.forName("UTF-8"));
    private final byte[] IBM2 = HexFormat.ofDelimiter(":")
        .parseHex("00:E0:A2:2A:60:0C");
    private final byte[] IBM3 = {0x00, (byte)0xE0, (byte)0xA2, 0x2A, 0x60, 0x0C};
    private byte[] memory;

    public Memory() {
        super();
        this.memory = new byte[4096];
        System.arraycopy(FONTS, 0, memory, 0x0050, FONTS.length);
        // byte[] rom = java.util.Base64.getDecoder().decode(IBM);
        // System.arraycopy(rom, 0, memory, 0x0200, rom.length);
    }

    public void loadData(byte[] data)
    {
        for (int i = 0; i < data.length; i++)
        {
            this.setByte(i + 512, (byte)(data[i] & 0xFF));
        }
    }

    public int getOpCode(int pc) {
        return this.getByte(pc) << 8 | this.getByte(pc + 1);
    }

    public byte getByte(int address) {
        return this.memory[address];
    }

    public void setByte(int address, byte value) {
        this.memory[address] = value;
    }
}
