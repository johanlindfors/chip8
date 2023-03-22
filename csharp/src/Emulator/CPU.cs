namespace Chip8;

public class CPU 
{
    private readonly Memory memory;
    private readonly Registers registers;
    private readonly Random random;
    private readonly IKeyboard keyboard;
    private readonly IScreen screen;
    private Stack<int> stack;

    private bool drawFlag;

    private int i;
    private int pc;
    private byte delayTimer, soundTimer;
    public int InstructionsCounter;
    private int microseconds;

    public CPU(Memory memory, Registers register, Random random, IKeyboard keyboard, IScreen screen) {
        this.memory = memory;
        this.registers = register;
        this.random = random;
        this.keyboard = keyboard;
        this.screen = screen;

        stack = new Stack<int>();

        pc = 0x200;
        i = 0;
    }

    public void Tick() {
        if(this.delayTimer > 0) {
            this.delayTimer -= 1;
        }

        if(this.soundTimer > 0) {
            this.soundTimer -= 1;
        }

        microseconds += 16666;
        while(microseconds > 0) {
            var delta = EmulateCycle();
            if (delta == 0) break;
            microseconds -= delta;
        }
    }

    public int EmulateCycle() 
    {
        InstructionsCounter++;
        int opcode = memory.GetOpcode(pc);
        //Console.WriteLine(opcode.ToString("X"));

        int x = (opcode & 0x0F00) >> 8;
        int y = (opcode & 0x00F0) >> 4;
        byte VX = registers[x];
        byte VY = registers[y];
        byte N = (byte)(opcode & 0x000F);
        byte NN = (byte)(opcode & 0x00FF);
        int NNN = (opcode & 0x0FFF);

        pc += 2;

        switch(opcode) {
 
            case 0x00E0: // Clears the screen
                screen.Clear();
                drawFlag = true;
                return 100;
 
            case 0x00EE: // Returns from a subroutine.
                pc = stack.Pop();
                return 1;
 
            default:
                break;
        }

        switch(opcode & 0xF000) {
 
            case 0x1000: // Jumps to address NNN.
                pc = NNN;
                return 105;
 
            case 0x2000: // Calls subroutine at NNN.
                stack.Push(pc);
                pc = NNN;
                return 105; 

            case 0x3000: // Skips the next instruction if VX equals NN. (Usually the next instruction is a jump to skip a code block)
                if(VX == NN) {
                    pc += 2;
                    return 55;
                }
                return 64;

            case 0x4000: // Skips the next instruction if VX doesn't equal NN. (Usually the next instruction is a jump to skip a code block)
                if(VX != NN) {
                    pc += 2;
                    return 55;
                }
                return 64;

            case 0x5000: // Skips the next instruction if VX equals VY. (Usually the next instruction is a jump to skip a code block)
                if(VX == VY) {
                    pc += 2;
                    return 55;
                }
                return 64;

            case 0x6000: // Sets VX to NN.
                registers[x] = NN;
                return 27;

            case 0x7000: // Adds NN to VX. (Carry flag is not changed)
                registers.Apply(x, vx => (byte)((vx + NN) & 0xFF));
                return 45;

            case 0x8000:{
                switch(N) {
                    
                    case 0x0000: // Sets VX to the value of VY.
                        registers[x] = VY;
                        break;

                    case 0x0001: //	Sets VX to VX or VY. (Bitwise OR operation)
                        registers.Apply(x, vx => (byte)(vx | VY));
                        break;

                    case 0x0002: // Sets VX to VX and VY. (Bitwise AND operation)
                        registers.Apply(x, vx => (byte)(vx & VY));
                        break;

                    case 0x0003: // Sets VX to VX xor VY.
                        registers.Apply(x, vx => (byte)(vx ^ VY));
                        break;

                    case 0x0004: // Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't.
                        registers.Apply(x, vx => {
                            var sum = vx + VY;
                            registers[0xF] = (byte)(sum > 0xFF ? 1 : 0);
                            return (byte)(sum & 0xFF);
                        });
                        break;

                    case 0x0005: // VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                        registers[0xF] = (byte)(VY > VX ? 0 : 1);
                        registers.Apply(x, vx => (byte)(vx - VY));
                        
                        break;

                    case 0x0006: // Stores the least significant bit of VX in VF and then shifts VX to the right by 1.[2]
                        registers.Apply(x, vx => (byte)(vx >> 1));
                        registers[0xF] = (byte)(VX & 0x1);
                        break;

                    case 0x0007: // Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.
                        registers[0xF] = (byte)(VX > VY ? 0 : 1);
                        registers.Apply(x, vx => (byte)((VY - vx) & 0xFF));
                        break;

                    case 0x000E: // Stores the most significant bit of VX in VF and then shifts VX to the left by 1.[3]
                        registers[0xF] = (byte)((VX & 0x80) > 0 ? 1 : 0);
                        registers.Apply(x, vx => (byte)((vx << 1) & 0xFF));
                        registers[0xF] = (byte)((VX & 0x80) > 0 ? 1 : 0);
                        break;
                    default:
                        return 0;
                    }
                }
                return 200;

            case 0x9000: // Skips the next instruction if VX doesn't equal VY. (Usually the next instruction is a jump to skip a code block)
                pc += VX != VY ? 2 : 0;
                return VX != VY ? 73 : 82;

            case 0xA000: // Sets I to the address NNN.
                i = NNN;
                return 55;

            case 0xB000: // Jumps to the address NNN plus V0..
                pc = (registers[0] + NNN) & 0x0FFF;
                return 105;

            case 0xC000: // Sets VX to the result of a bitwise and operation on a random number (Typically: 0 to 255) and NN.
                registers[x] = (byte)((random.Next() % 256) & NN);
                return 164;
            
            case 0xD000: // Draws a sprite at coordinate (VX, VY) that has a width of 8 pixels and a height of N pixels. Each row of 8 pixels is read as bit-coded starting from memory location I; I value doesn’t change after the execution of this instruction. As described above, VF is set to 1 if any screen pixels are flipped from set to unset when the sprite is drawn, and to 0 if that doesn’t happen
                int height = N;

                registers[0xF] = 0;

                for (int yLine = 0; yLine < height; yLine++) {
                    int pixelValue = memory.GetByte(I + yLine);

                    for (int xLine = 0; xLine < 8; xLine++) {

                        if (Utils.GetBitValue(pixelValue, xLine) != 0) {

                            int xCoord = (VX + xLine);
                            int yCoord = (VY + yLine) ;

                            if(xCoord >= screen.Width )
                                xCoord %= screen.Width;

                            if(yCoord >= screen.Height)
                                yCoord %= screen.Height;

                         if (screen.GetPixel(xCoord, yCoord) == 1)
                                registers[0xF] = 1;

                            screen.SetPixel(xCoord, yCoord);
                        }
                    }
                }
                drawFlag = true;
                return 22734;
            
            case 0xE000: {
                switch (NN) {
                    
                    case 0x9E: // Skips the next instruction if the key stored in VX is pressed. (Usually the next instruction is a jump to skip a code block)
                        pc += keyboard[VX] ? 2 : 0;
                        break;

                    case 0xA1: // Skips the next instruction if the key stored in VX isn't pressed. (Usually the next instruction is a jump to skip a code block)
                        pc += keyboard[VX] ? 0 : 2;
                        break;

                    default:
                        break;
                }
                return 73;
            }

            case 0xF000: {
                switch (NN) {

                    case 0x07: // Sets VX to the value of the delay timer.
                        registers[x] = (byte)(delayTimer & 0xFF);
                        return 45;

                    case 0x0A: // A key press is awaited, and then stored in VX. (Blocking Operation. All instruction halted until next key event)
                        for (int i = 0; i < keyboard.Length; i++) {
                            if(keyboard[i]){
                                System.Console.WriteLine("Key pressed!");
                                registers[x] = (byte)i;
                                return 1;
                            }
                        }
                        pc -= 2;
                        return 1;

                    case 0x15: // Sets the delay timer to VX.
                        delayTimer = (byte)(VX & 0xFF);
                        return 45;

                    case 0x18: // Sets the sound timer to VX.
                        soundTimer = (byte)(VX & 0xFF);
                        return 45;

                    case 0x1E: // Adds VX to I.
                        i += (VX & 0xFF);
                        return 86;

                    case 0x29: // Sets I to the location of the sprite for the character in VX. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
                        i = 0x50 + VX * 5;
                        return 91;

                    case 0x33: // VX, with the most significant of three digits at the address in I, the middle digit at I plus 1, and the least significant digit at I plus 2. (In other words, take the decimal representation of VX, place the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.)
                        memory.SetByte(i, (Byte)(VX / 100));
                        memory.SetByte(i + 1, (Byte)((VX % 100) / 10));
                        memory.SetByte(i + 2, (Byte)(VX % 10));
                        return 927;

                    case 0x55: // Stores V0 to VX (including VX) in memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                        for(int p = 0; p <= x; p++)
                            memory.SetByte(i + p, (Byte)registers[p]);
                        return 605 + x * 64;

                    case 0x65: // Fills V0 to VX (including VX) with values from memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                        for(int p = 0; p <= x; p++)
                            registers[p] = (byte)(memory.GetByte(i + p) & 0xFF);
                        return 605 + x * 64;

                    default:
                        break;
                    }
                }
                return 0;

            default:
                return 0;
        }
    }

    public byte DelayTimer { get { return delayTimer; } set { delayTimer = value; } }
    public bool DrawFlag { get { return drawFlag; } set { drawFlag = value; } }

    public byte SoundTimer => soundTimer;

    public int PC { get { return pc; } }

    public int I { get { return i; } }
}
