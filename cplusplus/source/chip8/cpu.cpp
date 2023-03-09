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
            printf( "Break tick loop: \n" );
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

   //printf("OPCODE: %04X\n", opcode);
    
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
        case 0x2000: return opJumpToSubroutine(nnn);
        case 0x3000: return opSkipIfVxEqualsNn(x, nn);
        case 0x4000: return opSkipIfVxNotEqualsNn(x, nn);
        case 0x5000: return opSkipIfVxEqualsVy(x, y);
        case 0x6000: return opSetRegisterVxToNn(x, nn);
        case 0x7000: return opAddNnToRegisterVx(x, nn);
        case 0x8000:
            switch (opcode & 0x000F) {
                    case 0x0000: return opSetVxToValueOfVy(x, y);
                    case 0x0001: return opBinaryOr(x, y);
                    case 0x0002: return opBinaryAnd(x, y);
                    case 0x0003: return opBinaryXor(x, y);
                    case 0x0004: return opAddWithCarry(x, y);
                    case 0x0005: return opSubtractVyFromVx(x, y);
                    case 0x0006: return opShiftRight(x);
                    case 0x0007: return opSubtractVxFromVy(x, y);
                    case 0x000E: return opShiftLeft(x);
            }
        case 0x9000: return opSkipIfVxNotEqualsVy(x, y);
        case 0xA000: return opSetIndexRegister(nnn);
        case 0xB000: return opJumpWithOffset(nnn);
        case 0xC000: return opRandom(x, nn);
        case 0xD000: return opDisplay(x, y, n, display);
        case 0xE000:
            switch (opcode & 0x00FF) {
                case 0x009E: return opSkipIfKeyPressed(x, keyboard);
                case 0x00A1: return opSkipIfNotKeyPressed(x, keyboard);
            }
        case 0xF000:
            switch (opcode & 0x00FF) {
                case 0x0007: return opGetDelayTimer(x);
                case 0x000A: return opGetKey(x, keyboard);
                case 0x0015: return opSetDelayTimer(x);
                case 0x0018: return opSetSoundTimer(x);
                case 0x001E: return opAddToIndex(x);
                case 0x0029: return opFontCharacter(x);
                case 0x0033: return opBinaryCodeDecimalConversion(x);
                case 0x0055: return opStoreRegistersToMemory(x);
                case 0x0065: return opLoadRegistersFromMemory(x);
            }
        default: return 0;
    }   
}

// 0x1NNN
int CPU::opJump(uint16_t nnn)
{
    printf("Jump to %#04x,\n", nnn);
    _pc = nnn;
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
        _pc = _stack[_sp];
    }
    return 105;
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
    printf("Setting register I to %04x\n", nnn);
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
int CPU::opGetKey(uint8_t x, shared_ptr<Keyboard> keyboard)
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

// 0xFX29
int CPU::opFontCharacter(uint8_t x)
{
    _index = SPRITE_CHARS_ADDR + _registers->get(x);
    return 91;
}

// 0xFX55
int CPU::opStoreRegistersToMemory(uint8_t x)
{
    for (auto i=0; i <= x ; i++) {
        _memory->set(_index + i, _registers->get(i));
    }
    return 605 + x * 64;
}

// 0xFX65
int CPU::opLoadRegistersFromMemory(uint8_t x)
{
    for(auto i=0; i <= x; i++) {
        _registers->set(i, _memory->get(_index + i));
    }
    return 605 + x * 64;
}

// 0xFX33
int CPU::opBinaryCodeDecimalConversion(uint8_t x)
{
    auto vx = _registers->get(x);
    printf("%d\n", vx);
    _memory->set(_index, vx / 100);
    _memory->set(_index + 1, (vx / 10) % 10);
    _memory->set(_index + 2, vx % 10);
    printf("%d\n", _memory->get(_index));
    printf("%d\n", _memory->get(_index+1));
    printf("%d\n", _memory->get(_index+2));
    // exit(0);
    return 927;
}

// 0xFX1E
int CPU::opAddToIndex(uint8_t x)
{
    _index += _registers->get(x);
    return 86;
}

// 0xFX07
int CPU::opGetDelayTimer(uint8_t x)
{
    _registers->set(x, _delayTimer);
    return 45;
}

// 0xFX15
int CPU::opSetDelayTimer(uint8_t x)
{
    _delayTimer = _registers->get(x);
    return 45;
}

// 0xFX18
int CPU::opSetSoundTimer(uint8_t x)
{
    _soundTimer = _registers->get(x);
    return 45;
}

// 0x3XNN
int CPU::opSkipIfVxEqualsNn(uint8_t x, uint8_t nn)
{
    auto clockCycles = 55;
    if(_registers->get(x) == nn) {
        _pc += 2;
    } else {
        clockCycles += 9;
    }
    return clockCycles;
}

// 0x4XNN
int CPU::opSkipIfVxNotEqualsNn(uint8_t x, uint8_t nn)
{
    auto clockCycles = 55;
    if(_registers->get(x) != nn) {
        _pc += 2;
    } else {
        clockCycles += 9;
    }
    return clockCycles;
}

// 0x9XY0
int CPU::opSkipIfVxNotEqualsVy(uint8_t x, uint8_t y)
{
    auto clockCycles = 73;
    if(_registers->get(x) != _registers->get(y)) {
        _pc += 2;
    } else {
        clockCycles += 9;
    }
    return clockCycles;
}

// 0x5XY0
int CPU::opSkipIfVxEqualsVy(uint8_t x, uint8_t y)
{
    auto clockCycles = 55;
    if(_registers->get(x) == _registers->get(y)) {
        _pc += 2;
    } else {
        clockCycles += 9;
    }
    return clockCycles;
}

// 0xEX9E
int CPU::opSkipIfKeyPressed(uint8_t x, shared_ptr<Keyboard> keyboard)
{
    if(keyboard->isKeyPressed(_registers->get(x))) {
        _pc += 2;
    }
    return 73;
}

// 0xEXA1
int CPU::opSkipIfNotKeyPressed(uint8_t x, shared_ptr<Keyboard> keyboard)
{
    if(!keyboard->isKeyPressed(_registers->get(x))) {
        _pc += 2;
    }
    return 73;
}

// 0xCXNN
int CPU::opRandom(uint8_t x, uint8_t nn)
{
    _registers->set(x, randByte(randGen) & nn);
    return 73;
}

// 0xBNNN
int CPU::opJumpWithOffset(uint16_t nnn)
{
    _pc = nnn + _registers->get(0);
    return 105;
}

// 0x8XY0
int CPU::opSetVxToValueOfVy(uint8_t x, uint8_t y)
{
    _registers->set(x, _registers->get(y));
    return 200;
}

// 0x8XY6
int CPU::opShiftRight(uint8_t x)
{
    auto vx = _registers->get(x);
    auto flag = vx & 0x1;
    _registers->set(x, vx >> 1);
    _registers->set(0xF, flag);
    return 200;
}

// 0x8XYE
int CPU::opShiftLeft(uint8_t x)
{
    auto vx = _registers->get(x);
    auto flag = (vx & 0x80) >> 7;
    _registers->set(x, vx << 1);
    _registers->set(0xF, flag);
    return 200;
}

// 0x8XY5
int CPU::opSubtractVyFromVx(uint8_t x, uint8_t y)
{
    uint8_t vx = _registers->get(x);
    uint8_t vy = _registers->get(y);
    auto borrow = vx > vy ? 1 : 0;
    _registers->set(x, (vx - vy) & 0xFF);
    _registers->set(0xF, borrow);
    return 200;
}

// 0x8XY7
int CPU::opSubtractVxFromVy(uint8_t x, uint8_t y)
{
    auto vx = _registers->get(x);
    auto vy = _registers->get(y);
    auto flag = vy > vx ? 1 : 0;
    _registers->set(x, (vy - vx) & 0xFF);
    _registers->set(0xF, flag);
    return 200;
}

// 0x8XY4
int CPU::opAddWithCarry(uint8_t x, uint8_t y)
{
    auto sum = _registers->get(x) + _registers->get(y);
    _registers->set(0xF, sum > 255 ? 1 : 0);
    _registers->set(x, sum & 0xFF);
    return 200;
}

// 0x8XY1
int CPU::opBinaryOr(uint8_t x, uint8_t y)
{
    auto vx = _registers->get(x);
    auto vy = _registers->get(y);
    _registers->set(x, vx | vy);
    return 200;
}

// 0x8XY3
int CPU::opBinaryXor(uint8_t x, uint8_t y)
{
    auto vx = _registers->get(x);
    auto vy = _registers->get(y);
    _registers->set(x, vx ^ vy);
    return 200;
}

// 0x8XY2
int CPU::opBinaryAnd(uint8_t x, uint8_t y)
{
    auto vx = _registers->get(x);
    auto vy = _registers->get(y);
    _registers->set(x, vx & vy);
    return 200;
}

// 0x2NNN
int CPU::opJumpToSubroutine(uint16_t nnn)
{
    if(_sp < 16) {
        _stack[_sp] = _pc;
        _sp++;
        _pc = nnn;
    }
    return 105;
}
