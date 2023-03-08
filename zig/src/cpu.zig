const std = @import("std");
const Memory = @import("memory.zig").Memory;
const Display = @import("display.zig").Display;
const Keyboard = @import("keyboard.zig").Keyboard;
const Audio = @import("audio.zig").Audio;

const SPRITE_CHARS_ADDR = 0x0000;
var rnd = std.rand.DefaultPrng.init(0);

pub const CPU = struct {
    memory: Memory,
    delayTimer: u8,
    soundTimer: u8,
    microseconds: i32,
    pc: u16,
    registers: [16]u8,
    index: u16,
    sp: u8,
    stack: [16]u16,


    const Self = @This();

    pub fn init() Self {
        return CPU {
            .memory = Memory.init(),
            .delayTimer = 0,
            .soundTimer = 0,
            .microseconds = 0,
            .pc = 0x0200,
            .registers = [_]u8{0x00} ** 16,
            .index = 0x0000,
            .sp = 0,
            .stack = [_]u16{0x0000} ** 16,
        };
    }

    pub fn tick(self: *Self, display: *Display, keyboard: *Keyboard, audio: *Audio) void {
        if(self.delayTimer > 0){
            self.delayTimer -= 1;
        }
        if(self.soundTimer > 0){
            self.soundTimer -= 1;
            if(audio.isPlaying() == false){
                audio.start();
            }
        } else {
            if(audio.isPlaying()){
                audio.stop();
            }
        }

        self.microseconds += 16666;
        var delta: i32 = 0;
        while(self.microseconds > 0) : (self.microseconds -= delta){
            delta = self.emulateCycle(display, keyboard);
            if(delta == 0) {
                break;
            }
        }
    }

    pub fn emulateCycle(self: *Self, display: *Display, keyboard: *Keyboard) i32 {
        var opcode = self.getOpCode();
        return self.execute(opcode, display, keyboard);   
    }

    fn getOpCode(self: *Self) u16 {
        var opcode: u16 = @intCast(u16, self.memory.get(self.pc)) << 8 | self.memory.get(self.pc + 1);
        self.pc += 2;
        return opcode;
    }

    fn execute(self: *Self, opcode: u16, display: *Display, keyboard: *Keyboard) i32 {
        var x: u8 = @intCast(u8, opcode >> 8 & 0x0F);
        var y: u8 = @intCast(u8, opcode >> 4 & 0x0F);
        var n: u8 = @intCast(u8, opcode & 0x000F);
        var nn: u8 = @intCast(u8, opcode & 0x00FF);
        var nnn: u16 = opcode & 0x0FFF;
        switch(opcode & 0xF000) {
            0x0000 => switch (opcode & 0x00FF) {
                    0x00E0 => return opClearScreen(display),
                    0x000E => return self.opReturn(),
                    0x00EE => return self.opReturnFromSubroutine(),
                    else => return 0,
                },
            0x1000 => return self.opJump(nnn),
            0x2000 => return self.opJumpToSubroutine(nnn),
            0x3000 => return self.opSkipIfVxEqualsNn(x,nn),
            0x4000 => return self.opSkipIfVxNotEqualsNn(x, nn),
            0x5000 => return self.opSkipIfVxEqualsVy(x,y),
            0x6000 => return self.opSetRegisterXToNN(x, nn),
            0x7000 => return self.opAddNNToX(x, nn),
            0x8000 => switch (opcode & 0x000F) {
                    0x0000 => return self.opSetVxToValueOfVy(x, y),
                    0x0001 => return self.opBinaryOr(x, y),
                    0x0002 => return self.opBinaryAnd(x, y),
                    0x0003 => return self.opBinaryXor(x, y),
                    0x0004 => return self.opAddWithCarry(x, y),
                    0x0005 => return self.opSubtractYFromX(x, y),
                    0x0006 => return self.opShiftRight(x),
                    0x0007 => return self.opSubtractXFromY(x, y),
                    0x000E => return self.opShiftLeft(x),
                    else => return 0,
                },
            0x9000 => return self.opSkipIfVxNotEqualsVy(x, y),
            0xA000 => return self.opSetIndexToNNN(nnn),
            0xB000 => return self.opJumpWithOffset(nnn),
            0xC000 => return self.opRandom(x, nn),
            0xD000 => return self.opDisplay(x, y, n, display),
            0xE000 => switch (opcode & 0x00FF) {
                    0x009E => return self.opSkipIfKeyPressed(x, keyboard),
                    0x00A1 => return self.opSkipIfNotKeyPressed(x, keyboard),
                    else => return 0,
                },
            0xF000 => switch (opcode & 0x00FF) {
                    0x0007 => return self.opGetDelayTimer(x),
                    0x000A => return self.opGetKey(x, keyboard),
                    0x0015 => return self.opSetDelayTimer(x),
                    0x0018 => return self.opSetSoundTimer(x),
                    0x001E => return self.opAddToIndex(x),
                    0x0029 => return self.opFontCharacter(x),
                    0x0033 => return self.opBinaryCodedDecimalConversion(x),
                    0x0055 => return self.opStoreRegistersToMemory(x),
                    0x0065 => return self.opLoadRegistersFromMemory(x),
                    else => return 0,
                },
            else => return 0,
        }
    }

    // 0x00E0
    fn opClearScreen(display: *Display) u16 {
        display.clear();
        return 100;
    }

    // 0x1NNN
    fn opJump(self: *Self, nnn: u16) u16 {
        self.pc = nnn;
        return 105; 
    }

    // 0x6XNN
    fn opSetRegisterXToNN(self: *Self, x: u8, nn: u8) u16 {
        self.registers[x] = nn;
        return 27;
    }

    // 0x7XNN
    fn opAddNNToX(self: *Self, x: u8, nn: u8) u16 {
        var vx = self.registers[x];
        var vy = nn;
        while (vy != 0) {
            var carry = vx & vy;
            vx ^= vy;
            vy = carry << 1;
        }
        self.registers[x] = vx;
        return 45;
    }

    // 0xANNN
    fn opSetIndexToNNN(self: *Self, nnn: u16) u16 {
        self.index = nnn;
        return 55;
    }

    // 0xDXYN
    fn opDisplay(self: *Self, x: u8, y: u8, n: u8, display: *Display) u16 {
    	var index = self.index;
        var vx = @intCast(u16, self.registers[x]);
        var vy = @intCast(u16, self.registers[y]);
        std.debug.print("Rendering a {} pixel tall sprite at X: {}, Y: {} from the address: {}\n", .{n, vx, vy, index});

        var row = vy;
        while(row < vy + @intCast(u16,n)) : (row += 1) {
            if(row >= 32){
                break;
            }
            var startX = vx + row * 64;
            var endX = startX + 8;
            var bitMask: u8 = 0x80;
            var col = startX;
            while (col < endX) : (col += 1) {
                if ((col % 64) >= (startX % 64)) {
                    var position = (row * 64) + (col % 64);
                    if (self.memory.get(index) & bitMask == bitMask) {
                        display.flipPixel(position);
                    }
                }
                bitMask /= 2;
            }
            index += 1;
        }

        display.setDrawFlag(true);
        return 22734;
    }

    // 0xFX0A
    fn opGetKey(self: *Self, x: u16, keyboard: *Keyboard) u16 {
        var i: u8 = 0;
        while (i < 16) : (i += 1) {
            if (keyboard.hasKeyBeenReleased(i)) {
                self.registers[x] = i;
                return 1;
            }
        }
        self.pc -= 2;
        return 1;
    }

    // 0xFX29
    fn opFontCharacter(self: *Self, x: u16) u16 {
        self.index = SPRITE_CHARS_ADDR + self.registers[x];
        return 91;
    }

    // 0xFX55
    fn opStoreRegistersToMemory(self: *Self, x: u16) u16 {
        var i: u8 = 0;
        while (i <= x) : (i += 1) {
            self.memory.set(self.index+i, self.registers[i]);
        }
        return 605 + x * 64;
    }

    // 0xFX65
    fn opLoadRegistersFromMemory(self: *Self, x: u16) u16 {
        var i: u8 = 0;
        while (i <= x) : (i += 1) {
            self.registers[i] = self.memory.get(self.index + i);
        }
        return 605 + x * 64;
    }

    // 0xFX33
    fn opBinaryCodedDecimalConversion(self: *Self, x: u16) u16 {
        self.memory.set(self.index, self.registers[x] / 100);
        self.memory.set(self.index+1, (self.registers[x] / 10) % 10);
        self.memory.set(self.index+2, self.registers[x] % 10);
        return 927;
    }

    // 0xFX1E
    fn opAddToIndex(self: *Self, x: u16) u16 {
        self.index += self.registers[x];
        return 86;
    }

    // 0xFX07
    fn opGetDelayTimer(self: *Self, x: u16) u16 {
        self.registers[x] = self.delayTimer;
        return 45;
    }

    // 0xFX15
    fn opSetDelayTimer(self: *Self, x: u16) u16 {
        self.delayTimer = self.registers[x];
        return 45;
    }

    // 0xFX18
    fn opSetSoundTimer(self: *Self, x: u16) u16 {
        self.soundTimer = self.registers[x];
        return 45;
    }

    // 0x3XNN
    fn opSkipIfVxEqualsNn(self: *Self, x: u16, nn: u8) u16 {
        var clockCycles: u16 = 55;
        if (self.registers[x] == nn) {
            self.pc += 2;
        } else {
            clockCycles += 9;
        }
        return clockCycles;
    }

    // 0x4XNN
    fn opSkipIfVxNotEqualsNn(self: *Self, x: u16, nn: u8) u16 {
        var clockCycles: u16 = 55;
        if (self.registers[x] != nn) {
            self.pc += 2;
        } else {
            clockCycles += 9;
        }
        return clockCycles;
    }

    // 0x9XY0
    fn opSkipIfVxNotEqualsVy(self: *Self, x: u16, y: u16) u16 {
        var clockCycles: u16 = 73;
        if ((self).registers[x] != self.registers[y]) {
            self.pc += 2;
        } else {
            clockCycles += 9;
        }
        return clockCycles;
    }

    // 0x5XY0
    fn opSkipIfVxEqualsVy(self: *Self, x: u16, y: u16) u16 {
        var clockCycles: u16 = 55;
        if (self.registers[x] == self.registers[y]) {
            self.pc += 2;
        } else {
            clockCycles += 9;
        }
        return clockCycles;
    }

    // 0xEX9E
    fn opSkipIfKeyPressed(self: *Self, x: u16, keyboard: *Keyboard) u16 {
        if (keyboard.isKeyPressed(self.registers[x])) {
            self.pc += 2;
        }
        return 73;
    }

    // 0xEXA1
    fn opSkipIfNotKeyPressed(self: *Self, x: u16, keyboard: *Keyboard) u16 {
        if (!keyboard.isKeyPressed(self.registers[x])) {
            self.pc += 2;
        }
        return 73;
    }

    // 0xCXNN
    fn opRandom(self: *Self, x: u16, nn: u8) u16 {
        var random = rnd.random().int(u8);
        self.registers[x] = random & nn;
        return 164;
    }

    // 0xBXNN
    fn opJumpWithOffset(self: *Self, addr: u16) u16 {
        self.pc = addr + self.registers[0];
        return 105;
    }

    // 0x8XY0
    fn opSetVxToValueOfVy(self: *Self, x: u16, y: u16) u16 {
        self.registers[x] = self.registers[y];
        return 200;
    }

    // 0x8XY6
    fn opShiftRight(self: *Self, x: u16) u16 {
        var flag = self.registers[x] & 0x1;
        self.registers[x] >>= 1;
        self.registers[0xF] = flag;
        return 200;
    }

    // 0x8XYE
    fn opShiftLeft(self: *Self, x: u16) u16 {
        var flag = (self.registers[x] & 0x80) >> 7;
        self.registers[x] <<= 1;
        self.registers[0xF] = flag;
        return 200;
    }

    // 0x8XY5
    fn opSubtractYFromX(self: *Self, x: u16, y: u16) u16 {
        var vx = self.registers[x];
        var vy = self.registers[y];
        var borrow: u8 = 0;
        if (vx > vy) {
            borrow = 1;
        }
        self.registers[x] = vx -% vy;
        self.registers[0xF] = borrow;
        return 200;
    }

    // 0x8XY7
    fn opSubtractXFromY(self: *Self, x: u16, y: u16) u16 {
        var vx = self.registers[x];
        var vy = self.registers[y];
        var flag: u8 = 0;
        if (vy > vx) {
            flag = 1;
        }
        self.registers[x] = vy -% vx;
        self.registers[0xF] = flag;
        return 200;
    }

    // 0x8XY4
    fn opAddWithCarry(self: *Self, x: u8, y: u8) u16 {
        var sum = @intCast(u16, self.registers[x]) + @intCast(u16, self.registers[y]);
        if (sum > 255) {
            self.registers[0xF] = 1;
        } else {
            self.registers[0xF] = 0;
        }
        self.registers[x] = @intCast(u8, sum & 0xFF);
        return 200;
    }

    // 0x8XY1
    fn opBinaryOr(self: *Self, x: u16, y: u16) u16 {
        self.registers[x] |= self.registers[y];
        return 200;
    }

    // 0x8XY3
    fn opBinaryXor(self: *Self, x: u16, y: u16) u16 {
        self.registers[x] ^= self.registers[y];
        return 200;
    }

    // 0x8XY2
    fn opBinaryAnd(self: *Self, x: u16, y: u16) u16 {
        self.registers[x] &= self.registers[y];
        return 200;
    }

    // 0x000E
    fn opReturn(self: *Self) u16 {
        self.pc = self.index;
        return 1;
    }

    // 0x2NNN
    fn opJumpToSubroutine(self: *Self, addr: u16) u16 {
        if (self.sp < self.stack.len) {
            self.stack[self.sp] = self.pc;
            self.sp += 1;
            self.pc = addr;
        }
        return 105;
    }

    // 0x00EE
    fn opReturnFromSubroutine(self: *Self) u16 {
        if (self.sp > 0) {
            self.sp -= 1;
            self.pc = self.stack[self.sp];
        }
        return 105;
    }
};
