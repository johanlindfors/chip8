// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8.Tests;

/// <summary>
/// Emulator tests.
/// </summary>
public class EmulatorTest
{
    private CPU cpu;
    private Memory memory;
    private Registers registers;
    private RandomNumberGeneratorMock random;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmulatorTest"/> class.
    /// </summary>
    public EmulatorTest()
    {
        this.memory = new Memory();
        this.registers = new Registers();
        this.random = new RandomNumberGeneratorMock();
        var screen = new ScreenMock();
        var keyboard = new KeyboardMock();
        var audio = new AudioMock();
        this.cpu = new Chip8.CPU(
            this.memory,
            this.registers,
            this.random,
            keyboard,
            screen,
            audio);
    }

    [Fact(DisplayName = "code 1XXX jumps to address NNN")]
    private void ShouldTestOpcode0x1XXX()
    {
        byte[] data = { 0x12, 0x02, 0x60, 0x01 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal("1", this.registers[0].ToString("X"));
    }

    [Fact(DisplayName = "code 2XXX jump to subroutine")]
    private void ShouldTestOpcode0x2XXX()
    {
        byte[] data = { 0x22, 0x02, 0x00, 0xEE };
        this.memory.LoadData(data);

        this.Emulate(data);

        Assert.Equal(514, this.cpu.PC);
    }

    [Fact(DisplayName = "code 3XNN Should skip the next instruction if VX == NN")]
    private void ShouldSkipNextInstruction()
    {
        byte[] data = { 0x60, 0x01, 0x30, 0x01 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(518, this.cpu.PC);
    }

    [Fact(DisplayName = "code 3XNN Should NOT skip the next instruction if VX != NN")]
    private void ShouldNotSkipNextInstruction()
    {
        byte[] data = { 0x60, 0x01, 0x30, 0x02 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(516, this.cpu.PC);
    }

    [Fact(DisplayName = "code 4XNN Should skip the next instruction if VX != NN")]
    private void ShouldSkipNextInstructionIfNotEquals()
    {
        byte[] data = { 0x60, 0x01, 0x40, 0x02 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(518, this.cpu.PC);
    }

    [Fact(DisplayName = "code 4XNN Should NOT skip the next instruction if VX == NN")]
    private void ShouldNotSkipNextInstructionIfEquals()
    {
        byte[] data = { 0x60, 0x01, 0x40, 0x01 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(516, this.cpu.PC);
    }

    [Fact(DisplayName = "code 5XY0 Skips the next instruction if VX == VY")]
    private void ShouldSkipNextInstructionIfVxEqualsVy()
    {
        byte[] data = { 0x60, 0x01, 0x61, 0x01, 0x50, 0x10 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(520, this.cpu.PC);
    }

    [Fact(DisplayName = "code 5XY0 Does not skip the next instruction if VX != VY")]
    private void ShouldNotSkipNextInstructionIfVxNotEqualsVy()
    {
        byte[] data = { 0x60, 0x01, 0x61, 0x02, 0x50, 0x10 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(518, this.cpu.PC);
    }

    [Fact(DisplayName = "code 6XXX set registers to value")]
    private void ShouldTestOpcode0x6000()
    {
        byte[] data = { 0x60, 0x01 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal("1", this.registers[0].ToString("X"));
    }

    [Fact(DisplayName = "code 7XNN Adds NN to VX")]
    private void ShouldAddValueToValueInRegistry()
    {
        byte[] data = { 0x60, 0x01, 0x70, 0x01 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(2, this.registers[0]);
    }

    [Fact(DisplayName = "code 7XNN Adds NN to VX and resolves overflow (Carry flag is not changed)")]
    private void ShouldAddValueToValueInRegistryAndResolveOverflow()
    {
        byte[] data = { 0x60, 0xFF, 0x70, 0xFF };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(254, this.registers[0]);
    }

    [Fact(DisplayName = "code 8XY0 Sets VX to the value of VY.")]
    private void ShouldTestOpcode0x8XY0()
    {
        byte[] data = { 0x60, 0x01, 0x62, 0x02, 0x80, 0x20 };

        this.memory.LoadData(data);

        this.cpu.EmulateCycle();
        this.cpu.EmulateCycle();
        Assert.Equal("1", this.registers[0].ToString("X"));

        this.cpu.EmulateCycle();
        Assert.Equal("2", this.registers[0].ToString("X"));
    }

    [Fact(DisplayName = "code 8XY1 Sets VX to VX or VY. (Bitwise OR operation)")]
    private void ShouldTestOpcode0x8XY1()
    {
        byte[] data = { 0x60, 0x01, 0x61, 0x06, 0x80, 0x11 };

        this.memory.LoadData(data);

        this.Emulate(data);

        Assert.Equal(7, this.registers[0]);
    }

    [Fact(DisplayName = "code 8XY2 Sets VX to VX and VY. (Bitwise AND operation)")]
    private void ShouldTestOpcode0x8XY2()
    {
        byte[] data = { 0x60, 0x0C, 0x61, 0x06, 0x80, 0x12 };

        this.memory.LoadData(data);

        this.Emulate(data);

        Assert.Equal(4, this.registers[0]);
    }

    [Fact(DisplayName = "code 8XY3 Sets VX to VX xor VY.")]
    private void ShouldTestOpcode0x8XY3()
    {
        byte[] data = { 0x60, 0x09, 0x61, 0x05, 0x80, 0x13 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(12, this.registers[0]);
    }

    [Fact(DisplayName = "code 8XY4 Adds VY to VX. VF is set to 0 when there's no carry.")]
    private void ShouldTestOpcode0x8XY4NoCarrySetRegisterFtoZero()
    {
        byte[] data = { 0x60, 0x01, 0x61, 0x01, 0x80, 0x14 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(2, this.registers[0]);
        Assert.Equal(0, this.registers[0xF]);
    }

    [Fact(DisplayName = "code 8XY4 Adds VY to VX. VF is set to 1 when there's a carry")]
    private void ShouldTestOpcode0x8XY4WithCarrySetRegisterFtoOne()
    {
        byte[] data = { 0x60, 0xF1, 0x61, 0xF1, 0x80, 0x14 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(226, this.registers[0]);
        Assert.Equal(1, this.registers[0xF]);
    }

    [Fact(DisplayName = "code 8XY5 VY is subtracted from VX. VF is set to 1 when there's no borrow")]
    private void ShouldTestOpcode0x8XY5IfSumIsNonNegativeValue()
    {
        byte[] data = { 0x60, 0x03, 0x61, 0x02, 0x80, 0x15 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(1, this.registers[0]);
        Assert.Equal(1, this.registers[0xF]);
    }

    [Fact(DisplayName = "code 8XY5 VY is subtracted from VX. VF is set to 1 when there's no borrow")]
    private void ShouldTestOpcode0x8XY5IfSumIsNegativeValue()
    {
        byte[] data = { 0x60, 0x02, 0x61, 0x03, 0x80, 0x15 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(255, this.registers[0]);
        Assert.Equal(0, this.registers[0xF]);
    }

    [Fact(DisplayName = "code 8XY6 Stores the least significant bit of VX in VF and then shifts VX to the right by 1.")]
    private void ShouldTestOpcode0x8XY6IfSumHasLeastSignificantBitOfOne()
    {
        byte[] data = { 0x60, 0x03, 0x80, 0x16 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(1, this.registers[0]);
        Assert.Equal(1, this.registers[0xF]);
    }

    [Fact(DisplayName = "code 8XY6 Stores the least significant bit of VX in VF and then shifts VX to the right by 1.")]
    private void ShouldTestOpcode0x8XY6IfSumHasLeastSignificantBitOfZero()
    {
        byte[] data = { 0x60, 0x02, 0x80, 0x16 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(1, this.registers[0]);
        Assert.Equal(0, this.registers[0xF]);
    }

    [Fact(DisplayName = "code 8XY7 Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.")]
    private void ShouldTestOpcode0x8XY7IfThereIsNoBorrowSetVFToOne()
    {
        byte[] data = { 0x60, 0x02, 0x61, 0x03, 0x80, 0x17 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(1, this.registers[0]);
        Assert.Equal(1, this.registers[0xF]);
    }

    [Fact(DisplayName = "code 8XY7 Sets VX to VY minus VX. VF is set to 0 when there's a borrow, and 1 when there isn't.")]
    private void ShouldTestOpcode0x8XY7IfThereIsBorrowSetVFToZero()
    {
        byte[] data = { 0x60, 0x03, 0x61, 0x02, 0x80, 0x17 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(255, this.registers[0]);
        Assert.Equal(0, this.registers[0xF]);
    }

    [Fact(DisplayName = "code 8XYE Stores the most significant bit of VX in VF and then shifts VX to the left by 1")]
    private void ShouldTestOpcode0x8XYEMSBShouldBeOne()
    {
        byte[] data = { 0x60, 0xFF, 0x80, 0x0E };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(1, this.registers[0xF]);
        Assert.Equal(254, this.registers[0]);
    }

    [Fact(DisplayName = "code 8XYE Stores the most significant bit of VX in VF and then shifts VX to the left by 1")]
    private void ShouldTestOpcode0x8XYEMSBShouldBeZero()
    {
        byte[] data = { 0x60, 0x01, 0x80, 0x0E };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(0, this.registers[0xF]);
        Assert.Equal(2, this.registers[0]);
    }

    [Fact(DisplayName = "code 9XY0 Skips the next instruction when VX doesn't equal VY")]
    private void ShouldTestOpcode0x9XY0SkipsNextInstruction()
    {
        byte[] data = { 0x60, 0x01, 0x61, 0x02, 0x90, 0x10 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(520, this.cpu.PC);
    }

    [Fact(DisplayName = "code 9XY0 Does not skip the next instruction when VX equal VY")]
    private void ShouldTestOpcode0x9XY0ShouldNotSkipsNextInstruction()
    {
        byte[] data = { 0x60, 0x01, 0x61, 0x01, 0x90, 0x10 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(518, this.cpu.PC);
    }

    [Fact(DisplayName = "code ANNN Sets I to the address NNN")]
    private void ShouldTestOpcode0xANNNShouldSetItoNNN()
    {
        byte[] data = { 0xAF, 0xFF };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(4095, this.cpu.I);
    }

    [Fact(DisplayName = "code BNNN Jumps to the address NNN plus V0.")]
    private void ShouldTestOpcode0xBNNNShouldJumpToNNNPlusVZero()
    {
        byte[] data = { 0x60, 0x01, 0xB2, 0x05 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(518, this.cpu.PC);
    }

    [Fact(DisplayName = "code CXNN Sets VX to the result of a bitwise and operation on a random number (Typically: 0 to 255) and NN.")]
    private void ShouldTestOpcode0xCXNNShouldSetVxToRandomNumberANDNN()
    {
        byte[] data = { 0xC0, 0x07 };

        this.random.PopulateRandomQueue(new byte[] { 85 });
        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(5, this.registers[0]);
    }

    [Fact(DisplayName = "code FX15 Sets the delay timer to VX")]
    private void ShouldTestOpcodeFX15SetDelayTimerToVX()
    {
        byte[] data = { 0x60, 0x01, 0xF0, 0x15 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(1, this.cpu.DelayTimer);
    }

    [Fact(DisplayName = "code FX18 Sets the sound timer to VX")]
    private void ShouldTestOpcodeFX18SetSoundTimerToVX()
    {
        byte[] data = { 0x60, 0x01, 0xF0, 0x18 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(1, this.cpu.SoundTimer);
    }

    [Fact(DisplayName = "code FX1E adds Vx to I")]
    private void ShouldTestOpcodeFX1EAddVxToI()
    {
        byte[] data = { 0x60, 0x01, 0xF0, 0x1E };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(1, this.cpu.I);
    }

    [Fact(DisplayName = "code FX07 set Vx to the delayTimer")]
    private void ShouldTestOpcodeFX07SetVxToTheDelayTimer()
    {
        this.cpu.DelayTimer = 1;
        byte[] data = { 0xF0, 0x07 };

        this.memory.LoadData(data);
        this.Emulate(data);

        Assert.Equal(1, this.registers[0]);
    }

    private void Emulate(byte[] data)
    {
        for (int i = 0; i < (data.Length / 2); i++)
        {
            this.cpu.EmulateCycle();
        }
    }
}
