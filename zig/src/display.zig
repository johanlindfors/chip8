const std = @import("std");
const sdl = @cImport({
    @cInclude("SDL2/SDL.h");
});

pub const Display = struct {
    frameBuffer: [64*32]bool,
    renderer: ?*sdl.SDL_Renderer,
    window: ?*sdl.SDL_Window,
    drawFlag: bool,

    const Self = @This();

    pub fn init() Self {
        _ = sdl.SDL_Init(sdl.SDL_INIT_VIDEO);
        
        var window = sdl.SDL_CreateWindow("hello gamedev", sdl.SDL_WINDOWPOS_CENTERED, sdl.SDL_WINDOWPOS_CENTERED, 640, 320, 0);

        var renderer = sdl.SDL_CreateRenderer(window, 0, sdl.SDL_RENDERER_PRESENTVSYNC);

        return Display {
            .frameBuffer = [_]bool{false} ** 2048,
            .renderer = renderer,
            .window = window,
            .drawFlag = false,
        };
    }

    pub fn draw(self: *Self) void {
        self.frameBuffer[892] = true;
        _ = sdl.SDL_SetRenderDrawColor(self.renderer, 0x00, 0x00, 0x00, 0xff);
        _ = sdl.SDL_RenderClear(self.renderer);

        _ = sdl.SDL_SetRenderDrawColor(self.renderer, 0xff, 0xff, 0xff, 0xff);
        var y: usize = 0;
        while (y < 32) {
            var x: usize = 0;
            while (x < 64) {
                if(self.frameBuffer[y * 64 + x]){
                    var rect = sdl.SDL_Rect{ .x = @intCast(i16, x * 10), .y = @intCast(i16, y * 10), .w = 10, .h = 10 };
                    _ = sdl.SDL_RenderFillRect(self.renderer, &rect);
                }
                x += 1;
            }
            y += 1;
        }
        _ = sdl.SDL_RenderPresent(self.renderer);
        self.drawFlag = false;
    }

    pub fn cleanUp(self: *Self) void {
        sdl.SDL_DestroyRenderer(self.renderer);
        sdl.SDL_DestroyWindow(self.window);
    }
};