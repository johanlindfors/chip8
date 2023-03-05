const sdl = @cImport({
    @cInclude("SDL2/SDL.h");
});

const Display = @import("display.zig").Display;
const Keyboard = @import("keyboard.zig").Keyboard;

pub const Emulator = struct {
    display: Display,
    keyboard: Keyboard,

    const Self = @This();

    pub fn init() Self {
        return Emulator{
            .display = Display.init(),
            .keyboard = Keyboard.init(),
        };
    }

    pub fn run(self: *Self) void {
        
        mainloop: while (true) {
            self.keyboard.handleInput();
            self.display.draw();

            var sdl_event: sdl.SDL_Event = undefined;
            while (sdl.SDL_PollEvent(&sdl_event) != 0) {
                switch (sdl_event.type) {
                    sdl.SDL_KEYDOWN => {
                        if (sdl_event.key.keysym.sym == sdl.SDLK_ESCAPE) {
                            break :mainloop;
                        } else { 
                            self.keyboard.handleKeyDown(sdl_event.key.keysym.sym);
                        }
                    },
                    sdl.SDL_KEYUP => self.keyboard.handleKeyUp(sdl_event.key.keysym.sym),
                    sdl.SDL_QUIT => break :mainloop,
                    else => {},
                }
            }
        }

        self.display.cleanUp();
        sdl.SDL_Quit();       
    }
};
 