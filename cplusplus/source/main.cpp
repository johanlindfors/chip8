#include <memory>
#include "chip8/emulator.h"
#include "chip8/memory.h"
#include "chip8/cpu.h"

int main() {
    auto emulator = std::make_unique<Chip8::Emulator>(nullptr, 0);
    emulator->run(); 
    // auto sdl = SDL_Init(SDL_INIT_EVERYTHING);
    // SDL_Quit();
    emulator.reset();
}
