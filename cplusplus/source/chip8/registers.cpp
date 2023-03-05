#include "chip8/registers.h"

using namespace Chip8;

Registers::Registers()
{
    for (int i = 0; i < 16; i++)
    {
        _registers[i] = 0x0;
    }
}

Registers::~Registers()
{
}

void Registers::set(int index, char value) 
{
    _registers[index] = value;
}

char Registers::get(int index) 
{
    return _registers[index];
}
