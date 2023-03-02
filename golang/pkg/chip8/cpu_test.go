package chip8

import (
	"testing"
)

func TestOpcode0x1xxx(t *testing.T) {
	// arrange
	data := []byte{0x12, 0x02, 0x60, 0x01}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], 0x1)
}

func TestOpcode0x2xxx(t *testing.T) {
	// arrange
	data := []byte{0x22, 0x02, 0x00, 0xEE}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.pc, 514)
}

func TestShouldSkipNextInstruction(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x30, 0x01}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.pc, 518)
}

func TestShouldNotSkipNextInstruction(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x30, 0x02}
	cpu := initCPU(data)
	wanted := uint16(516)
	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.pc, wanted)
}

func TestShouldSkipNextInstructionIfNotEquals(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x40, 0x02}
	cpu := initCPU(data)
	var wanted uint16 = 518

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.pc, wanted)
}

func TestShouldNotSkipNextInstructionIfEquals(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x40, 0x01}
	cpu := initCPU(data)
	var wanted uint16 = 516

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.pc, wanted)
}

func TestShouldSkipNextInstructionIfVxEqualsVy(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x61, 0x01, 0x50, 0x10}
	cpu := initCPU(data)
	var wanted uint16 = 520

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.pc, wanted)
}

func TestShouldNotSkipNextInstructionIfVxNotEqualsVy(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x61, 0x02, 0x50, 0x10}
	cpu := initCPU(data)
	var wanted uint16 = 518

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.pc, wanted)
}

func TestOpcode0x6000(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01}
	cpu := initCPU(data)
	var wanted byte = 0x1

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], wanted)
}

func TestShouldAddValueToValueInRegistry(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x70, 0x01}
	cpu := initCPU(data)
	var wanted byte = 0x2

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], wanted)
}

func TestShouldAddValueToValueInRegistryAndResolveOverflow(t *testing.T) {
	// arrange
	data := []byte{0x60, 0xFF, 0x70, 0xFF}
	cpu := initCPU(data)
	var wanted byte = 254

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], wanted)
}

func TestOpcode0x8xy0(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x62, 0x02, 0x80, 0x20}
	cpu := initCPU(data)

	// act
	emulate(cpu, 4)

	// assert
	assertEq(t, cpu.registers[0], 0x1)
	emulate(cpu, 2)
	assertEq(t, cpu.registers[0], 0x2)
}

func TestOpcode0x8xy1(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x61, 0x06, 0x80, 0x11}
	cpu := initCPU(data)
	var wanted byte = 7

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], wanted)
}

func TestOpcode0x8xy2(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x0C, 0x61, 0x06, 0x80, 0x12}
	cpu := initCPU(data)
	var wanted byte = 4

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], wanted)
}

func TestOpcode0x8xy3(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x09, 0x61, 0x05, 0x80, 0x13}
	cpu := initCPU(data)
	var wanted byte = 12

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], wanted)
}

func TestOpcode0x8xy4NoCarrySetRegisterFToZero(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x61, 0x01, 0x80, 0x14}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], 2)
	assertEq(t, cpu.registers[0xF], 0)
}

func TestOpcode0x8xy5IfSumIsNonNegativeValue(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x03, 0x61, 0x02, 0x80, 0x15}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], 1)
	assertEq(t, cpu.registers[0xF], 1)
}

func TestOpcode0x8xy5IfSumIsNegativeValue(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x02, 0x61, 0x03, 0x80, 0x15}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], 255)
	assertEq(t, cpu.registers[0xF], 0)
}

func TestOpcode0x8xy6IfSumHasLeastSignificantBitOfOne(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x03, 0x80, 0x16}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], 1)
	assertEq(t, cpu.registers[0xF], 1)
}

func TestOpcode0x8xy6IfSumHasLeastSignificantBitOfZero(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x02, 0x80, 0x16}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], 1)
	assertEq(t, cpu.registers[0xF], 0)
}

func TestOpcode0x8xy7IfThereIsNoBorrowSetVfToOne(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x02, 0x61, 0x03, 0x80, 0x17}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], 1)
	assertEq(t, cpu.registers[0xF], 1)
}

func TestOpcode0x8xy7IfThereIsBorrowSetVfToZero(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x03, 0x61, 0x02, 0x80, 0x17}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], 255)
	assertEq(t, cpu.registers[0xF], 0)
}

func TestOpcode0x8xyeMsbShouldBeOne(t *testing.T) {
	// arrange
	data := []byte{0x60, 0xFF, 0x80, 0x0E}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], 254)
	assertEq(t, cpu.registers[0xF], 1)
}

func TestOpcode0x8xyeMbsShouldBeZero(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x80, 0x0E}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.registers[0], 2)
	assertEq(t, cpu.registers[0xF], 0)
}

func TestOpcode0x9xy0SkipsNextInstruction(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x61, 0x02, 0x90, 0x10}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.pc, 520)
}

func TestOpcode0x9xy0ShouldNotSkipNextInstruction(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0x61, 0x01, 0x90, 0x10}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.pc, 518)
}

func TestOpcode0xANNNShouldSetIToNNN(t *testing.T) {
	// arrange
	data := []byte{0xAF, 0xFF}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.index, 4095)
}

func TestOpcode0xBNNNShouldJumpToNNNPlusVzero(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0xB2, 0x05}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.pc, 518)
}

func TestOpcode0xFX15SetDelayTimerToVx(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0xF0, 0x15}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.delayTimer, 1)
}

func TestOpcode0xFX18SetSoundTimerToVx(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0xF0, 0x18}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.soundTimer, 1)
}

func TestOpcode0xFX1EAddVxToI(t *testing.T) {
	// arrange
	data := []byte{0x60, 0x01, 0xF0, 0x1E}
	cpu := initCPU(data)

	// act
	emulate(cpu, len(data))

	// assert
	assertEq(t, cpu.index, 1)
}

func TestOpcode0xFX07SetVxToTheDelayTimer(t *testing.T) {
	// arrange
	input := FakeInput {}
	output := FakeOutput {}
	// audio := FakeAudioPlayer {}
	data := []byte{0xF0, 0x07}
	cpu := initCPU(data)
	cpu.delayTimer = 2

	// act
	cpu.tick(&input, &output, nil);

	// assert
	assertEq(t, cpu.registers[0], 1)
}




func assertEq[T comparable](t *testing.T, got T, want T) {
	t.Helper()

	if want != got {
		t.Errorf("Got %v, wanted %v", got, want)
	}
}

func initCPU(data []byte) *CPU {
	memory := Memory{}
	memory.LoadData(512, data)
	cpu := CPU{
		memory:     memory,
		registers:  [16]byte{},
		index:      0,
		program:    []byte{},
		pc:         0x200,
		stack:      [16]uint16{},
		sp:         0,
		delayTimer: 0,
		soundTimer: 0,
	}
	return &cpu
}

type FakeInput struct{}

func (f *FakeInput) IsPressed(key byte) bool       { return false }
func (f *FakeInput) HasBeenReleased(key byte) bool { return false }

type FakeOutput struct{}

func (f *FakeOutput) clear()                 {}
func (f *FakeOutput) draw()                  {}
func (f *FakeOutput) flipPixel(addr uint16)  {}
func (f *FakeOutput) setDrawFlag(value bool) {}

func emulate(cpu *CPU, iterations int) {
	input := FakeInput{}
	output := FakeOutput{}

	for i := 0; i < iterations/2; i++ {
		cpu.emulateCycle(&input, &output)
	}
}
