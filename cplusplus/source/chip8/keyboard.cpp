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
