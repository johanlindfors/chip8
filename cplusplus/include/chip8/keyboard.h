#pragma once

namespace Chip8
{
    class Keyboard
    {
    public:
        Keyboard();
        ~Keyboard();
        
    private:
        bool _keypad[16];
    };   
}