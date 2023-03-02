extern crate sdl2;

use sdl2::event::Event;
use sdl2::keyboard::Keycode;
use std::time::Duration;

use crate::chip8::cpu::CPU;

use super::audio::Audio;
use super::display::Display;
use super::keyboard::Keyboard;

pub struct Emulator {
    _cpu: CPU,
    _display: Display,
    _keyboard: Keyboard,
    _audio: Audio,
}

impl Emulator {
    pub fn run(mut cpu: CPU) -> Result<(), String> {
        let sdl_context = sdl2::init()?;
        let video_subsystem = sdl_context.video()?;

        let window = video_subsystem
            .window("Hello chip8!", 640, 320)
            .position_centered()
            .opengl()
            .build()
            .map_err(|e| e.to_string())?;

        let mut event_pump = sdl_context.event_pump()?;

        let mut keyboard = Keyboard::new();
        let mut display = Display::new(window).unwrap();
        let mut audio = Audio::new(&sdl_context);

        'running: loop {
            keyboard.handle_input();
            for event in event_pump.poll_iter() {
                match event {
                    Event::Quit { .. } => break 'running,
                    Event::KeyDown {
                        keycode: Some(keycode),
                        ..
                    } => {
                        if keycode == Keycode::Escape {
                            break 'running;
                        } else {
                            keyboard.handle_key_down(keycode);
                        }
                    }
                    Event::KeyUp {
                        keycode: Some(keycode),
                        ..
                    } => {
                        keyboard.handle_key_up(keycode);
                    }
                    _ => {}
                }
            }
            cpu.tick(&keyboard, &mut display, &mut audio);
            if display.draw_flag {
                display.draw();
            }

            ::std::thread::sleep(Duration::new(0, 1_000_000_000u32 / 60));
        }
        Ok(())
    }
}
