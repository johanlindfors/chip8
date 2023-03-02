package chip8

import (
	"fmt"
	"math/rand"
)

type CPU struct {
	memory     Memory
	registers  [16]byte
	index      uint16
	program    []byte
	pc         uint16
	stack      [16]uint16
	sp         byte
	delayTimer byte
	soundTimer byte
}

func (c *CPU) tick(keyboard Input, display Output, audio *Audio) {
	if c.delayTimer > 0 {
		c.delayTimer -= 1
	}
	if c.soundTimer > 0 {
		c.soundTimer -= 1
		if !audio.isPlaying() {
			audio.start()
		}
	} else {
		if audio != nil && audio.isPlaying() {
			audio.stop()
		}
	}

	microseconds := 16666
	for microseconds > 0 {

		delta := int(c.emulateCycle(keyboard, display))
		if delta == 0 {
			break
		}
		microseconds -= delta
	}
}

func (c *CPU) emulateCycle(keyboard Input, display Output) uint16 {
	opcode := c.getOpcode()
	return c.exec(opcode, keyboard, display)
}

func (c *CPU) getOpcode() uint16 {
	opcode := uint16(c.memory.get(c.pc))<<8 | uint16(c.memory.get(c.pc+1))
	c.pc += 2
	return opcode
}

func (c *CPU) exec(opcode uint16, keyboard Input, display Output) uint16 {
	x := opcode & 0x0F00 >> 8
	y := opcode & 0x00F0 >> 4
	n := byte(opcode & 0x000F)
	nn := byte(opcode & 0x00FF)
	nnn := opcode & 0x0FFF

	// fmt.Printf("Opcode %v\n", opcode)
	switch opcode & 0xF000 {
	case 0x0000:
		switch opcode & 0x00FF {
		case 0x00E0:
			return c.opClearScreen(display)
		case 0x000E:
			return c.opReturn()
		case 0x00EE:
			return c.opReturnFromSubroutine()
		}
	case 0x1000:
		return c.opJump(nnn)
	case 0x2000:
		return c.opJumpToSubroutine(nnn)
	case 0x3000:
		return c.opSkipIfVxEqualsNn(x, nn)
	case 0x4000:
		return c.opSkipIfVxNotEqualsNn(x, nn)
	case 0x5000:
		return c.opSkipIfVxEqualsVy(x, y)
	case 0x6000:
		return c.opSetRegister(x, nn)
	case 0x7000:
		return c.opAddValueToRegister(x, nn)
	case 0x8000:
		switch opcode & 0x000F {
		case 0x0000:
			return c.opSetVxToValueOfVy(x, y)
		case 0x0001:
			return c.opBinaryOr(x, y)
		case 0x0002:
			return c.opBinaryAnd(x, y)
		case 0x0003:
			return c.opBinaryXor(x, y)
		case 0x0004:
			return c.opAddWithCarry(x, y)
		case 0x0005:
			return c.opSubtractYFromX(x, y)
		case 0x0006:
			return c.opShiftRight(x)
		case 0x0007:
			return c.opSubtractXFromY(x, y)
		case 0x000E:
			return c.opShiftLeft(x)
		}
	case 0x9000:
		return c.opSkipIfVxNotEqualsVy(x, y)
	case 0xA000:
		return c.opSetIndexRegister(nnn)
	case 0xB000:
		return c.opJumpWithOffset(nnn)
	case 0xC000:
		return c.opRandom(x, nn)
	case 0xD000:
		return c.opDisplay(x, y, n, display)
	case 0xE000:
		switch opcode & 0x00FF {
		case 0x009E:
			return c.opSkipIfKeyPressed(x, keyboard)
		case 0x00A1:
			return c.opSkipIfNotKeyPressed(x, keyboard)
		}
	case 0xF000:
		switch opcode & 0x00FF {
		case 0x0007:
			return c.opGetDelayTimer(x)
		case 0x000A:
			return c.opGetKey(x, keyboard)
		case 0x0015:
			return c.opSetDelayTimer(x)
		case 0x0018:
			return c.opSetSoundTimer(x)
		case 0x001E:
			return c.opAddToIndex(x)
		case 0x0029:
			return c.opFontCharacter(x)
		case 0x0033:
			return c.opBinaryCodedDecimalConversion(x)
		case 0x0055:
			return c.opStoreRegistersToMemory(x)
		case 0x0065:
			return c.opLoadRegistersFromMemory(x)
		}
	}
	return 0
}

// 0xFX0A
func (c *CPU) opGetKey(x uint16, keyboard Input) uint16 {
	for i := byte(0); i < 16; i++ {
		if keyboard.HasBeenReleased(i) {
			c.registers[x] = i
			return 1
		}
	}
	c.pc -= 2
	return 1
}

// 0xFX29
func (c *CPU) opFontCharacter(x uint16) uint16 {
	c.index = SPRITE_CHARS_ADDR + uint16(c.registers[x])
	return 91
}

// 0xFX55
func (c *CPU) opStoreRegistersToMemory(x uint16) uint16 {
	for i := uint16(0); i <= x; i++ {
		c.memory.set(c.index+i, c.registers[i])
	}
	return 605 + x*64
}

// 0xFX65
func (c *CPU) opLoadRegistersFromMemory(x uint16) uint16 {
	for i := uint16(0); i <= x; i++ {
		c.registers[i] = c.memory.get(c.index + i)
	}
	return 605 + x*64
}

// 0xFX33
func (c *CPU) opBinaryCodedDecimalConversion(x uint16) uint16 {
	c.memory.set(c.index, c.registers[x]/100)
	c.memory.set(c.index+1, (c.registers[x]/10)%10)
	c.memory.set(c.index+2, c.registers[x]%10)
	return 927
}

// 0xFX1E
func (c *CPU) opAddToIndex(x uint16) uint16 {
	c.index += uint16(c.registers[x])
	return 86
}

// 0xFX07
func (c *CPU) opGetDelayTimer(x uint16) uint16 {
	c.registers[x] = c.delayTimer
	return 45
}

// 0xFX15
func (c *CPU) opSetDelayTimer(x uint16) uint16 {
	c.delayTimer = c.registers[x]
	return 45
}

// 0xFX18
func (c *CPU) opSetSoundTimer(x uint16) uint16 {
	c.soundTimer = c.registers[x]
	return 45
}

// 0x3XNN
func (c *CPU) opSkipIfVxEqualsNn(x uint16, value byte) uint16 {
	var clockCycles uint16 = 55
	if c.registers[x] == value {
		c.pc += 2
	} else {
		clockCycles += 9
	}
	return clockCycles
}

// 0x4XNN
func (c *CPU) opSkipIfVxNotEqualsNn(x uint16, value byte) uint16 {
	var clockCycles uint16 = 55
	if c.registers[x] != value {
		c.pc += 2
	} else {
		clockCycles += 9
	}
	return clockCycles
}

// 0x9XY0
func (c *CPU) opSkipIfVxNotEqualsVy(x uint16, y uint16) uint16 {
	var clockCycles uint16 = 73
	if c.registers[x] != c.registers[y] {
		c.pc += 2
	} else {
		clockCycles += 9
	}
	return clockCycles
}

// 0x5XY0
func (c *CPU) opSkipIfVxEqualsVy(x uint16, y uint16) uint16 {
	var clockCycles uint16 = 55
	if c.registers[x] == c.registers[y] {
		c.pc += 2
	} else {
		clockCycles += 9
	}
	return clockCycles
}

// 0xEX9E
func (c *CPU) opSkipIfKeyPressed(x uint16, keyboard Input) uint16 {
	if keyboard.IsPressed(c.registers[x]) {
		c.pc += 2
	}
	return 73
}

// 0xEXA1
func (c *CPU) opSkipIfNotKeyPressed(x uint16, keyboard Input) uint16 {
	if !keyboard.IsPressed(c.registers[x]) {
		c.pc += 2
	}
	return 73
}

// 0xCXNN
func (c *CPU) opRandom(x uint16, nn byte) uint16 {
	random := byte(rand.Intn(256))
	c.registers[x] = random & nn
	return 164
}

// 0xBXNN
func (c *CPU) opJumpWithOffset(addr uint16) uint16 {
	c.pc = addr + uint16(c.registers[0])
	return 105
}

// 0x8XY0
func (c *CPU) opSetVxToValueOfVy(x uint16, y uint16) uint16 {
	c.registers[x] = c.registers[y]
	return 200
}

// 0x8XY6
func (c *CPU) opShiftRight(x uint16) uint16 {
	flag := c.registers[x] & 0x1
	c.registers[x] >>= 1
	c.registers[0xF] = flag
	return 200
}

// 0x8XYE
func (c *CPU) opShiftLeft(x uint16) uint16 {
	flag := (c.registers[x] & 0x80) >> 7
	c.registers[x] <<= 1
	c.registers[0xF] = flag
	return 200
}

// 0x8XY5
func (c *CPU) opSubtractYFromX(x uint16, y uint16) uint16 {
	vx := c.registers[x]
	vy := c.registers[y]
	borrow := 0
	if vx > vy {
		borrow = 1
	}
	c.registers[x] = byte(vx - vy)
	c.registers[0xF] = byte(borrow)
	return 200
}

// 0x8XY7
func (c *CPU) opSubtractXFromY(x uint16, y uint16) uint16 {
	vx := c.registers[x]
	vy := c.registers[y]
	flag := 0
	if vy > vx {
		flag = 1
	}
	c.registers[x] = byte(vy - vx)
	c.registers[0xF] = byte(flag)
	return 200
}

// 0x8XY4
func (c *CPU) opAddWithCarry(x uint16, y uint16) uint16 {
	sum := uint16(c.registers[x]) + uint16(c.registers[y])
	if sum > 255 {
		c.registers[0xF] = 1
	} else {
		c.registers[0xF] = 0
	}
	c.registers[x] = byte(sum)
	return 200
}

// 0x8XY1
func (c *CPU) opBinaryOr(x uint16, y uint16) uint16 {
	c.registers[x] |= c.registers[y]
	return 200
}

// 0x8XY3
func (c *CPU) opBinaryXor(x uint16, y uint16) uint16 {
	c.registers[x] ^= c.registers[y]
	return 200
}

// 0x8XY2
func (c *CPU) opBinaryAnd(x uint16, y uint16) uint16 {
	c.registers[x] &= c.registers[y]
	return 200
}

// 0x00E0
func (c *CPU) opClearScreen(display Output) uint16 {
	display.clear()
	return 109
}

// 0x000E
func (c *CPU) opReturn() uint16 {
	c.pc = c.index
	return 1
}

// 0x1NNN
func (c *CPU) opJump(addr uint16) uint16 {
	c.pc = addr
	return 105
}

// 0x6XNN
func (c *CPU) opSetRegister(x uint16, value byte) uint16 {
	//fmt.Printf("Setting register %v to value %v\n", x, value)
	c.registers[x] = value
	return 27
}

// 0x7XNN
func (c *CPU) opAddValueToRegister(x uint16, value byte) uint16 {
	vx := c.registers[x]
	vy := value
	for vy != 0 {
		carry := vx & vy
		vx ^= vy
		vy = carry << 1
	}
	c.registers[x] = vx
	return 45
}

// 0xANNN
func (c *CPU) opSetIndexRegister(value uint16) uint16 {
	fmt.Printf("Setting register I to value %v\n", value)
	c.index = value
	return 55
}

// 0xDXYN
func (c *CPU) opDisplay(x uint16, y uint16, n byte, display Output) uint16 {
	index := c.index
	vx := uint16(c.registers[x])
	vy := uint16(c.registers[y])
	fmt.Printf("Rendering a %v pixel tall sprite at X: %v, Y: %v from the address: %v\n", n, vx, vy, index)

	for row := vy; row < vy+uint16(n); row++ {
		if row >= 32 {
			break
		}
		startX := vx + row*64
		endX := startX + 8
		var bitMask byte = 0x80

		for col := startX; col < endX; col++ {
			if (col % 64) >= (startX % 64) {
				position := (row * 64) + (col % 64)
				if c.memory.get(index)&bitMask == bitMask {
					display.flipPixel(position)
				}
			}
			bitMask /= 2
		}
		index++
	}

	display.setDrawFlag(true)
	return 22734
}

// 0x2NNN
func (c *CPU) opJumpToSubroutine(addr uint16) uint16 {
	if c.sp < byte(len(c.stack)) {
		c.stack[c.sp] = c.pc
		c.sp += 1
		c.pc = addr
	}
	return 105
}

// 0x00EE
func (c *CPU) opReturnFromSubroutine() uint16 {
	if c.sp > 0 {
		c.sp -= 1
		c.pc = c.stack[c.sp]
	}
	return 105
}
