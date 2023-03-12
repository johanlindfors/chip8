#include "chip8/display.h"

using namespace Chip8;

void Display::init()
{    
    //Create window
    SDL_CreateWindowAndRenderer(640, 320, SDL_WINDOW_SHOWN, &_window, &_renderer);
    if( _window == NULL || _renderer == NULL)
    {
        printf( "Window or renderer could not be created! SDL_Error: %s\n", SDL_GetError() );
    }

}

void Display::flipPixel(int index)
{
    _frameBuffer[index] = !_frameBuffer[index];
}

void Display::setDrawFlag(bool value)
{
    _drawFlag = value;
}

bool Display::getDrawFlag()
{
    return _drawFlag;
}

void Display::clear()
{
    for(int i = 0; i < 64*32; i++){
        _frameBuffer[i] = false;
    }
}

void Display::draw()
{
    SDL_SetRenderDrawColor(_renderer, 0x00, 0x00, 0x00, 0xFF);
    SDL_RenderClear(_renderer);

    SDL_SetRenderDrawColor(_renderer, 0xFF, 0xFF, 0xFF, 0xFF);
    for (int y = 0; y < 32; y++) {
        for(int x = 0; x < 64; x++) {
            if(_frameBuffer[y * 64 + x]) {
                SDL_Rect rect = { x*10, y*10, 10, 10};
                SDL_RenderFillRect(_renderer, &rect);
            }
        }
    }
    
    SDL_RenderPresent(_renderer);
    _drawFlag = false;
}
