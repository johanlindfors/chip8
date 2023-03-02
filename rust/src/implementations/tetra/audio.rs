use crate::chip8::Speaker;
use tetra::audio::{self, Sound, SoundInstance, SoundState};
use tetra::Context;

pub struct Audio {
    channel: SoundInstance,
}

impl Audio {
    pub fn new(ctx: &mut Context) -> tetra::Result<Self> {
        audio::set_master_volume(ctx, 0.4);
        let sound = Sound::new("./resources/audio/beep.ogg")?;
        let channel = sound.spawn(ctx)?;
        channel.set_repeating(true);

        Ok(Audio { channel })
    }
}

impl Speaker for Audio {
    fn start(&mut self) {
        self.channel.play();
    }

    fn stop(&mut self) {
        self.channel.stop();
    }

    fn is_playing(&self) -> bool {
        self.channel.state() == SoundState::Playing
    }
}
