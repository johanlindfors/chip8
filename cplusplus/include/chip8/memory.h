#pragma once

namespace Chip8 {
    class Memory {
        public:
            Memory();
            void set(int addr, char value);
            char get(int addr);
            void load(int addr, char* data, int length);

        private:
            char _ram[4096]; 
    };
}