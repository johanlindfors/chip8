#include <stdio.h>
#include "chip8/emulator.h"
#include "chip8/cpu.h"
#include "chip8/memory.h"
#include "chip8/registers.h"
#include "chip8/display.h"
#include "chip8/keyboard.h"
#include "chip8/audio.h"
#include <chrono>

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
    if( SDL_Init( SDL_INIT_EVERYTHING ) < 0 )
    {
        printf( "SDL could not initialize! SDL_Error: %s\n", SDL_GetError() );
    }
    
    _display->init();

    SDL_Event e; 
    bool quit = false; 
    auto lastCycleTime = std::chrono::high_resolution_clock::now();
    while( quit == false ) {
        _keyboard->update();
        auto currentTime = std::chrono::high_resolution_clock::now();
        
        while( SDL_PollEvent( &e ) ) {
            // printf( "SDL event! %d\n", e.type);
            switch (e.type) {
                case SDL_QUIT:
                    quit = true;
                    break;
                case SDL_KEYDOWN:
                    _keyboard->handleKeyDown(e.key.keysym.sym);
                    break;
                case SDL_KEYUP:
                    _keyboard->handleKeyUp(e.key.keysym.sym);
                    break;
                default:
                    break;
            }
        }
        _cpu->tick(_display, _keyboard, _audio);
        if(_display->getDrawFlag()){
            _display->draw();
        }
    }
}
