#include <stdio.h>
#include "chip8/emulator.h"
#include "chip8/cpu.h"
#include "chip8/memory.h"
#include "chip8/registers.h"
#include "chip8/display.h"
#include "chip8/keyboard.h"
#include "chip8/audio.h"

using namespace Chip8;

Emulator::Emulator(char* data, int length) 
{
    auto memory = std::make_shared<Memory>();
    auto registers = std::make_shared<Registers>();
    _cpu = std::make_unique<CPU>(memory, registers);

    _display = std::make_shared<Display>();
    _keyboard = std::make_shared<Keyboard>();
    _audio = std::make_shared<Audio>();
}

Emulator::~Emulator() {
    _cpu.reset();
}

void Emulator::run() {
    _display->init();

    //Hack to get window to stay up
    SDL_Event e; 
    bool quit = false; 
    while( quit == false ) { 
        while( SDL_PollEvent( &e ) ) { 
            if( e.type == SDL_QUIT ) 
                quit = true;
        }
    }
}
