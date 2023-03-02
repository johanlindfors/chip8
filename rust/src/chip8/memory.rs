use super::{RAM_SIZE, SPRITE_CHARS, SPRITE_CHARS_ADDR};

pub struct Memory {
    ram: [u8; RAM_SIZE], // RAM
}

impl Memory {
    pub fn new() -> Self {
        let mut memory = Memory::default();

        // load predefined sprites
        for (i, sprite) in SPRITE_CHARS.iter().enumerate() {
            let p = SPRITE_CHARS_ADDR as usize + i * sprite.len();
            memory.load_data(p, sprite);
        }
        memory
    }

    pub fn load_data(&mut self, addr: usize, data: &[u8]) {
        self.ram[addr..addr + data.len()].copy_from_slice(data);
    }

    pub fn get(&self, addr: usize) -> u8 {
        self.ram[addr]
    }

    pub fn set(&mut self, addr: usize, value: u8) {
        self.ram[addr] = value;
    }
}

impl Default for Memory {
    fn default() -> Self {
        Self { ram: [0; 4096] }
    }
}
