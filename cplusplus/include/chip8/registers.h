#pragma once

namespace Chip8
{
    class Registers
    {
    private:
        char _registers[16];

    public:    
        Registers();
        ~Registers();
        void set(int index, char value);
        char get(int index);
    };
    
} // namespace Chip8
