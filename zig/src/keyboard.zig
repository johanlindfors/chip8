const std = @import("std");
const sdl = @cImport({
    @cInclude("SDL2/SDL.h");
});

pub const Keyboard = struct {
    const Self = @This();
    keypad: [16]bool,
    lastState: [16]bool,

    pub fn init() Self {
        return Keyboard {
            .keypad = [_]bool{false} ** 16,
            .lastState = [_]bool{false} ** 16
        };
    }

    pub fn handleInput(self: *Self) void {
        var index: u16 = 0;

        while (index < self.keypad.len) {
            self.lastState[index] = self.keypad[index];
            index += 1;
        }
    }

    pub fn handleKeyDown(self: *Self, keycode: sdl.SDL_Keycode) void {
        switch (keycode) {
            sdl.SDLK_1 => self.keypad[0x1] = true,
            sdl.SDLK_2 => self.keypad[0x2] = true,
            sdl.SDLK_3 => self.keypad[0x3] = true,
            sdl.SDLK_4 => self.keypad[0xC] = true,

            sdl.SDLK_q => self.keypad[0x4] = true,
            sdl.SDLK_w => self.keypad[0x5] = true,
            sdl.SDLK_e => self.keypad[0x6] = true,
            sdl.SDLK_r => self.keypad[0xD] = true,

            sdl.SDLK_a => self.keypad[0x8] = true,
            sdl.SDLK_s => self.keypad[0x8] = true,
            sdl.SDLK_d => self.keypad[0x9] = true,
            sdl.SDLK_f => self.keypad[0xE] = true,

            sdl.SDLK_z => self.keypad[0xA] = true,
            sdl.SDLK_x => self.keypad[0x0] = true,
            sdl.SDLK_c => self.keypad[0xB] = true,
            sdl.SDLK_v => self.keypad[0xF] = true,
            else => {}
        }
    }

    pub fn handleKeyUp(self: *Self, keycode: sdl.SDL_Keycode) void {
        switch (keycode) {
            sdl.SDLK_1 => self.keypad[0x1] = false,
            sdl.SDLK_2 => self.keypad[0x2] = false,
            sdl.SDLK_3 => self.keypad[0x3] = false,
            sdl.SDLK_4 => self.keypad[0xC] = false,

            sdl.SDLK_q => self.keypad[0x4] = false,
            sdl.SDLK_w => self.keypad[0x5] = false,
            sdl.SDLK_e => self.keypad[0x6] = false,
            sdl.SDLK_r => self.keypad[0xD] = false,

            sdl.SDLK_a => self.keypad[0x8] = false,
            sdl.SDLK_s => self.keypad[0x8] = false,
            sdl.SDLK_d => self.keypad[0x9] = false,
            sdl.SDLK_f => self.keypad[0xE] = false,

            sdl.SDLK_z => self.keypad[0xA] = false,
            sdl.SDLK_x => self.keypad[0x0] = false,
            sdl.SDLK_c => self.keypad[0xB] = false,
            sdl.SDLK_v => self.keypad[0xF] = false,
            else => {}
        }
    }

    pub fn isKeyPressed(self: *Self, key: u8) bool {
        return self.keypad[key];
    }

    pub fn hasKeyBeenReleased(self: *Self, key: u8) bool {
        return self.lastState[key] and self.keypad[key] == false;
    }

    pub fn cleanUp(_: *Self) void {

    }
};