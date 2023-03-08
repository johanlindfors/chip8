#pragma once

namespace Chip8
{
    class Keyboard
    {
    public:
        Keyboard();
        ~Keyboard();
        void update();
                
    private:
        bool _keypad[16];
        bool _lastState[16];
    };   
}