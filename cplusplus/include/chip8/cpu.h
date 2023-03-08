#pragma once
#include <memory>
#include "chip8/memory.h"

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
                : _pc(PROGRAM_START_ADDRESS)
                , _index(0)
                , _sp(0)
                , _delayTimer(0)
                , _soundTimer(0)
                , _microSeconds(0)
                , _memory(memory)
                , _registers(registers)
            { 
                for (int i = 0; i < 16; i++)
                {
                    _stack[i] = 0;
                }
                
            }

            void tick(
                std::shared_ptr<Display> display,
                std::shared_ptr<Keyboard> keyboard,
                std::shared_ptr<Audio> audio);

        private:
            int emulateCycle(
                std::shared_ptr<Display> display,
                std::shared_ptr<Keyboard> Keyboard);

            uint16_t getOpcode();

            int execute(
                uint16_t opcode,
                std::shared_ptr<Display> display,
                std::shared_ptr<Keyboard> keyboard);

            int opJump(uint16_t addr);
            int opClearScreen(std::shared_ptr<Display> display);
            int opReturn();
            int opReturnFromSubroutine();
            int opSetRegisterVxToNn(uint8_t x, uint8_t nn);
            int opAddNnToRegisterVx(uint8_t x, uint8_t nn);
            int opSetIndexRegister(uint16_t nnn);
            int opDisplay(uint8_t x, uint8_t y, uint8_t n, std::shared_ptr<Display> display);
            int opGetKey(uint16_t x, std::shared_ptr<Keyboard> keyboard);

            int _pc;
            int _index;
            int _sp;
            int _stack[16];
            int _delayTimer;
            int _soundTimer;
            int _microSeconds;
            std::shared_ptr<Memory> _memory;
            std::shared_ptr<Registers> _registers;
    };
}
