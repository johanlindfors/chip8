#pragma once
#include <cassert>
#include <memory>
#include "../include/chip8/memory.h"
#include "../include/chip8/registers.h"
#include "../include/chip8/cpu.h"

void emulate(std::shared_ptr<Chip8::CPU> cpu, int length) {
    auto iterations = length / 2;
    while(iterations-- > 0) {
        cpu->emulateCycle(nullptr, nullptr);
    }
}
