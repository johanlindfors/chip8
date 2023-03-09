#include "chip8/keyboard.h"

using namespace Chip8;
    
Keyboard::Keyboard()
{
    for (int i = 0; i < 16; i++)
    {
        _keypad[i] = false;
    }   
}

Keyboard::~Keyboard()
{
    
}

void Keyboard::update()
{
    for (int i = 0; i < 16; i++)
    {
        _lastState[i] = _keypad[i];
    }   
}

bool Keyboard::hasBeenReleased(uint8_t key)
{
    return _lastState[key] && _keypad[key]== false;
}

bool Keyboard::isKeyPressed(uint8_t key)
{
    return _keypad[key];
}

void Keyboard::handleKeyDown(SDL_Keycode key) {
    printf("KeyDown: %d\n", key);
    switch(key) {
        case SDLK_1: _keypad[0x1] = true; break;
        case SDLK_2: _keypad[0x2] = true; break;
        case SDLK_3: _keypad[0x3] = true; break;
        case SDLK_4: _keypad[0xC] = true; break;

        case SDLK_q: _keypad[0x4] = true; break;
        case SDLK_w: _keypad[0x5] = true; break;
        case SDLK_e: _keypad[0x6] = true; break;
        case SDLK_r: _keypad[0xD] = true; break;

        case SDLK_a: _keypad[0x7] = true; break;
        case SDLK_s: _keypad[0x8] = true; break;
        case SDLK_d: _keypad[0x9] = true; break;
        case SDLK_f: _keypad[0xE] = true; break;

        case SDLK_z: _keypad[0xA] = true; break;
        case SDLK_x: _keypad[0x0] = true; break;
        case SDLK_c: _keypad[0xB] = true; break;
        case SDLK_v: _keypad[0xF] = true; break;
        default: break;
    }
}


void Keyboard::handleKeyUp(SDL_Keycode key) {
    switch(key) {
        case SDLK_1: _keypad[0x1] = false; break;
        case SDLK_2: _keypad[0x2] = false; break;
        case SDLK_3: _keypad[0x3] = false; break;
        case SDLK_4: _keypad[0xC] = false; break;

        case SDLK_q: _keypad[0x4] = false; break;
        case SDLK_w: _keypad[0x5] = false; break;
        case SDLK_e: _keypad[0x6] = false; break;
        case SDLK_r: _keypad[0xD] = false; break;

        case SDLK_a: _keypad[0x7] = false; break;
        case SDLK_s: _keypad[0x8] = false; break;
        case SDLK_d: _keypad[0x9] = false; break;
        case SDLK_f: _keypad[0xE] = false; break;

        case SDLK_z: _keypad[0xA] = false; break;
        case SDLK_x: _keypad[0x0] = false; break;
        case SDLK_c: _keypad[0xB] = false; break;
        case SDLK_v: _keypad[0xF] = false; break;
        default: break;
    }
}
