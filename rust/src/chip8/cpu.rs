use super::memory::Memory;
use crate::chip8::{
    Input, Output, Speaker, COLS, FRAME_TICKS, PROGRAM_START_ADDRESS, REGISTER_COUNT, ROWS,
    SPRITE_CHARS_ADDR, STACK_DEPTH,
};
use rand;

pub struct CPU {
    memory: Memory,            // RAM
    pc: u16,                   // program Counter
    pub dt: u8,                // delay timer
    pub st: u8,                // sound timer
    i: u16,                    // index register
    v: [u8; REGISTER_COUNT],   // registers
    sp: u8,                    // stack pointer
    stack: [u16; STACK_DEPTH], // stack
    microseconds: i32,         // ticks per frame
}

impl CPU {
    pub fn new(memory: Memory) -> CPU {
        CPU {
            memory,
            pc: PROGRAM_START_ADDRESS,
            dt: 0,
            st: 0,
            i: 0,
            v: [0; REGISTER_COUNT],
            sp: 0,
            stack: [0; STACK_DEPTH],
            microseconds: 0,
        }
    }

    pub fn tick<I: Input, O: Output, A: Speaker>(
        &mut self,
        keyboard: &I,
        display: &mut O,
        audio: &mut A,
    ) {
        if self.dt > 0 {
            self.dt -= 1;
        }
        if self.st > 0 {
            self.st -= 1;
            if !audio.is_playing() {
                audio.start();
            }
        } else if audio.is_playing() {
            audio.stop();
        }
        self.microseconds += FRAME_TICKS;
        loop {
            let delta = self.emulate_cycle(keyboard, display) as i32;
            self.microseconds -= delta;
            if self.microseconds <= 0 || delta == 0 {
                break;
            }
        }
    }

    fn emulate_cycle<I: Input, O: Output>(&mut self, keyboard: &I, display: &mut O) -> u32 {
        let opcode = self.get_opcode();
        self.exec(opcode, keyboard, display)
    }

    fn get_opcode(&mut self) -> u16 {
        let highbyte = self.memory.get(self.pc as usize);
        let lowbyte = self.memory.get((self.pc + 1) as usize);
        self.pc += 2;
        ((highbyte as u16) << 8) | (lowbyte as u16)
    }

    pub fn exec<I: Input, O: Output>(&mut self, opcode: u16, keyboard: &I, display: &mut O) -> u32 {
        let x: usize = ((opcode & 0x0F00) >> 8) as usize;
        let y: usize = ((opcode & 0x00F0) >> 4) as usize;

        let n: u8 = (opcode & 0x000F) as u8;
        let nn: u8 = (opcode & 0x00FF) as u8;
        let nnn: u16 = opcode & 0x0FFF;

        println!("Opcode {}", opcode);

        match opcode & 0xF000 {
            0x0000 => match opcode & 0x00FF {
                0x00E0 => self.op_clear_screen(display),
                0x000E => self.op_return(),
                0x00EE => self.op_return_from_subroutine(),
                _ => 0,
            },
            0x1000 => self.op_jump(nnn),
            0x2000 => self.op_jump_to_subroutine(nnn),
            0x3000 => self.op_skip_if_vx_equals_nn(x, nn),
            0x4000 => self.op_skip_if_vx_not_equals_nn(x, nn),
            0x5000 => self.op_skip_if_vx_equals_vy(x, y),
            0x6000 => self.op_set_register(x, nn),
            0x7000 => self.op_add_value_to_register(x, nn),
            0x8000 => match opcode & 0x000F {
                0x0000 => self.op_set_vx_to_value_of_vy(x, y),
                0x0001 => self.op_binary_or(x, y),
                0x0002 => self.op_binary_and(x, y),
                0x0003 => self.op_binary_xor(x, y),
                0x0004 => self.op_add_with_carry(x, y),
                0x0005 => self.op_subtract_y_from_x(x, y),
                0x0006 => self.op_shift_right(x),
                0x0007 => self.op_subtract_x_from_y(x, y),
                0x000E => self.op_shift_left(x),
                _ => 0,
            },
            0x9000 => self.op_skip_if_vx_not_equals_vy(x, y),
            0xA000 => self.op_set_index_register(nnn),
            0xB000 => self.op_jump_with_offset(nnn),
            0xC000 => self.op_random(x, nn),
            0xD000 => self.op_display(x, y, n, display),
            0xE000 => match opcode & 0x00FF {
                0x009E => self.op_skip_if_key_pressed(x, keyboard),
                0x00A1 => self.op_skip_if_not_key_pressed(x, keyboard),
                _ => 0,
            },
            0xF000 => match opcode & 0x00FF {
                0x0007 => self.op_get_delay_timer(x),
                0x000A => self.op_get_key(x, keyboard),
                0x0015 => self.op_set_delay_timer(x),
                0x0018 => self.op_set_sound_timer(x),
                0x001E => self.op_add_to_index(x),
                0x0029 => self.op_font_character(x),
                0x0033 => self.op_binary_coded_decimal_conversion(x),
                0x0055 => self.op_store_register_to_memory(x),
                0x0065 => self.op_load_register_from_memory(x),
                _ => 0,
            },
            _ => 0,
        }
    }

    // 0xFX0A
    fn op_get_key<I: Input>(&mut self, x: usize, keyboard: &I) -> u32 {
        for i in 0..16 {
            if keyboard.has_been_released(i) {
                self.v[x] = i as u8;
                return 1;
            }
        }
        self.pc -= 2;
        1
    }

    // 0xFX29
    fn op_font_character(&mut self, x: usize) -> u32 {
        self.i = SPRITE_CHARS_ADDR + u16::from(self.v[x]);
        91
    }

    // 0xFX55
    fn op_store_register_to_memory(&mut self, x: usize) -> u32 {
        for i in 0..=x {
            self.memory.set(self.i as usize + i, self.v[i]);
        }
        (605 + x * 64) as u32
    }

    // 0xFX65
    fn op_load_register_from_memory(&mut self, x: usize) -> u32 {
        for i in 0..=x as u16 {
            self.v[i as usize] = self.memory.get(usize::from(self.i + i));
        }
        (605 + x * 64) as u32
    }

    // 0xFX33
    fn op_binary_coded_decimal_conversion(&mut self, x: usize) -> u32 {
        self.memory.set(self.i as usize, self.v[x] / 100);
        self.memory
            .set((self.i + 1) as usize, (self.v[x] / 10) % 10);
        self.memory.set((self.i + 2) as usize, self.v[x] % 10);
        927 // +- 545
    }

    // 0xFX1E
    fn op_add_to_index(&mut self, x: usize) -> u32 {
        self.i += self.v[x] as u16;
        86 // +-14
    }

    // 0xFX07
    fn op_get_delay_timer(&mut self, x: usize) -> u32 {
        self.v[x] = self.dt;
        45
    }

    // 0xFX15
    fn op_set_delay_timer(&mut self, x: usize) -> u32 {
        self.dt = self.v[x];
        45
    }

    // 0xFX18
    fn op_set_sound_timer(&mut self, x: usize) -> u32 {
        self.st = self.v[x];
        45
    }

    // 0x3XNN
    fn op_skip_if_vx_equals_nn(&mut self, x: usize, nn: u8) -> u32 {
        let mut clock_cycles = 55;
        if self.v[x] == nn {
            self.pc += 2;
        } else {
            clock_cycles += 9;
        }
        clock_cycles // +-9
    }

    // 0x4XNN
    fn op_skip_if_vx_not_equals_nn(&mut self, x: usize, nn: u8) -> u32 {
        let mut clock_cycles = 55;
        if self.v[x] != nn {
            self.pc += 2;
        } else {
            clock_cycles += 9;
        }
        clock_cycles // +-9
    }

    // 0x9XY0
    fn op_skip_if_vx_not_equals_vy(&mut self, x: usize, y: usize) -> u32 {
        let mut clock_cycles = 73;
        if self.v[x] != self.v[y] {
            self.pc += 2;
        } else {
            clock_cycles += 9;
        }
        clock_cycles // +-9
    }

    // 0x5XY0
    fn op_skip_if_vx_equals_vy(&mut self, x: usize, y: usize) -> u32 {
        let mut clock_cycles = 55;
        if self.v[x] == self.v[y] {
            self.pc += 2;
        } else {
            clock_cycles += 9;
        }
        clock_cycles // +-9
    }

    // 0xEX9#
    fn op_skip_if_key_pressed<I: Input>(&mut self, x: usize, keyboard: &I) -> u32 {
        if keyboard.is_pressed(self.v[x] as usize) {
            self.pc += 2;
        }
        73
    }

    // 0xEXA1
    fn op_skip_if_not_key_pressed<I: Input>(&mut self, x: usize, keyboard: &I) -> u32 {
        if !keyboard.is_pressed(self.v[x] as usize) {
            self.pc += 2;
        }
        73
    }

    // 0xCXNN
    fn op_random(&mut self, x: usize, nn: u8) -> u32 {
        let random = rand::random::<u8>();
        self.v[x] = random & nn;
        164
    }

    // 0xBNNN
    fn op_jump_with_offset(&mut self, address: u16) -> u32 {
        self.pc = address + u16::from(self.v[0]);
        105 // +-5
    }

    // 0x8XY0
    fn op_set_vx_to_value_of_vy(&mut self, x: usize, y: usize) -> u32 {
        self.v[x] = self.v[y];
        200
    }

    // 0x8XY6
    fn op_shift_right(&mut self, x: usize) -> u32 {
        let flag = self.v[x] & 0x1;
        self.v[x] >>= 1;
        self.v[0xF] = flag;
        200
    }

    // 0x8XYE
    fn op_shift_left(&mut self, x: usize) -> u32 {
        let flag = (self.v[x] & 0x80) >> 7;
        self.v[x] <<= 1;
        self.v[0xF] = flag;
        200
    }

    // 0x8XY5
    fn op_subtract_y_from_x(&mut self, x: usize, y: usize) -> u32 {
        let vx = self.v[x];
        let vy = self.v[y];
        let borrow = if vx > vy { 1 } else { 0 };
        self.v[x] = vx.wrapping_sub(vy);
        self.v[0xF] = borrow;
        200
    }

    // 0x8XY7
    fn op_subtract_x_from_y(&mut self, x: usize, y: usize) -> u32 {
        let vx = self.v[x];
        let vy = self.v[y];
        let flag = if vy > vx { 1 } else { 0 };
        self.v[x] = vy.wrapping_sub(vx);
        self.v[0xF] = flag;
        200
    }

    // 0x8XY4
    fn op_add_with_carry(&mut self, x: usize, y: usize) -> u32 {
        let sum: u16 = u16::from(self.v[x]) + u16::from(self.v[y]);
        self.v[0xF] = if sum > 255 { 1 } else { 0 };
        self.v[x] = sum as u8;
        200
    }

    // 0x8XY1
    fn op_binary_or(&mut self, x: usize, y: usize) -> u32 {
        self.v[x] |= self.v[y];
        200
    }

    // 0x8XY3
    fn op_binary_xor(&mut self, x: usize, y: usize) -> u32 {
        self.v[x] ^= self.v[y];
        200
    }

    // 0x8XY2)
    fn op_binary_and(&mut self, x: usize, y: usize) -> u32 {
        self.v[x] &= self.v[y];
        200
    }

    // 0x00E0
    fn op_clear_screen<T: Output>(&mut self, display: &mut T) -> u32 {
        display.clear();
        109
    }

    // 0x000E
    fn op_return(&mut self) -> u32 {
        // println!("Return");
        self.pc = self.i;
        1
    }

    // 0x1NNN
    fn op_jump(&mut self, address: u16) -> u32 {
        // println!("Jump to address {}", address);
        self.pc = address;
        105 // +- 5
    }

    // 0x6XNN
    fn op_set_register(&mut self, register: usize, value: u8) -> u32 {
        println!("Setting register {} to value {}", register, value);
        self.v[register] = value;
        27
    }

    // 0x7XNN
    fn op_add_value_to_register(&mut self, register: usize, value: u8) -> u32 {
        // println!("Adding value {} to register {}", value, register);
        let mut vx = self.v[register];
        let mut vy = value;
        while vy != 0 {
            let carry = vx & vy;
            vx ^= vy;
            vy = carry << 1;
        }
        self.v[register] = vx;
        45
    }

    // 0xANNN
    fn op_set_index_register(&mut self, value: u16) -> u32 {
        println!("Setting register I to {}", value);
        self.i = value;
        55
    }

    // 0xDXYN
    fn op_display<T: Output>(&mut self, x: usize, y: usize, n: u8, display: &mut T) -> u32 {
        println!("Rendering a {} pixel tall sprite at X: {}, Y: {} from the address: {}", n, self.v[x], self.v[y], self.i);
        let mut data_index: usize = self.i.into();

        let vx = self.v[x] as u16;
        let vy = self.v[y] as u16;
        let rows = ROWS as u16;
        let cols = COLS as u16;

        for row in vy..(vy + n as u16) {
            if row >= rows {
                break;
            }
            let start_x = vx + row * cols;
            let end_x: u16 = start_x + 8;
            let mut bit_mask: u8 = 128;
            for col in start_x..end_x {
                if (col % cols) >= (start_x % cols) {
                    let position: usize = (row * cols + (col % cols)).into();
                    if (self.memory.get(data_index) & bit_mask) == bit_mask {
                        display.flip_pixel(position);
                    }
                }
                bit_mask /= 2;
            }
            data_index += 1;
        }
        display.set_draw_flag(true);
        22734 //+-4634
    }

    // 0x2NNN
    fn op_jump_to_subroutine(&mut self, address: u16) -> u32 {
        if self.sp < self.stack.len() as u8 {
            self.stack[self.sp as usize] = self.pc;
            self.sp += 1;
            self.pc = address;
        }
        105 // +-5
    }

    // 0x00EE
    fn op_return_from_subroutine(&mut self) -> u32 {
        if self.sp > 0 {
            self.sp -= 1;
            self.pc = self.stack[self.sp as usize];
        }
        105 // +- 5
    }
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn should_test_opcode_0x1xxx() {
        // arrange
        let data: Vec<u8> = [0x12, 0x02, 0x60, 0x01].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(0x1, cpu.v[0]);
    }

    #[test]
    fn should_test_opcode_0x2xxx() {
        // arrange
        let data: Vec<u8> = [0x22, 0x02, 0x00, 0xEE].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(514, cpu.pc);
    }

    #[test]
    fn should_skip_next_instruction() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x30, 0x01].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(518, cpu.pc);
    }

    #[test]
    fn should_not_skip_next_instruction() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x30, 0x02].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(516, cpu.pc);
    }

    #[test]
    fn should_skip_next_instruction_if_not_equals() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x40, 0x02].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(518, cpu.pc);
    }

    #[test]
    fn should_not_skip_next_instruction_if_equals() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x40, 0x01].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(516, cpu.pc);
    }

    #[test]
    fn should_skip_next_instruction_if_vx_equals_vy() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x61, 0x01, 0x50, 0x10].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(520, cpu.pc);
    }

    #[test]
    fn should_not_skip_next_instruction_if_vx_not_equals_vy() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x61, 0x02, 0x50, 0x10].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(518, cpu.pc);
    }

    #[test]
    fn should_test_opcode_0x6000() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(0x1, cpu.v[0]);
    }

    #[test]
    fn should_add_value_to_value_in_registry() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x70, 0x01].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(2, cpu.v[0]);
    }

    #[test]
    fn should_add_value_to_value_in_registry_and_resolve_overflow() {
        // arrange
        let data: Vec<u8> = [0x60, 0xFF, 0x70, 0xFF].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(254, cpu.v[0]);
    }

    #[test]
    fn should_test_opcode_0x8xy0() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x62, 0x02, 0x80, 0x20].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, 4);

        // assert
        assert_eq!(0x1, cpu.v[0]);
        emulate(&mut cpu, 2);
        assert_eq!(0x2, cpu.v[0]);
    }

    #[test]
    fn should_test_opcode_0x8xy1() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x61, 0x06, 0x80, 0x11].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(7, cpu.v[0]);
    }

    #[test]
    fn should_test_opcode_0x8xy2() {
        // arrange
        let data: Vec<u8> = [0x60, 0x0C, 0x61, 0x06, 0x80, 0x12].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(4, cpu.v[0]);
    }

    #[test]
    fn should_test_opcode_0x8xy3() {
        // arrange
        let data: Vec<u8> = [0x60, 0x09, 0x61, 0x05, 0x80, 0x13].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(12, cpu.v[0]);
    }

    #[test]
    fn should_test_opcode_0x8xy4_no_carry_set_register_f_to_zero() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x61, 0x01, 0x80, 0x14].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(2, cpu.v[0]);
        assert_eq!(0, cpu.v[0xF]);
    }

    #[test]
    fn should_test_opcode_0x8xy4_with_carry_set_register_f_to_one() {
        // arrange
        let data: Vec<u8> = [0x60, 0xF1, 0x61, 0xF1, 0x80, 0x14].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(226, cpu.v[0]);
        assert_eq!(1, cpu.v[0xF]);
    }

    #[test]
    fn should_test_opcode_0x8xy5_if_sum_is_non_negative_value() {
        // arrange
        let data: Vec<u8> = [0x60, 0x03, 0x61, 0x02, 0x80, 0x15].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(1, cpu.v[0]);
        assert_eq!(1, cpu.v[0xF]);
    }

    #[test]
    fn should_test_opcode_0x8xy5_if_sum_is_negative_value() {
        // arrange
        let data: Vec<u8> = [0x60, 0x02, 0x61, 0x03, 0x80, 0x15].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(255, cpu.v[0]);
        assert_eq!(0, cpu.v[0xF]);
    }

    #[test]
    fn should_test_opcode_0x8xy6_if_sum_has_least_significant_bit_of_one() {
        // arrange
        let data: Vec<u8> = [0x60, 0x03, 0x80, 0x16].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(1, cpu.v[0]);
        assert_eq!(1, cpu.v[0xF]);
    }

    #[test]
    fn should_test_opcode_0x8xy6_if_sum_has_least_significant_bit_of_zero() {
        // arrange
        let data: Vec<u8> = [0x60, 0x02, 0x80, 0x16].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(1, cpu.v[0]);
        assert_eq!(0, cpu.v[0xF]);
    }

    #[test]
    fn should_test_opcode_0x8xy7_if_there_is_no_borrow_set_vf_to_one() {
        // arrange
        let data: Vec<u8> = [0x60, 0x02, 0x61, 0x03, 0x80, 0x17].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(1, cpu.v[0]);
        assert_eq!(1, cpu.v[0xF]);
    }

    #[test]
    fn should_test_opcode_0x8xy7_if_there_is_borrow_set_vf_to_zero() {
        // arrange
        let data: Vec<u8> = [0x60, 0x03, 0x61, 0x02, 0x80, 0x17].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(255, cpu.v[0]);
        assert_eq!(0, cpu.v[0xF]);
    }

    #[test]
    fn should_test_opcode_0x8xye_msb_should_be_one() {
        // arrange
        let data: Vec<u8> = [0x60, 0xFF, 0x80, 0x0E].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(254, cpu.v[0]);
        assert_eq!(1, cpu.v[0xF]);
    }

    #[test]
    fn should_test_opcode_0x8xye_mbs_should_be_zero() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x80, 0x0E].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(2, cpu.v[0]);
        assert_eq!(0, cpu.v[0xF]);
    }

    #[test]
    fn should_test_opcode_0x9xy0_skips_next_instruction() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x61, 0x02, 0x90, 0x10].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(520, cpu.pc);
    }

    #[test]
    fn should_test_opcode_0x9xy0_should_not_skips_next_instruction() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0x61, 0x01, 0x90, 0x10].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(518, cpu.pc);
    }

    #[test]
    fn should_test_opcode_0xannn_should_set_i_to_nnn() {
        // arrange
        let data: Vec<u8> = [0xAF, 0xFF].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(4095, cpu.i);
    }

    #[test]
    fn should_test_opcode_0xbnnn_should_jump_to_nnn_plus_vzero() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0xB2, 0x05].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(518, cpu.pc);
    }

    #[test]
    fn should_test_opcode_fx15_set_delay_timer_to_vx() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0xF0, 0x15].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(1, cpu.dt);
    }

    #[test]
    fn should_test_opcode_fx18_set_sound_timer_to_vx() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0xF0, 0x18].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(1, cpu.st);
    }

    #[test]
    fn should_test_opcode_fx1e_add_vx_to_i() {
        // arrange
        let data: Vec<u8> = [0x60, 0x01, 0xF0, 0x1E].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);

        // act
        emulate(&mut cpu, data.len());

        // assert
        assert_eq!(1, cpu.i);
    }

    #[test]
    fn should_test_opcode_fx07_set_vx_to_the_delay_timer() {
        // arrange
        let input = FakeInput {};
        let mut output = FakeOutput {};
        let mut audio = FakeAudioPlayer {};
        let data: Vec<u8> = [0xF0, 0x07].to_vec();
        let mut memory = Memory::new();
        memory.load_data(512, &data);
        let mut cpu = CPU::new(memory);
        cpu.dt = 2;

        // act
        cpu.tick(&input, &mut output, &mut audio);

        // assert
        assert_eq!(1, cpu.v[0]);
    }

    fn emulate(cpu: &mut CPU, iterations: usize) {
        let input = FakeInput {};
        let mut output = FakeOutput {};

        for _ in 0..(iterations / 2) {
            cpu.emulate_cycle(&input, &mut output);
        }
    }

struct FakeInput {}
impl Input for FakeInput {
    fn is_pressed(&self, _key: usize) -> bool {
        false
    }
    fn has_been_released(&self, _key: usize) -> bool {
        false
    }
}

struct FakeOutput {}
impl Output for FakeOutput {
    fn clear(&mut self) {}
    fn get_pixel(&self, _addr: usize) -> bool {
        false
    }
    fn set_pixel(&mut self, _addr: usize, _value: bool) {}
    fn flip_pixel(&mut self, _addr: usize) {}
    fn set_draw_flag(&mut self, _value: bool) {}
}

struct FakeAudioPlayer {}
impl Speaker for FakeAudioPlayer {
    fn start(&mut self) {}
    fn stop(&mut self) {}
    fn is_playing(&self) -> bool {
        false
    }
}
}