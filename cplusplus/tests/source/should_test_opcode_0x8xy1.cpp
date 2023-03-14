#include "tests_common.h"

int main() {
    // arrange
    auto memory = std::make_shared<Chip8::Memory>();
    auto registers = std::make_shared<Chip8::Registers>();
    auto cpu = std::make_shared<Chip8::CPU>(memory, registers);
    uint8_t data[] = { 0x60, 0x01, 0x61, 0x06, 0x80, 0x11 };
    memory->load(512, data, sizeof(data));
    
    // act
    emulate(cpu, sizeof(data));

    // assert
    assert(0x7 == registers->get(0));
}
