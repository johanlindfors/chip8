const std = @import("std");
const sdl = @cImport({
    @cInclude("SDL2/SDL.h");
});

pub const IDisplay = struct {
    initializeSDLFn: fn (*IDisplay) void,
    drawFn: fn (*IDisplay) void,
    clearFn: fn (*IDisplay) void,
    setDrawFlagFn: fn (*IDisplay, bool) void,
    flipPixelFn: fn (*IDisplay, usize) void,

    pub fn initializeSDL(display: *IDisplay) void {
        return display.initializeSDLFn(display);
    }

    pub fn draw(display: *IDisplay) i32 {
        return display.drawFn(display);
    }

    pub fn clear(display: *IDisplay) void {
        display.clearFn(display);
    }

    pub fn setDrawFlag(display: *IDisplay, value: bool) void {
        display.setDrawFlagFn(display, value);
    }

    pub fn flipPixel(display: *IDisplay, position: usize) void {
        display.flipPixelFn(display, position);
    }
};

// pub const FakeDisplay = struct {
//     interface: IDisplay,

//     const Self = @This();

//     pub fn init() Self {
//         return FakeDisplay{ .interface = IDisplay{ .drawFn = draw, .clearFn = clear, .setDrawFlagFn = setDrawFlag, .flipPixelFn = flipPixel } };
//     }

//     pub fn draw(_: *IDisplay) void {}

//     pub fn clear(_: *IDisplay) void {}

//     pub fn setDrawFlag(_: *IDisplay, _: bool) void {}

//     pub fn flipPixel(_: *IDisplay, _: usize) void {}
// };

pub const Display = struct {
    frameBuffer: [64 * 32]bool,
    renderer: ?*sdl.SDL_Renderer,
    window: ?*sdl.SDL_Window,
    drawFlag: bool,

    interface: IDisplay,

    const Self = @This();

    pub fn init() Self {
        // _ = sdl.SDL_Init(sdl.SDL_INIT_VIDEO);

        // const window = sdl.SDL_CreateWindow("hello gamedev", sdl.SDL_WINDOWPOS_CENTERED, sdl.SDL_WINDOWPOS_CENTERED, 640, 320, 0);

        // const renderer = sdl.SDL_CreateRenderer(window, 0, sdl.SDL_RENDERER_PRESENTVSYNC);

        return Display{ .frameBuffer = [_]bool{false} ** 2048, .window = null, .renderer = null, .drawFlag = false, .interface = IDisplay{ .initializeSDLFn = initializeSDL, .drawFn = draw, .clearFn = clear, .setDrawFlagFn = setDrawFlag, .flipPixelFn = flipPixel } };
    }

    pub fn initializeSDL(idisplay: *IDisplay) void {
        _ = sdl.SDL_Init(sdl.SDL_INIT_VIDEO);
        const self: *Display = @fieldParentPtr("interface", idisplay);
        self.window = sdl.SDL_CreateWindow("hello gamedev", sdl.SDL_WINDOWPOS_CENTERED, sdl.SDL_WINDOWPOS_CENTERED, 640, 320, 0);
        self.renderer = sdl.SDL_CreateRenderer(self.window, 0, sdl.SDL_RENDERER_PRESENTVSYNC);
    }

    pub fn draw(idisplay: *IDisplay) void {
        const self: *Display = @fieldParentPtr("interface", idisplay);

        _ = sdl.SDL_SetRenderDrawColor(self.renderer, 0x00, 0x00, 0x00, 0xff);
        _ = sdl.SDL_RenderClear(self.renderer);

        _ = sdl.SDL_SetRenderDrawColor(self.renderer, 0xff, 0xff, 0xff, 0xff);
        var y: usize = 0;
        while (y < 32) : (y += 1) {
            var x: usize = 0;
            while (x < 64) : (x += 1) {
                if (self.frameBuffer[y * 64 + x]) {
                    var rect = sdl.SDL_Rect{ .x = @intCast(x * 10), .y = @intCast(y * 10), .w = 10, .h = 10 };
                    _ = sdl.SDL_RenderFillRect(self.renderer, &rect);
                }
            }
        }
        _ = sdl.SDL_RenderPresent(self.renderer);
        self.setDrawFlag(false);
    }

    pub fn clear(idisplay: *IDisplay) void {
        const self: *Display = @fieldParentPtr("interface", idisplay);

        self.frameBuffer = [_]bool{false} ** 2048;
    }

    pub fn setDrawFlag(idisplay: *IDisplay, value: bool) void {
        const self: *Display = @fieldParentPtr("interface", idisplay);

        self.drawFlag = value;
    }

    pub fn flipPixel(idisplay: *IDisplay, position: usize) void {
        const self: *Display = @fieldParentPtr("interface", idisplay);

        self.frameBuffer[position] = !self.frameBuffer[position];
    }

    pub fn cleanUp(self: *Self) void {
        sdl.SDL_DestroyRenderer(self.renderer);
        sdl.SDL_DestroyWindow(self.window);
    }
};
