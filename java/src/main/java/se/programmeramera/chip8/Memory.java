package se.programmeramera.chip8;

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

    private int[] memory;

    public Memory() {
        super();
        this.memory = new int[4096];
        for (int i = 0; i < FONTS.length; i++) {
            this.memory[i + 0x50] = FONTS[i] & 0xFF;
        }
        //System.arraycopy(FONTS, 0, memory, 0x0050, FONTS.length);
        // byte[] rom = java.util.Base64.getDecoder().decode(IBM);
        // System.arraycopy(rom, 0, memory, 0x0200, rom.length);
    }

    public void loadData(byte[] data)
    {
        for (int i = 0; i < data.length; i++)
        {
            this.setByte(i + 512, (data[i] & 0xFF));
        }
    }

    public Integer getOpCode(Integer pc) {
        return this.getByte(pc) << 8 | this.getByte(pc + 1);
    }

    public Integer getByte(Integer address) {
        return this.memory[address];
    }

    public void setByte(Integer address, Integer value) {
        this.memory[address] = value;
    }
}
