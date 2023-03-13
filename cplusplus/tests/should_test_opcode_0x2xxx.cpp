#include "tests_common.h"

int main() {
    // arrange
    uint8_t data[4] = { 0x22, 0x02, 0x00, 0xEE };
    auto memory = std::make_shared<Chip8::Memory>();
    memory->load(512, data, sizeof(data));
    auto registers = std::make_shared<Chip8::Registers>();
    auto cpu = std::make_shared<Chip8::CPU>(memory, registers);

    // act
    emulate(cpu, sizeof(data));

    // assert
    assert(514 == cpu->getPc());
}
