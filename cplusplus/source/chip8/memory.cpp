#include "chip8/memory.h"

using namespace Chip8;

Memory::Memory() {
    for (int i = 0; i < 4096; i++) {
        _ram[i] = 0;
    }
}

void Memory::set(int addr, char value) {
    _ram[addr] = value;
}

char Memory::get(int addr) {
    return _ram[addr];
}

void Memory::load(int addr, char* data, int length) {
    for (int i = 0; i < length; i++) {
        _ram[addr + i] = data[i];
    }
}