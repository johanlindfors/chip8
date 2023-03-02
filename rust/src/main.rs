use std::env;
use std::fs;

use chip8::cpu::CPU;
use chip8::memory::Memory;
//use implementations::tetra::emulator::Emulator;
use implementations::sdl2::emulator::Emulator;

pub mod chip8;
pub mod implementations;

fn main() {
    let args: Vec<String> = env::args().collect();
    let file_path = &args[1];
    let data = fs::read(file_path).expect("Failed to open file");

    let mut memory = Memory::new();
    memory.load_data(512, &data);

    Emulator::run(CPU::new(memory)).unwrap();
}
