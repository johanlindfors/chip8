#pragma once
#include <memory>
#include <random>
#include <chrono>
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
                , randGen(std::chrono::system_clock::now().time_since_epoch().count())
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

            int emulateCycle(
                std::shared_ptr<Display> display,
                std::shared_ptr<Keyboard> keyboard);

            uint16_t getPc() { return _pc; }
            uint16_t getIndex() { return _index; }
            void setDelayTimer(uint8_t value) { _delayTimer = value; }
            uint8_t getDelayTimer() { return _delayTimer; }
            uint8_t getSoundTimer() { return _soundTimer; }

        private:
            uint16_t getOpcode();

            int execute(
                uint16_t opcode,
                std::shared_ptr<Display> display,
                std::shared_ptr<Keyboard> keyboard);

            int opJump(uint16_t nnn);
            int opClearScreen(std::shared_ptr<Display> display);
            int opReturn();
            int opReturnFromSubroutine();
            int opSetRegisterVxToNn(uint8_t x, uint8_t nn);
            int opAddNnToRegisterVx(uint8_t x, uint8_t nn);
            int opSetIndexRegister(uint16_t nnn);
            int opDisplay(uint8_t x, uint8_t y, uint8_t n, std::shared_ptr<Display> display);
            int opGetKey(uint8_t x, std::shared_ptr<Keyboard> keyboard);
            int opFontCharacter(uint8_t x);
            int opStoreRegistersToMemory(uint8_t x);
            int opLoadRegistersFromMemory(uint8_t x);
            int opBinaryCodeDecimalConversion(uint8_t x);
            int opAddToIndex(uint8_t x);
            int opGetDelayTimer(uint8_t x);
            int opSetDelayTimer(uint8_t x);
            int opSetSoundTimer(uint8_t x);
            int opSkipIfVxEqualsNn(uint8_t x, uint8_t nn);
            int opSkipIfVxNotEqualsNn(uint8_t x, uint8_t nn);
            int opSkipIfVxNotEqualsVy(uint8_t x, uint8_t y);
            int opSkipIfVxEqualsVy(uint8_t x, uint8_t y);
            int opSkipIfKeyPressed(uint8_t x, std::shared_ptr<Keyboard> keyboard);
            int opSkipIfNotKeyPressed(uint8_t x, std::shared_ptr<Keyboard> keyboard);
            int opRandom(uint8_t x, uint8_t nn);
            int opJumpWithOffset(uint16_t nn);
            int opSetVxToValueOfVy(uint8_t x, uint8_t y);
            int opShiftRight(uint8_t x);
            int opShiftLeft(uint8_t x);
            int opSubtractVyFromVx(uint8_t x, uint8_t y);
            int opSubtractVxFromVy(uint8_t x, uint8_t y);
            int opAddWithCarry(uint8_t x, uint8_t y);
            int opBinaryOr(uint8_t x, uint8_t y);
            int opBinaryXor(uint8_t x, uint8_t y);
            int opBinaryAnd(uint8_t x, uint8_t y);
            int opJumpToSubroutine(uint16_t nnn);

            uint16_t _pc;
            uint16_t _index;
            uint8_t _sp;
            uint16_t _stack[16];
            uint8_t _delayTimer;
            uint8_t _soundTimer;
            int _microSeconds;
            std::shared_ptr<Memory> _memory;
            std::shared_ptr<Registers> _registers;

            std::default_random_engine randGen;
            std::uniform_int_distribution<uint8_t> randByte;
    };
}
