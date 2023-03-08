#pragma once
#include <cstdint>

namespace Chip8 {

    const uint16_t SPRITE_CHARS_ADDR = 0x0000;
    const uint32_t FRAME_TICKS = 16666;
    const uint16_t RAM_SIZE= 4096;
    const uint16_t PROGRAM_START_ADDRESS = 0x0200;
    const uint16_t REGISTER_COUNT = 16;
    const uint8_t STACK_DEPTH = 16;
    const uint8_t SCALE = 10;
    const uint8_t COLS = 64;
    const uint8_t ROWS = 32;

    class Memory {
        public:
            Memory();
            void set(int addr, char value);
            char get(int addr);
            void load(int addr, uint8_t* data, int length);
            void loadROM(char const* filename);

        private:
            char _ram[4096]; 
    };
}
