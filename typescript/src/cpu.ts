/// <reference path="registers.ts"/>
/// <reference path="memory.ts"/>
/// <reference path="keyboard.ts"/>
/// <reference path="display.ts"/>

const FRAME_TICKS = 16666;

class CPU {
    _registers: Registers;
    _memory : IMemory;
    _keyboard : IKeyboard;
    _display : IDisplay;
    _audio : IAudio;
    _instructionsCounter : number;
    _pc : number;
    _drawFlag : boolean;
    _stack : number[];
    _i : number;
    _microseconds : number;

    public DelayTimer : number;
    public SoundTimer : number;
    
    get PC() : number { return this._pc; }
    get I() : number { return this._i; }

    constructor(keyboard : IKeyboard, display : IDisplay, audio : IAudio) {
        this._keyboard = keyboard;
        this._display = display;
        this._audio = audio;
    }

    attachMemory(memory : IMemory) : void {
        this._display.clear();
        this._audio.stop();
        this._registers = new Registers();
        this._pc = 0x200;
        this._instructionsCounter = 0;
        this._stack = [];
        this._microseconds = 0;
        this._memory = memory;
    }

    tick() : void {
        if(this._memory == null)
            return;
        
        if (this.DelayTimer > 0) {
            this.DelayTimer -= 1;
        }
        if (this.SoundTimer> 0) {
            this.SoundTimer -= 1;
            if (this._audio.isPlaying() == false) {
                this._audio.start();
            }
        } else if (this._audio.isPlaying()) {
            this._audio.stop();
        }
        this._microseconds += FRAME_TICKS;
        while(1) {
            let delta = this.emulateCPUCycle();
            this._microseconds -= delta;
            if (this._microseconds <= 0 || delta == 0) {
                break;
            }
        }
    }

    emulateCPUCycle() : number {
        this._instructionsCounter++;
        let opcode = this._memory.getOpCode(this._pc);

        let x = (opcode & 0x0F00) >> 8;
        let y = (opcode & 0x00F0) >> 4;
        let vx = this._registers.getRegister(x);
        let vy = this._registers.getRegister(y);
        let n = opcode & 0x000F;
        let nn = opcode & 0x00FF;
        let nnn = opcode & 0x0FFF;

        this._pc += 2;

        switch (opcode)
        {
            case 0x00E0: // Clears the screen
                this._display.clear();
                this._drawFlag = true;
                return 109;

            case 0x00EE: // Returns from a subroutine.
                this._pc = this._stack.pop();
                return 105;
            
            case 0x000E:
                this._pc = this._i;
                return 1;

            default:
                break;
        }

        switch (opcode & 0xF000)
        {
            case 0x1000: // Jumps to address nnn.
                this._pc = nnn;
                return 105;

            case 0x2000: // Calls subroutine at nnn.
                this._stack.push(this._pc);
                this._pc = nnn;
                return 105;

            case 0x3000: {// Skips the next instruction if vx equals nn. (Usually the next instruction is a jump to skip a code block)
                let clockCycles = 55;    
                if (vx == nn) {
                    this._pc += 2;
                } else {
                    clockCycles += 9;
                }
                return clockCycles;
            }

            case 0x4000: {// Skips the next instruction if vx doesn't equal nn. (Usually the next instruction is a jump to skip a code block)
                let clockCycles = 55;
                if (vx != nn) {
                    this._pc += 2;
                } else {
                    clockCycles += 9;
                }
                return clockCycles;
            }
            case 0x5000: {// Skips the next instruction if vx equals vy. (Usually the next instruction is a jump to skip a code block)
                let clockCycles = 55;
                if (vx == vy) {
                    this._pc += 2;
                } else {
                    clockCycles += 9;
                }
                return clockCycles;
            }

            case 0x6000: // Sets vx to nn.
                this._registers.setRegister(x, nn);
                return 27;

            case 0x7000: // Adds nn to vx. (Carry flag is not changed)
                this._registers.apply(x, vx => (vx + nn) & 0xFF);
                return 45;

            case 0x8000: {
                switch (n) {
                    case 0x0000: // Sets vx to the value of vy.
                        this._registers.setRegister(x, vy);
                        return 200;

                    case 0x0001: // Sets vx to vx or vy. (Bitwise OR operation)
                        this._registers.apply(x, vx => vx | vy);
                        return 200;

                    case 0x0002: // Sets vx to vx and vy. (Bitwise AND operation)
                        this._registers.apply(x, vx => vx & vy);
                        return 200;

                    case 0x0003: // Sets vx to vx xor vy.
                        this._registers.apply(x, vx => vx ^ vy);
                        return 200;

                    case 0x0004: // Adds vy to vx. VF is set to 1 when there's a carry, and to 0 when there isn't.
                        this._registers.apply(x, vx => {
                            var sum = vx + vy;
                            this._registers.setRegister(0xF, sum > 0xFF ? 1 : 0);
                            return sum & 0xFF;
                        });
                        return 200;

                    case 0x0005: // vy is subtracted from vx. VF is set to 0 when there's a borrow, and 1 when there isn't.
                        this._registers.setRegister(0xF, vy > vx ? 0 : 1);
                        this._registers.apply(x, vx => vx - vy);
                        return 200;

                    case 0x0006: // Stores the least significant bit of vx in VF and then shifts vx to the right by 1.[2]
                        this._registers.setRegister(0xF, vx & 0x1);
                        this._registers.apply(x, vx => vx >> 1);
                        return 200;

                    case 0x0007: // Sets vx to vy minus vx. VF is set to 0 when there's a borrow, and 1 when there isn't.
                        this._registers.setRegister(0xF, vx > vy ? 0 : 1);
                        this._registers.apply(x, vx => (vy - vx) & 0xFF);
                        return 200;

                    case 0x000E: // Stores the most significant bit of vx in VF and then shifts vx to the left by 1.[3]
                        this._registers.setRegister(0xF, (vx & 0x80) > 0 ? 1 : 0);
                        this._registers.apply(x, vx => (vx << 1) & 0xFF);
                        return 200;
                    default:
                        break;
                    }
                }
                break;

            case 0x9000: {// Skips the next instruction if vx doesn't equal vy. (Usually the next instruction is a jump to skip a code block)
                let clockCycles = 73;
                if (vx != vy) {
                    this._pc += 2;
                } else {
                    clockCycles += 9;
                }
                return clockCycles;
            }
            case 0xA000: // Sets I to the address nnn.
                this._i = nnn;
                return 55;

            case 0xB000: // Jumps to the address nnn plus V0..
                this._pc = (this._registers.getRegister(0) + nnn) & 0x0FFF;
                return 105;

            case 0xC000: // Sets vx to the result of a bitwise and operation on a random number (Typically: 0 to 255) and nn.
                // TODO: this._registers.setRegister(x, this._random.NextByte() & nn);
                return 164;

            case 0xD000: // Draws a sprite at coordinate (vx, vy) that has a width of 8 pixels and a height of n pixels. Each row of 8 pixels is read as bit-coded starting from memory location I; I value doesn’t change after the execution of this instruction. As described above, VF is set to 1 if any screen pixels are flipped from set to unset when the sprite is drawn, and to 0 if that doesn’t happen
                let height = n;
                this._registers.setRegister(0xF, 0);

                for (let yLine = 0; yLine < height; yLine++) {
                    let pixelValue = this._memory.getByte(this._i + yLine);

                    for (let xLine = 0; xLine < 8; xLine++) {
                        if (CPU.getBitValue(pixelValue, xLine) != 0) {
                            let xCoord = vx + xLine;
                            let yCoord = vy + yLine;

                            if (xCoord >= this._display.Width) {
                                xCoord %= this._display.Width;
                            }

                            if (yCoord >= this._display.Height) {
                                yCoord %= this._display.Height;
                            }

                            if (this._display.getPixel(xCoord, yCoord)) {
                                this._registers.setRegister(0xF, 1);
                            }

                            this._display.setPixel(xCoord, yCoord);
                        }
                    }
                }

                this._display.setDrawFlag();
                return 22734;

            case 0xE000:
                switch (nn) {
                    case 0x9E: // Skips the next instruction if the key stored in vx is pressed. (Usually the next instruction is a jump to skip a code block)
                        this._pc += this._keyboard.isKeyPressed(vx) ? 2 : 0;
                        return 73;

                    case 0xA1: // Skips the next instruction if the key stored in vx isn't pressed. (Usually the next instruction is a jump to skip a code block)
                        this._pc += this._keyboard.isKeyPressed(vx) ? 0 : 2;
                        return 73;

                    default:
                        break;
                }
                break;

            case 0xF000:

                switch (nn) {
                    case 0x07: // Sets vx to the value of the delay timer.
                        this._registers.setRegister(x, this.DelayTimer & 0xFF);
                        return 45;

                    case 0x0A: // A key press is awaited, and then stored in vx. (Blocking Operation. All instruction halted until next key event)
                        for (let i = 0; i < this._keyboard.getAllKeys().length; i++) {
                            if (this._keyboard.isKeyPressed(i)) {
                                this._registers.setRegister(x, i);
                                break;
                            }
                        }
                        this._pc -= 2;
                        return 1;

                    case 0x15: // Sets the delay timer to vx.
                        this.DelayTimer = vx & 0xFF;
                        return 45;

                        case 0x18: // Sets the sound timer to vx.
                        this.SoundTimer = vx & 0xFF;
                        return 45;

                    case 0x1E: // Adds vx to I.
                        this._i += vx & 0xFF;
                        return 86;

                    case 0x29: // Sets I to the location of the sprite for the character in vx. Characters 0-F (in hexadecimal) are represented by a 4x5 font.
                        this._i = 0x50 + (vx * 5);
                        return 91;

                    case 0x33: // vx, with the most significant of three digits at the address in I, the middle digit at I plus 1, and the least significant digit at I plus 2. (In other words, take the decimal representation of vx, place the hundreds digit in memory at location in I, the tens digit at location I+1, and the ones digit at location I+2.)
                        this._memory.setByte(this._i, vx / 100);
                        this._memory.setByte(this._i + 1, (vx % 100) / 10);
                        this._memory.setByte(this._i + 2, vx % 10);
                        return 927;

                    case 0x55: // Stores V0 to vx (including vx) in memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                        for (let p = 0; p <= x; p++) {
                            this._memory.setByte(this._i + p, this._registers.getRegister(p));
                        }
                        return 605 + x * 64;

                    case 0x65: // Fills V0 to vx (including vx) with values from memory starting at address I. The offset from I is increased by 1 for each value written, but I itself is left unmodified.
                        for (let p = 0; p <= x; p++) {
                            this._registers.setRegister(p, this._memory.getByte(this._i + p) & 0xFF);
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

    private static getBitValue(value : number, bitIndex : number) : number {
        return value & (0x80 >> bitIndex);
    }
}
