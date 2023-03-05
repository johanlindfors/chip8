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
