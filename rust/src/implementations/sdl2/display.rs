use sdl2::{pixels::Color, rect::Rect, render::WindowCanvas, video::Window};

use crate::chip8::{Output, COLS, ROWS, SCALE};

pub struct Display {
    canvas: WindowCanvas,
    pub draw_flag: bool,
    pub frame_buffer: [bool; COLS as usize * ROWS as usize], // display buffer
}

impl Display {
    pub fn new(window: Window) -> Result<Self, String> {
        Ok(Display {
            canvas: window.into_canvas().build().unwrap(),
            draw_flag: false,
            frame_buffer: [false; COLS as usize * ROWS as usize],
        })
    }

    pub fn draw(&mut self) {
        if self.draw_flag {
            self.canvas.set_draw_color(Color::RGB(0, 0, 0));
            self.canvas.clear();
            self.canvas.set_draw_color(Color::RGB(255, 255, 255));
            for y in 0..ROWS {
                for x in 0..COLS {
                    if self.frame_buffer[y as usize * COLS as usize + x as usize] {
                        self.canvas
                            .fill_rect(Rect::new(
                                x as i32 * SCALE as i32,
                                y as i32 * SCALE as i32,
                                SCALE as u32,
                                SCALE as u32,
                            ))
                            .unwrap();
                    }
                }
            }
            self.canvas.present();
            self.draw_flag = false;
        }
    }
}

impl Output for Display {
    fn clear(&mut self) {
        self.frame_buffer = [false; 64 * 32];
        self.draw_flag = true;
    }

    fn get_pixel(&self, addr: usize) -> bool {
        self.frame_buffer[addr]
    }

    fn set_pixel(&mut self, addr: usize, value: bool) {
        self.frame_buffer[addr] = value;
    }

    fn flip_pixel(&mut self, addr: usize) {
        println!("Flipping pixel {}", addr);
        self.frame_buffer[addr] = !self.frame_buffer[addr];
    }

    fn set_draw_flag(&mut self, value: bool) {
        self.draw_flag = value;
    }
}
