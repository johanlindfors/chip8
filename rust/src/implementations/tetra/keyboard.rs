use crate::chip8::Input;
use tetra::input::{self, Key};
use tetra::Context;

#[derive(Default)]
pub struct Keyboard {
    pub state: [bool; 16],
    pub last_state: [bool; 16],
}

impl Input for Keyboard {
    fn is_pressed(&self, key: usize) -> bool {
        self.state[key]
    }

    fn has_been_released(&self, key: usize) -> bool {
        self.last_state[key] && !self.state[key]
    }
}

impl Keyboard {
    pub fn new() -> Self {
        Keyboard {
            state: [false; 16],
            last_state: [false; 16],
        }
    }

    pub fn handle_input(&mut self, ctx: &mut Context) {
        // store old state
        self.last_state[..16].clone_from_slice(&self.state);

        let mut pressed = input::get_keys_pressed(ctx).peekable();
        if pressed.peek().is_some() {
            self.keys_pressed(pressed.collect::<Vec<&Key>>());
        }

        let mut released = input::get_keys_released(ctx).peekable();
        if released.peek().is_some() {
            self.keys_released(released.collect::<Vec<&Key>>());
        }
    }

    fn keys_pressed(&mut self, pressed: Vec<&Key>) {
        for key in pressed {
            match key {
                Key::Num1 => self.state[0x1] = true,
                Key::Num2 => self.state[0x2] = true,
                Key::Num3 => self.state[0x3] = true,
                Key::Num4 => self.state[0xC] = true,

                Key::Q => self.state[0x4] = true,
                Key::W => self.state[0x5] = true,
                Key::E => self.state[0x6] = true,
                Key::R => self.state[0xD] = true,

                Key::A => self.state[0x7] = true,
                Key::S => self.state[0x8] = true,
                Key::D => self.state[0x9] = true,
                Key::F => self.state[0xE] = true,

                Key::Z => self.state[0xA] = true,
                Key::X => self.state[0x0] = true,
                Key::C => self.state[0xB] = true,
                Key::V => self.state[0xF] = true,
                _ => {}
            }
        }
    }

    fn keys_released(&mut self, released: Vec<&Key>) {
        for key in released {
            match key {
                Key::Num1 => self.state[0x1] = false,
                Key::Num2 => self.state[0x2] = false,
                Key::Num3 => self.state[0x3] = false,
                Key::Num4 => self.state[0xC] = false,

                Key::Q => self.state[0x4] = false,
                Key::W => self.state[0x5] = false,
                Key::E => self.state[0x6] = false,
                Key::R => self.state[0xD] = false,

                Key::A => self.state[0x7] = false,
                Key::S => self.state[0x8] = false,
                Key::D => self.state[0x9] = false,
                Key::F => self.state[0xE] = false,

                Key::Z => self.state[0xA] = false,
                Key::X => self.state[0x0] = false,
                Key::C => self.state[0xB] = false,
                Key::V => self.state[0xF] = false,
                _ => {}
            }
        }
    }
}
