#include <stdio.h>
#include "chip8/emulator.h"
#include "chip8/cpu.h"
#include "chip8/memory.h"
#include "chip8/registers.h"
#include "chip8/display.h"
#include "chip8/keyboard.h"
#include "chip8/audio.h"

using namespace std;
using namespace Chip8;

Emulator::Emulator(shared_ptr<Memory> memory)
{
    auto registers = make_shared<Registers>();
    _cpu = make_unique<CPU>(memory, registers);

    _display = make_shared<Display>();
    _keyboard = make_shared<Keyboard>();
    _audio = make_shared<Audio>();
}

Emulator::~Emulator()
{
    _display.reset();
    _keyboard.reset();
    _audio.reset();
    _cpu.reset();
}

void Emulator::run()
{
    _display->init();

    SDL_Event e; 
    bool quit = false; 
    while( quit == false ) {
        
        _cpu->tick(_display, _keyboard, _audio);
        
        while( SDL_PollEvent( &e ) ) { 
            if( e.type == SDL_QUIT ) 
                quit = true;
        }
        if(_display->getDrawFlag()){
            _display->draw();
        }
    }
}
