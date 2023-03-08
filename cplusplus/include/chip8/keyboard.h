#pragma once
#include <cstdint>

namespace Chip8
{
    class Keyboard
    {
    public:
        Keyboard();
        ~Keyboard();
        void update();

        bool hasBeenReleased(uint8_t key);
        bool isKeyPressed(uint8_t key);

    private:
        bool _keypad[16];
        bool _lastState[16];
    };   
}
