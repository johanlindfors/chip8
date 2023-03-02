use sdl2::keyboard::Keycode;

use crate::chip8::Input;

#[derive(Default)]
pub struct Keyboard {
    pub state: [bool; 16],
    pub last_state: [bool; 16],
}

impl Keyboard {
    pub fn new() -> Self {
        Keyboard {
            state: [false; 16],
            last_state: [false; 16],
        }
    }

    pub fn handle_input(&mut self) {
        // store old state
        self.last_state[..16].clone_from_slice(&self.state);
    }

    pub fn handle_key_down(&mut self, keycode: Keycode) {
        match keycode {
            Keycode::Num1 => self.state[0x1] = true,
            Keycode::Num2 => self.state[0x2] = true,
            Keycode::Num3 => self.state[0x3] = true,
            Keycode::Num4 => self.state[0xC] = true,

            Keycode::Q => self.state[0x4] = true,
            Keycode::W => self.state[0x5] = true,
            Keycode::E => self.state[0x6] = true,
            Keycode::R => self.state[0xD] = true,

            Keycode::A => self.state[0x7] = true,
            Keycode::S => self.state[0x8] = true,
            Keycode::D => self.state[0x9] = true,
            Keycode::F => self.state[0xE] = true,

            Keycode::Z => self.state[0xA] = true,
            Keycode::X => self.state[0x0] = true,
            Keycode::C => self.state[0xB] = true,
            Keycode::V => self.state[0xF] = true,
            _ => {}
        };
    }

    pub fn handle_key_up(&mut self, keycode: Keycode) {
        match keycode {
            Keycode::Num1 => self.state[0x1] = false,
            Keycode::Num2 => self.state[0x2] = false,
            Keycode::Num3 => self.state[0x3] = false,
            Keycode::Num4 => self.state[0xC] = false,

            Keycode::Q => self.state[0x4] = false,
            Keycode::W => self.state[0x5] = false,
            Keycode::E => self.state[0x6] = false,
            Keycode::R => self.state[0xD] = false,

            Keycode::A => self.state[0x7] = false,
            Keycode::S => self.state[0x8] = false,
            Keycode::D => self.state[0x9] = false,
            Keycode::F => self.state[0xE] = false,

            Keycode::Z => self.state[0xA] = false,
            Keycode::X => self.state[0x0] = false,
            Keycode::C => self.state[0xB] = false,
            Keycode::V => self.state[0xF] = false,
            _ => {}
        };
    }
}

impl Input for Keyboard {
    fn is_pressed(&self, key: usize) -> bool {
        self.state[key]
    }

    fn has_been_released(&self, key: usize) -> bool {
        self.last_state[key] && !self.state[key]
    }
}
