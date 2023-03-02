use crate::chip8::{Output, COLS, ROWS, SCALE};
use tetra::graphics::{self, Canvas, Color, DrawParams, Texture};
use tetra::math::Vec2;
use tetra::Context;

pub struct Display {
    canvas: Canvas,
    texture: Texture,
    pub draw_flag: bool,
    pub frame_buffer: [bool; COLS as usize * ROWS as usize], // display buffer
}

impl Display {
    pub fn new(ctx: &mut Context) -> tetra::Result<Self> {
        Ok(Display {
            canvas: Canvas::new(ctx, COLS as i32, ROWS as i32)?,
            texture: Texture::new(ctx, "./resources/textures/square.png")?,
            draw_flag: false,
            frame_buffer: [false; COLS as usize * ROWS as usize],
        })
    }

    pub fn draw(&mut self, ctx: &mut Context) {
        if self.draw_flag {
            graphics::set_canvas(ctx, &self.canvas);
            graphics::clear(ctx, Color::BLACK);
            for y in 0..ROWS {
                for x in 0..COLS {
                    if self.frame_buffer[y as usize * COLS as usize + x as usize] {
                        self.texture.draw(
                            ctx,
                            DrawParams::new().position(Vec2::new(x as f32, y as f32)),
                        );
                    }
                }
            }
            graphics::reset_canvas(ctx);
            self.draw_flag = false;
        }
        self.canvas.draw(
            ctx,
            DrawParams::new()
                .position(Vec2::zero())
                .scale(Vec2 { x: SCALE, y: SCALE }),
        );
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
        self.frame_buffer[addr] = !self.frame_buffer[addr];
    }

    fn set_draw_flag(&mut self, value: bool) {
        self.draw_flag = value;
    }
}
