const sdl = @cImport({
    @cInclude("SDL2/SDL.h");
});

const Display = @import("display.zig").Display;
const Keyboard = @import("keyboard.zig").Keyboard;
const Audio = @import("audio.zig").Audio;
const CPU = @import("cpu.zig").CPU;

pub const Emulator = struct {
    display: Display,
    keyboard: Keyboard,
    audio: Audio,
    cpu: CPU,

    const Self = @This();

    pub fn init() Self {
        return Emulator{
            .display = Display.init(),
            .keyboard = Keyboard.init(),
            .audio = Audio {},
            .cpu = CPU.init(),
        };
    }

    pub fn run(self: *Self, program: []u8, length: usize) void {
        
        self.cpu.memory.loadProgram(0x0200, program, length);

        mainloop: while (true) {
            self.keyboard.handleInput();
            
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
            self.cpu.tick(&self.display, &self.keyboard, &self.audio);
            if(self.display.drawFlag) {
                self.display.draw();
            }
        }

        self.display.cleanUp();
        sdl.SDL_Quit();       
    }
};
 