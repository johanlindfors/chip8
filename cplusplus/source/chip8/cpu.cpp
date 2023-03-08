#include "chip8/cpu.h"
#include "chip8/display.h"
#include "chip8/keyboard.h"
#include "chip8/audio.h"
#include "chip8/memory.h"
#include "chip8/registers.h"

using namespace std;
using namespace Chip8;

void CPU::tick(
    shared_ptr<Display> display, 
    shared_ptr<Keyboard> keyboard, 
    shared_ptr<Audio> audio)
{
    if(_delayTimer > 0) {
        _delayTimer--;
    }
    if(_soundTimer > 0) {
        if(!audio->isPlaying()) {
            audio->play();
        }
        _soundTimer--;        
    } else {
        if(audio != nullptr && audio->isPlaying()) {
            audio->pause();
        }
    }

    _microSeconds += 16666;
    while(_microSeconds > 0) {
        auto delta = emulateCycle(display, keyboard);
        if(delta == 0) {
            break;
        }
        _microSeconds -= delta;
    }
}

int CPU::emulateCycle(
    shared_ptr<Display> display,
    shared_ptr<Keyboard> keyboard)
{
    auto opcode = getOpcode();
    return execute(opcode, display, keyboard);    
}

uint16_t CPU::getOpcode()
{
    return _memory->get(_pc++) << 8 | _memory->get(_pc++);
}

int CPU::execute(
    uint16_t opcode,
    shared_ptr<Display> display,
    shared_ptr<Keyboard> keyboard)
{
    uint8_t x = (opcode & 0x0F00) >> 8;
    uint8_t y = (opcode & 0x00F0) >> 4;

    uint8_t n = opcode & 0x000F;
    uint8_t nn = opcode & 0x00FF;
    uint16_t nnn = opcode & 0x0FFF;

    printf("OPCODE: %#04X\n", opcode);
    
    switch (opcode & 0xF000)
    {
        case 0x0000:
            switch (opcode & 0x00FF)
            {
                case 0x00E0: return opClearScreen(display);
                case 0x000E: return opReturn();
                case 0x00EE: return opReturnFromSubroutine();
            }
        case 0x1000: return opJump(nnn);
        case 0x6000: return opSetRegisterVxToNn(x, nn);
        case 0x7000: return opAddNnToRegisterVx(x, nn);
        case 0xA000: return opSetIndexRegister(nnn);
        case 0xD000: return opDisplay(x, y, n, display);
        default: return 0;
    }   
}

// 0x1000
int CPU::opJump(uint16_t addr)
{
    printf("Jump to %#04x,\n", addr);
    _pc = addr;
    return 105;
}

// 0x00E0
int CPU::opClearScreen(shared_ptr<Display> display)
{
    printf("ClearScreen,\n");
    display->clear();
    return 109;
}

// 0x000E
int CPU::opReturn()
{
    printf("Return,\n");
    _pc = _index;
    return 1;
}

// 0x00EE
int CPU::opReturnFromSubroutine()
{
    printf("Return from subroutine,\n");
    if(_sp > 0) {
        _sp--;
        _sp = _stack[_sp];
    }
    return 0;
}

// 0x6XNN
int CPU::opSetRegisterVxToNn(uint8_t x, uint8_t nn)
{
    printf("Setting register %d to value: %d,\n", x, nn);
    _registers->set(x, nn);
    return 27;
}

// 0x7XNN
int CPU::opAddNnToRegisterVx(uint8_t x, uint8_t nn)
{
    auto vx = _registers->get(x);
    auto vy = nn;
    while(vy != 0) {
        auto carry = vx & vy;
        vx ^= vy;
        vy = carry << 1;
    }
    _registers->set(x, vx);
    return 45;
}

// 0xANNN
int CPU::opSetIndexRegister(uint16_t nnn)
{
    _index = nnn;
    return 55;
}

// 0xDXYN
int CPU::opDisplay(uint8_t x, uint8_t y, uint8_t n, std::shared_ptr<Display> display)
{
    auto index = _index;
    auto vx = _registers->get(x);
    auto vy = _registers->get(y);
    auto rows = 32;
    auto cols = 64;

    printf("Rendering a %d pixel tall sprite at X: %d, Y: %d from the address: %d\n", n, vx, vy, index);

    for(auto row = vy; row < vy + n; row++) {
        auto startX = vx + row * cols;
        auto endX = startX + 8;
        uint8_t bitMask = 128;
        for(auto col = startX; col < endX; col++) {
            if((col % cols) >= (startX % cols)) {
                auto position = (row * cols) + (col % cols);
                if((_memory->get(index) & bitMask) == bitMask) {
                    display->flipPixel(position);
                }
            }
            bitMask /= 2;
        }
        index++;
    }
    display->setDrawFlag(true);

    return 22734;
}

// 0xFX0A
int CPU::opGetKey(uint16_t x, shared_ptr<Keyboard> keyboard)
{
	for(auto i = 0; i < 16; i++) {
		if(keyboard->hasBeenReleased(i)) {
			_registers->set(x, i);
			return 1;
		}
	}
	_pc -= 2;
	return 1;
}
