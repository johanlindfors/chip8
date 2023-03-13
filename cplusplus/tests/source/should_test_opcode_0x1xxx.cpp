#include "tests_common.h"

int main() {
    // arrange
    uint8_t data[4] = { 0x12, 0x02, 0x60, 0x01 };
    auto memory = std::make_shared<Chip8::Memory>();
    memory->load(512, data, 4);
    auto registers = std::make_shared<Chip8::Registers>();
    auto cpu = std::make_shared<Chip8::CPU>(memory, registers);

    // act
    emulate(cpu, 4);

    // assert
    assert(0x1 == registers->get(0));
}
