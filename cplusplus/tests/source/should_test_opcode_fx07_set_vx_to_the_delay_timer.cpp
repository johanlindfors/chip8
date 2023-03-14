#include "tests_common.h"

int main() {
    // arrange
    auto memory = std::make_shared<Chip8::Memory>();
    auto registers = std::make_shared<Chip8::Registers>();
    auto cpu = std::make_shared<Chip8::CPU>(memory, registers);
    uint8_t data[] = { 0xF0, 0x07 };
    memory->load(512, data, sizeof(data));
    cpu->setDelayTimer(2);
    
    // act
    cpu->tick(nullptr, nullptr, nullptr);

    // assert
    assert(1 == registers->get(0));
}
