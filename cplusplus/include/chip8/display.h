#pragma once

#include <SDL2/SDL.h>

namespace Chip8 {
    class Display {
        public:
            Display() = default;
            ~Display() {
                SDL_DestroyWindow( _window );
            }
            void init();
            void flipPixel(int index);
            void setDrawFlag(bool value);
            bool getDrawFlag();

            void clear();
            void draw();

        private:
            bool _drawFlag;
            bool _frameBuffer[64*32];
            SDL_Window* _window = nullptr;
            SDL_Renderer* _renderer = nullptr;

    };
}