#include "tests_common.h"

int main() {
    // arrange
    auto memory = std::make_shared<Chip8::Memory>();
    auto registers = std::make_shared<Chip8::Registers>();
    auto cpu = std::make_shared<Chip8::CPU>(memory, registers);
    uint8_t data[] = { 0x22, 0x02, 0x00, 0xEE };
    memory->load(512, data, sizeof(data));
    
    // act
    emulate(cpu, sizeof(data));

    // assert
    assert(514 == cpu->getPc());
}
