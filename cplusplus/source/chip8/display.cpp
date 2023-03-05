#include "chip8/display.h"

using namespace Chip8;

void Display::init()
{    
    //Initialize SDL
    if( SDL_Init( SDL_INIT_VIDEO ) < 0 )
    {
        printf( "SDL could not initialize! SDL_Error: %s\n", SDL_GetError() );
    } else {
        //Create window
        _window = SDL_CreateWindow( "SDL Tutorial", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, 640, 320, SDL_WINDOW_SHOWN );
        if( _window == NULL )
        {
            printf( "Window could not be created! SDL_Error: %s\n", SDL_GetError() );
        } else {
            //Get window surface
            _screenSurface = SDL_GetWindowSurface( _window );

            //Fill the surface white
            SDL_FillRect( _screenSurface, NULL, SDL_MapRGB( _screenSurface->format, 0xFF, 0xFF, 0xFF ) );
            
            //Update the surface
            SDL_UpdateWindowSurface( _window );
        }
    }
}

void Display::flipPixel(int x, int y)
{
    auto index = y * 64 + x;
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

}

void Display::draw()
{

}
