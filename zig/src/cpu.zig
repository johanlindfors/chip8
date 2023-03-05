const std = @import("std");
const Memory = @import("memory.zig").Memory;
const Display = @import("display.zig").Display;
const Keyboard = @import("keyboard.zig").Keyboard;
const Audio = @import("audio.zig").Audio;

pub const CPU = struct {
    memory: Memory,
    delayTimer: u8,
    soundTimer: u8,
    microseconds: i32,
    pc: u16,
    registers: [16]u8,
    index: u16,

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

    fn execute(self: *Self, opcode: u16, display: *Display, _: *Keyboard) i32 {
        var x: u8 = @intCast(u8, opcode >> 8 & 0x0F);
        var y: u8 = @intCast(u8, opcode >> 4 & 0x0F);
        var n: u8 = @intCast(u8, opcode & 0x000F);
        var nn: u8 = @intCast(u8, opcode & 0x00FF);
        var nnn: u16 = opcode & 0x0FFF;
        switch(opcode & 0xF000) {
            0x0000 => switch (opcode & 0x00FF) {
                    0x00E0 => return opClearScreen(display),
                    else => return 0,
                },
            0x1000 => return self.opJump(nnn),
            0x6000 => return self.opSetRegisterXToNN(x, nn),
            0x7000 => return self.opAddNNToX(x, nn),
            0xA000 => return self.opSetIndexToNNN(nnn),
            0xD000 => return self.opDisplay(x, y, n, display),
            else => return 0,
        }
    }

    // 0x00E0
    fn opClearScreen(display: *Display) i32 {
        display.clear();
        return 100;
    }

    // 0x1NNN
    fn opJump(self: *Self, nnn: u16) i32 {
        self.pc = nnn;
        return 105; 
    }

    // 0x6XNN
    fn opSetRegisterXToNN(self: *Self, x: u8, nn: u8) i32 {
        self.registers[x] = nn;
        return 27;
    }

    // 0x7XNN
    fn opAddNNToX(self: *Self, x: u8, nn: u8) i32 {
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
    fn opSetIndexToNNN(self: *Self, nnn: u16) i32 {
        self.index = nnn;
        return 55;
    }

    // 0xDXYN
    fn opDisplay(self: *Self, x: u8, y: u8, n: u8, display: *Display) i32 {
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
};
