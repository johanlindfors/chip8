#include "tests_common.h"

int main() {
    // arrange
    auto memory = std::make_shared<Chip8::Memory>();
    auto registers = std::make_shared<Chip8::Registers>();
    auto cpu = std::make_shared<Chip8::CPU>(memory, registers);
    uint8_t data[] = { 0x60, 0x01, 0x62, 0x02, 0x80, 0x20 };
    memory->load(512, data, sizeof(data));
    
    // act
    emulate(cpu, 4);

    // assert
    assert(0x1 == registers->get(0));
    emulate(cpu, 2);
    assert(0x2 == registers->get(0));
}
