#pragma once
#include <memory>

namespace Chip8 {
    class CPU;
    class Display;
    class Keyboard;
    class Audio;

    class Emulator {
        public:
            Emulator(char* data, int length);
            ~Emulator();
            void run();

        private:
            std::unique_ptr<CPU> _cpu;
            std::shared_ptr<Display> _display;
            std::shared_ptr<Keyboard> _keyboard;
            std::shared_ptr<Audio> _audio;
    };
}