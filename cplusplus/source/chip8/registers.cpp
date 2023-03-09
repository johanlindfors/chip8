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

void Registers::set(uint8_t index, uint8_t value) 
{
    _registers[index] = value;
}

uint8_t Registers::get(uint8_t index) 
{
    return _registers[index];
}
