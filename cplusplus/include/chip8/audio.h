#pragma once

namespace Chip8
{
    class Audio
    {
    private:
        
    public:
        Audio();
        ~Audio();

        void play();
        void pause();
        bool isPlaying();
    };
} // namespace Chip8
