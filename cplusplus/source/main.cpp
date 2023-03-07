#include <memory>
#include "chip8/emulator.h"
#include "chip8/memory.h"
#include "chip8/cpu.h"

int main(int argc, char* argv[]) {
    char* romFilename = argv[1];
    auto memory = std::make_shared<Chip8::Memory>();
    memory->loadROM(romFilename);
    auto emulator = std::make_unique<Chip8::Emulator>(memory);
    emulator->run(); 
    emulator.reset();
}
