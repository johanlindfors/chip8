#include "chip8/memory.h"
#include <fstream>
#include <sstream>

using namespace Chip8;

uint8_t SPRITE_CHARS[80] =
{
    0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
    0x20, 0x60, 0x20, 0x20, 0x70, // 1
    0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
    0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
    0x90, 0x90, 0xF0, 0x10, 0x10, // 4
    0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
    0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
    0xF0, 0x10, 0x20, 0x40, 0x40, // 7
    0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
    0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
    0xF0, 0x90, 0xF0, 0x90, 0x90, // A
    0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
    0xF0, 0x80, 0x80, 0x80, 0xF0, // C
    0xE0, 0x90, 0x90, 0x90, 0xE0, // D
    0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
    0xF0, 0x80, 0xF0, 0x80, 0x80  // F
};


Memory::Memory() {
    for (int i = 0; i < 4096; i++) {
        _ram[i] = 0x00;
    }
    load(0x0000, SPRITE_CHARS, 80);
}

void Memory::set(uint16_t addr, uint8_t value) {
    _ram[addr] = value;
}

uint8_t Memory::get(uint16_t addr) {
    return _ram[addr];
}

void Memory::load(int addr, uint8_t* data, int length) {
    for (int i = 0; i < length; i++) {
        _ram[addr + i] = data[i];
    }
}

void Memory::loadROM(char const* filename)
{
	std::ifstream file(filename, std::ios::binary | std::ios::ate);
	if (file.is_open())
	{
		std::streampos size = file.tellg();
		char* buffer = new char[size];

		file.seekg(0, std::ios::beg);
		file.read(buffer, size);
		file.close();

		for (long i = 0; i < size; ++i)
		{
			_ram[PROGRAM_START_ADDRESS + i] = buffer[i];
            // printf("%02x\n", static_cast<uint8_t>(buffer[i]));
		}
        // _ram[0x1ff] = 4;
        printf("ROM Loaded...\n");
		delete[] buffer;
	}
}