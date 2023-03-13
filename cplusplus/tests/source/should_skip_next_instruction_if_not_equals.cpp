#include "tests_common.h"

int main() {
    // arrange
    auto memory = std::make_shared<Chip8::Memory>();
    auto registers = std::make_shared<Chip8::Registers>();
    auto cpu = std::make_shared<Chip8::CPU>(memory, registers);
    uint8_t data[] = { 0x60, 0x01, 0x40, 0x02 };
    memory->load(512, data, sizeof(data));

    // act
    emulate(cpu, sizeof(data));

    // assert
    assert(518 == cpu->getPc());
}
