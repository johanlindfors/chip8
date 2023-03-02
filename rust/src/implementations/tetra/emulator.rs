use tetra::time::Timestep;
use tetra::{Context, ContextBuilder, State, TetraError};

use crate::chip8::cpu::CPU;

use super::audio::Audio;
use super::display::Display;
use super::keyboard::Keyboard;

pub struct Emulator {
    cpu: CPU,
    display: Display,
    keyboard: Keyboard,
    audio: Audio,
}

impl Emulator {
    pub fn run(cpu: CPU) -> Result<(), TetraError> {
        let keyboard = Keyboard::new();

        ContextBuilder::new("Hello, chip8!", 640, 320)
            .timestep(Timestep::Variable)
            .key_repeat(true)
            .high_dpi(true)
            .quit_on_escape(true)
            .build()?
            .run(|ctx| {
                Ok(Emulator {
                    cpu,
                    display: Display::new(ctx)?,
                    keyboard,
                    audio: Audio::new(ctx)?,
                })
            })?;
        Ok(())
    }
}

impl State for Emulator {
    fn update(&mut self, ctx: &mut Context) -> Result<(), TetraError> {
        self.keyboard.handle_input(ctx);
        self.cpu
            .tick(&self.keyboard, &mut self.display, &mut self.audio);
        Ok(())
    }

    fn draw(&mut self, ctx: &mut Context) -> tetra::Result {
        self.display.draw(ctx);
        Ok(())
    }
}
