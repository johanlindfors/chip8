#pragma once
#include <memory>

namespace Chip8 {
    class Memory;
    class Registers;
    class Display;
    class Keyboard;
    class Audio;

    class CPU {
        public:
            CPU(std::shared_ptr<Memory> memory,
                std::shared_ptr<Registers> registers) 
                : _pc(0) 
                , _memory(memory)
                , _registers(registers)
            { }

            void tick(
                std::shared_ptr<Display> display,
                std::shared_ptr<Keyboard> keyboard,
                std::shared_ptr<Audio> audio);

        private:
            int _pc;
            std::shared_ptr<Memory> _memory;
            std::shared_ptr<Registers> _registers;
    };
}
