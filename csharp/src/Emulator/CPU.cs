// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8.Emulator;

/// <summary>
/// The Central Processing Unit.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CPU"/> class.
/// </remarks>
/// <param name="memory">Instance of memory module.</param>
/// <param name="register">Instance of register module.</param>
/// <param name="random">Instance of random module.</param>
/// <param name="keyboard">Instance of a keyboard implementation.</param>
/// <param name="screen">Instance of a screen implementation.</param>
public class CPU(Memory memory, Register register, Random random, IKeyboard keyboard, IScreen screen)
{
    private readonly Memory memory = memory;
    private readonly Register register = register;
    private readonly Random random = random;
    private readonly IKeyboard keyboard = keyboard;
    private readonly IScreen screen = screen;
    private Stack<int> stack = new Stack<int>();
    private bool drawFlag;
    private int i = 0;
    private int pc = 0x200;
    private byte delayTimer;
    private byte soundTimer;

    /// <summary>
    /// Gets the instructions counter.
    /// </summary>
    public int InstructionsCounter { get; private set; }

    /// <summary>
    /// Gets or sets the delay timer.
    /// </summary>
    public byte DelayTimer
    {
        get { return this.delayTimer; }
        set { this.delayTimer = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the draw flag is set.
    /// </summary>
    public bool DrawFlag
    {
        get { return this.drawFlag; }
        set { this.drawFlag = value; }
    }

    /// <summary>
    /// Gets the sound timer.
    /// </summary>
    public byte SoundTimer => this.soundTimer;

    /// <summary>
    /// Gets the current program counter.
    /// </summary>
    public int PC => this.pc;

    /// <summary>
    /// Gets the i variable.
    /// </summary>
    public int I => this.i;

    /// <summary>
    /// Emulates a full CPU cycle.
    /// </summary>
    public void EmulateCycle()
    {
        this.InstructionsCounter++;
        int opcode = this.memory.GetOpcode(this.pc);

        int x = (opcode & 0x0F00) >> 8;
        int y = (opcode & 0x00F0) >> 4;
        byte vx = this.register.Get(x);
        byte vy = this.register.Get(y);
        byte n = (byte)(opcode & 0x000F);
        byte nn = (byte)(opcode & 0x00FF);
        int nnn = opcode & 0x0FFF;

        this.pc += 2;

        switch (opcode)
        {
            case 0x00E0: // Clears the screen
                this.screen.Clear();
                this.drawFlag = true;
                return;

            case 0x00EE: // Returns from a subroutine.
                this.pc = this.stack.Pop();
                return;

            default:
                break;
        }

        switch (opcode & 0xF000)
        {
            case 0x1000: // Jumps to address nnn.
                this.pc = nnn;
                break;

            case 0x2000: // Calls subroutine at nnn.
                this.stack.Push(this.pc);
                this.pc = nnn;
                break;

            case 0x3000: // Skips the next instruction if vx equals nn. (Usually the next instruction is a jump to skip a code block)
                if (vx == nn)
                {
                    this.pc += 2;
                }

                break;

            case 0x4000: // Skips the next instruction if vx doesn't equal nn. (Usually the next instruction is a jump to skip a code block)
                if (vx != nn)
                {
                    this.pc += 2;
                }

                break;

            case 0x5000: // Skips the next instruction if vx equals vy. (Usually the next instruction is a jump to skip a code block)
                if (vx == vy)
                {
                    this.pc += 2;
                }

                break;

            case 0x6000: // Sets vx to nn.
                this.register.Set(x, nn);
                break;

            case 0x7000: // Adds nn to vx. (Carry flag is not changed)
                this.register.Apply(x, vx => (byte)((vx + nn) & 0xFF));
                break;

            case 0x8000:
                {
                switch (n)
                {
                    case 0x0000: // Sets vx to the value of vy.
                        this.register.Set(x, vy);
                        break;

                    case 0x0001: // Sets vx to vx or vy. (Bitwise OR operation)
                        this.register.Apply(x, vx => (byte)(vx | vy));
                        break;

                    case 0x0002: // Sets vx to vx and vy. (Bitwise AND operation)
                        this.register.Apply(x, vx => (byte)(vx & vy));
                        break;

                    case 0x0003: // Sets vx to vx xor vy.
                        this.register.Apply(x, vx => (byte)(vx ^ vy));
                        break;

                    case 0x0004: // Adds vy to vx. VF is set to 1 when there's a carry, and to 0 when there isn't.
                        this.register.Apply(x, vx =>
                        {
                            var sum = vx + vy;
                            this.register.Set(0xF, (byte)(sum > 0xFF ? 1 : 0));
                            return (byte)(sum & 0xFF);
                        });
                        break;

                    case 0x0005: // vy is subtracted from vx. VF is set to 0 when there's a borrow, and 1 when there isn't.
                        this.register.Set(0xF, (byte)(vy > vx ? 0 : 1));
                        this.register.Apply(x, vx => (byte)(vx - vy));

                        break;

                    case 0x0006: // Stores the least significant bit of vx in VF and then shifts vx to the right by 1.[2]
                        this.register.Set(0xF, (byte)(vx & 0x1));
                        this.register.Apply(x, vx => (byte)(vx >> 1));
                        break;

                    case 0x0007: // Sets vx to vy minus vx. VF is set to 0 when there's a borrow, and 1 when there isn't.
                        this.register.Set(0xF, (byte)(vx > vy ? 0 : 1));
                        this.register.Apply(x, vx => (byte)((vy - vx) & 0xFF));
                        break;

                    case 0x000E: // Stores the most significant bit of vx in VF and then shifts vx to the left by 1.[3]
                        this.register.Set(0xF, (byte)((vx & 0x80) > 0 ? 1 : 0));
                        this.register.Apply(x, vx => (byte)((vx << 1) & 0xFF));
                        break;
                    default:
                        break;
                    }
                }

                break;

            case 0x9000: // Skips the next instruction if vx doesn't equal vy. (Usually the next instruction is a jump to skip a code block)
                this.pc += vx != vy ? 2 : 0;
                break;

            case 0xA000: // Sets I to the address nnn.
                this.i = nnn;
                break;

            case 0xB000: // Jumps to the address nnn plus V0..
                this.pc = (this.register.Get(0) + nnn) & 0x0FFF;
                break;

            case 0xC000: // Sets vx to the result of a bitwise and operation on a random number (Typically: 0 to 255) and nn.
                this.register.Set(x, (byte)((this.random.Next() % 256) & nn));
                break;

            case 0xD000: // Draws a sprite at coordinate (vx, vy) that has a width of 8 pixels and a height of n pixels. Each row of 8 pixels is read as bit-coded starting from memory location I; I value doesn’t change after the execution of this instruction. As described above, VF is set to 1 if any screen pixels are flipped from set to unset when the sprite is drawn, and to 0 if that doesn’t happen
                int height = n;

                this.register.Set(0xF, 0);

                for (int yLine = 0; yLine < height; yLine++)
                {
                    int pixelValue = this.memory.GetByte(this.i + yLine);

                    for (int xLine = 0; xLine < 8; xLine++)
                    {
                        if (Utils.GetBitValue(pixelValue, xLine) != 0)
                        {
                            int xCoord = vx + xLine;
                            int yCoord = vy + yLine;

                            if (xCoord >= this.screen.Width)
                            {
                                xCoord %= this.screen.Width;
                            }

                            if (yCoord >= this.screen.Height)
                            {
                                yCoord %= this.screen.Height;
                            }

                            if (this.screen.GetPixel(xCoord, yCoord) == 1)
                            {
                                this.register.Set(0xF, 1);
                            }

                            this.screen.SetPixel(xCoord, yCoord);
                        }
                    }
                }

                this.drawFlag = true;
                break;

            case 0xE000:
                switch (nn)
                {
                    case 0x9E: // Skips the next instruction if the key stored in vx is pressed. (Usually the next instruction is a jump to skip a code block)
                        this.pc += this.keyboard.IsPressed(vx) ? 2 : 0;
                        break;

                    case 0xA1: // Skips the next instruction if the key stored in vx isn't pressed. (Usually the next instruction is a jump to skip a code block)
                        this.pc += this.keyboard.IsPressed(vx) ? 0 : 2;
                        break;

                    default:
                        break;
                }

                break;

            case 0xF000:

                switch (nn)
                {
                    case 0x07: // Sets vx to the value of the delay timer.
                        this.register.Set(x, (byte)(this.delayTimer & 0xFF));
                        break;

                    case 0x0A: // A key press is awaited, and then stored in vx. (Blocking Operation. All instruction halted until next key event)
                        for (int i = 0; i < this.keyboard.GetKeys().Length; i++)
                        {
                            if (this.keyboard.IsPressed(i))
                            {
                                this.register.Set(x, (byte)i);
                                break;
                            }
                        }

                        this.pc -= 2;
                        break;

                    case 0x15: // Sets the delay timer to vx.
                        this.delayTimer = (byte)(vx & 0xFF);
                        return;

                    case 0x18: // Sets the sound timer to vx.
                        this.soundTimer = (byte)(vx & 0xFF);
                        return;

                    case 0x1E: // Adds vx to I.
                        this.i += vx & 0xFF;
                        break;

                    case 0x29: // Sets I to the location of the sprite for the character in vx. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
                        this.i = 0x50 + (vx * 5);
                        break;

                    case 0x33: // vx, with the most significant of three digits at the address in I, the middle digit at I plus 1, and the least significant digit at I plus 2. (In other words, take the decimal representation of vx, place the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.)
                        this.memory.SetByte(this.i, (byte)(vx / 100));
                        this.memory.SetByte(this.i + 1, (byte)((vx % 100) / 10));
                        this.memory.SetByte(this.i + 2, (byte)(vx % 10));
                        break;

                    case 0x55: // Stores V0 to vx (including vx) in memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                        for (int p = 0; p <= x; p++)
                        {
                            this.memory.SetByte(this.i + p, (byte)this.register.Get(p));
                        }

                        break;

                    case 0x65: // Fills V0 to vx (including vx) with values from memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                        for (int p = 0; p <= x; p++)
                        {
                            this.register.Set(p, (byte)(this.memory.GetByte(this.i + p) & 0xFF));
                        }

                        break;

                    default:
                        break;
                    }

                break;

            default:
                break;
        }

        if (this.delayTimer > 0)
        {
            this.delayTimer -= 1;
        }

        if (this.soundTimer > 0)
        {
            this.soundTimer -= 1;
        }
    }
}
