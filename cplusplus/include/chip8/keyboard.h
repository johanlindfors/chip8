#pragma once
#include <cstdint>
#include <SDL2/SDL.h>

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

        void handleKeyDown(SDL_Keycode key);
        void handleKeyUp(SDL_Keycode key);
    private:
        bool _keypad[16];
        bool _lastState[16];
    };   
}
