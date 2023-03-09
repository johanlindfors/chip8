#pragma once
#include <cstdint>

namespace Chip8
{
    class Registers
    {
    private:
        char _registers[16];

    public:    
        Registers();
        ~Registers();
        void set(uint8_t index, uint8_t value);
        uint8_t get(uint8_t index);
    };
    
} // namespace Chip8
