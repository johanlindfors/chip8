package se.programmeramera.chip8;

import java.util.Random;
import java.util.Stack;

public class CPU {
    final Integer FRAMETICKS = 16667;

    private Integer microSeconds;
    private Integer delayTimer;
    private Integer soundTimer;
    private Integer pc;
    private Integer i;
    private byte[] registers;
    private Stack<Integer> stack;
    private Memory memory;
    private Display display;
    private Keyboard keyboard;
    private Random random = new Random();

    public CPU(Display display, Keyboard keyboard, Memory memory) {
        super();
        this.delayTimer = 0;
        this.soundTimer = 0;
        this.registers = new byte[16];
        this.stack = new Stack<>();
        this.pc = 0x0200;
        this.i = 0x0000;
        this.display = display;
        this.keyboard = keyboard;

        this.memory = memory;
    }

    public static int getBitValue(int value, int bitIndex) {
        return value & (0x80 >> bitIndex);
    } 

    public void tick() {
        if(this.delayTimer > 0) {
            this.delayTimer--;
        }
        if(this.soundTimer > 0) {
            this.soundTimer--;
        } // TODO: Add audio logic

        this.microSeconds = FRAMETICKS;
        int delta = 0;
        do {
            delta = this.emulateCpuCycle();
            this.microSeconds -= delta;
        } while (this.microSeconds > 0 && delta != 0);
    }

    public int emulateCpuCycle() {
        int opCode = this.memory.getOpCode(this.pc);
        int x = (opCode & 0x0F00) >> 8;
        int y = (opCode & 0x00F0) >> 4;
        byte vx = this.registers[x];
        byte vy = this.registers[y];
        byte n = (byte)(opCode & 0x000F);
        byte nn = (byte)(opCode & 0x00FF);
        int nnn = opCode & 0x0FFF;

        int clockCycles = 0;
        this.pc += 2;
        switch (opCode)
        {
            case 0x00E0: // Clears the screen
                this.display.clear();
                this.display.setDrawFlag();
                return 109;

            case 0x00EE: // Returns from a subroutine.
                this.pc = this.stack.pop();
                return 105;

            case 0x000E: // Returns from a subroutine.
                this.pc = this.i;
                return 1;

            default:
                break;
        }

        switch (opCode & 0xF000)
        {
            case 0x1000: // Jumps to address nnn.
                this.pc = nnn;
                return 105;

            case 0x2000: // Calls subroutine at nnn.
                this.stack.push(this.pc);
                this.pc = nnn;
                return 105;

            case 0x3000: // Skips the next instruction if vx equals nn. (Usually the next instruction is a jump to skip a code block)
                clockCycles = 55;
                if (vx == nn) {
                    this.pc += 2;
                } else {
                    clockCycles += 9;
                }

                return clockCycles;

            case 0x4000: // Skips the next instruction if vx doesn't equal nn. (Usually the next instruction is a jump to skip a code block)
                clockCycles = 55;
                if (vx != nn) {
                    this.pc += 2;
                } else {
                    clockCycles += 9;
                }

                return clockCycles;

            case 0x5000: 
                clockCycles = 55;
                if (vx == vy) {
                    this.pc += 2;
                } else {
                    clockCycles += 9;
                }

                return clockCycles;

            case 0x6000: // Sets vx to nn.
                this.registers[x] = nn;
                return 27;

            case 0x7000: // Adds nn to vx. (Carry flag is not changed)
                this.registers[x] = (byte)((vx + nn) & 0xFF);
                return 45;

            case 0x8000:
                {
                switch (n)
                {
                    case 0x0000: // Sets vx to the value of vy.
                        this.registers[x] = vy;
                        return 200;

                    case 0x0001: // Sets vx to vx or vy. (Bitwise OR operation)
                        this.registers[x] = (byte)(vx | vy);
                        return 200;

                    case 0x0002: // Sets vx to vx and vy. (Bitwise AND operation)
                        this.registers[x] = (byte)(vx & vy);
                        return 200;

                    case 0x0003: // Sets vx to vx xor vy.
                        this.registers[x] = (byte)(vx ^ vy);
                        return 200;

                    case 0x0004: // Adds vy to vx. VF is set to 1 when there's a carry, and to 0 when there isn't.
                        Integer sum = vx + vy;
                        this.registers[0xF] = (byte)(sum > 0xFF ? 1 : 0);
                        this.registers[x] = (byte)(sum & 0xFF);
                        return 200;

                    case 0x0005: // vy is subtracted from vx. VF is set to 0 when there's a borrow, and 1 when there isn't.
                        this.registers[0xF] = (byte)(vy > vx ? 0 : 1);
                        this.registers[x] = (byte)(vx - vy);
                        return 200;

                    case 0x0006: // Stores the least significant bit of vx in VF and then shifts vx to the right by 1.[2]
                        this.registers[0xF] = (byte)(vx & 0x1);
                        this.registers[x] = (byte)(vx >> 1);
                        return 200;

                    case 0x0007: // Sets vx to vy minus vx. VF is set to 0 when there's a borrow, and 1 when there isn't.
                        this.registers[0xF] = (byte)(vx > vy ? 0 : 1);
                        this.registers[x] = (byte)((vy - vx) & 0xFF);
                        return 200;

                    case 0x000E: // Stores the most significant bit of vx in VF and then shifts vx to the left by 1.[3]
                        this.registers[0xF] = (byte)((vx & 0x80) > 0 ? 1 : 0);
                        this.registers[x] = (byte)((vx << 1) & 0xFF);
                        return 200;
                    default:
                        break;
                    }
                }

                break;

            case 0x9000: // Skips the next instruction if vx doesn't equal vy. (Usually the next instruction is a jump to skip a code block)
                clockCycles = 73;
                if (vx != vy) {
                    this.pc += 2;
                } else {
                    clockCycles += 9;
                }

                return clockCycles;

            case 0xA000: // Sets I to the address nnn.
                this.i = nnn;
                return 55;

            case 0xB000: // Jumps to the address nnn plus V0..
                this.pc = (this.registers[0] + nnn) & 0x0FFF;
                return 105;

            case 0xC000: // Sets vx to the result of a bitwise and operation on a random number (Typically: 0 to 255) and nn.
                this.registers[x] = (byte)(this.random.nextInt() % 256 & nn);
                return 164;

            case 0xD000: // Draws a sprite at coordinate (vx, vy) that has a width of 8 pixels and a height of n pixels. Each row of 8 pixels is read as bit-coded starting from memory location I; I value doesn’t change after the execution of this instruction. As described above, VF is set to 1 if any screen pixels are flipped from set to unset when the sprite is drawn, and to 0 if that doesn’t happen
                int height = n;

                this.registers[0xF] = 0;

                for (int yLine = 0; yLine < height; yLine++) {
                    int pixelValue = this.memory.getByte(this.i + yLine);

                    for (int xLine = 0; xLine < 8; xLine++) {
                        if (getBitValue(pixelValue, xLine) != 0) {
                            int xCoord = vx + xLine;
                            int yCoord = vy + yLine;

                            if (xCoord >= this.display.getDisplayWidth()) {
                                xCoord %= this.display.getDisplayWidth();
                            }

                            if (yCoord >= this.display.getDisplayHeight()) {
                                yCoord %= this.display.getDisplayHeight();
                            }

                            if (this.display.getPixel(xCoord, yCoord)) {
                                this.registers[0xF] = 1;
                            }

                            this.display.setPixel(xCoord, yCoord);
                        }
                    }
                }

                this.display.setDrawFlag();
                return 22734;

            case 0xE000:
                switch (nn) {
                    case (byte)0x9E: // Skips the next instruction if the key stored in vx is pressed. (Usually the next instruction is a jump to skip a code block)
                        this.pc += this.keyboard.isKeyPressed(vx) ? 2 : 0;
                        return 73;

                    case (byte)0xA1: // Skips the next instruction if the key stored in vx isn't pressed. (Usually the next instruction is a jump to skip a code block)
                        this.pc += this.keyboard.isKeyPressed(vx) ? 0 : 2;
                        return 73;

                    default:
                        break;
                }

                break;

            case 0xF000:

                switch (nn) {
                    case 0x07: // Sets vx to the value of the delay timer.
                        this.registers[x] = (byte)(this.delayTimer & 0xFF);
                        return 45;

                    case 0x0A: // A key press is awaited, and then stored in vx. (Blocking Operation. All instruction halted until next key event)
                        for (int i = 0; i < this.keyboard.getKeys().length; i++) {
                            if (this.keyboard.isKeyPressed((byte)i)) {
                                this.registers[x] = (byte)i;
                                break;
                            }
                        }

                        this.pc -= 2;
                        return 1;

                    case 0x15: // Sets the delay timer to vx.
                        this.delayTimer = vx & 0xFF;
                        return 45;

                    case 0x18: // Sets the sound timer to vx.
                        this.soundTimer = vx & 0xFF;
                        return 45;

                    case 0x1E: // Adds vx to I.
                        this.i = (this.i + vx) & 0xFF;
                        return 86;

                    case 0x29: // Sets I to the location of the sprite for the character in vx. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
                        this.i = 0x50 + (vx * 5);
                        return 91;

                    case 0x33: // vx, with the most significant of three digits at the address in I, the middle digit at I plus 1, and the least significant digit at I plus 2. (In other words, take the decimal representation of vx, place the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.)
                        this.memory.setByte(this.i, (byte)(vx / 100));
                        this.memory.setByte(this.i + 1, (byte)((vx % 100) / 10));
                        this.memory.setByte(this.i + 2, (byte)(vx % 10));
                        return 927;

                    case 0x55: // Stores V0 to vx (including vx) in memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                        for (int p = 0; p <= x; p++) {
                            this.memory.setByte(this.i + p, (byte)this.registers[p]);
                        }

                        return 605 + x * 64;

                    case 0x65: // Fills V0 to vx (including vx) with values from memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                        for (int p = 0; p <= x; p++) {
                            this.registers[p] = (byte)(this.memory.getByte(this.i + p) & 0xFF);
                        }

                        return 605 + x * 64;

                    default:
                        break;
                    }

                break;

            default:
                break;
        }

        return 0;
    }
}
