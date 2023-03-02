package chip8

import (
	"github.com/veandco/go-sdl2/sdl"
)

type Input interface {
	IsPressed(key byte) bool;
	HasBeenReleased(key byte) bool;
}

type Keyboard struct {
	state		[16]bool
	last_state  [16]bool
}

func (k *Keyboard) update() {
	for i := 0; i < len(k.state); i++ {
		k.last_state[i] = k.state[i];
	}
}

func (k* Keyboard) handleKeyDown(keycode sdl.Keycode) {
	switch keycode {
	case sdl.K_1: k.state[0x1] = true;
	case sdl.K_2: k.state[0x2] = true;
	case sdl.K_3: k.state[0x3] = true;
	case sdl.K_4: k.state[0xC] = true;

	case sdl.K_q: k.state[0x4] = true;
	case sdl.K_w: k.state[0x5] = true;
	case sdl.K_e: k.state[0x6] = true;
	case sdl.K_r: k.state[0xD] = true;

	case sdl.K_a: k.state[0x7] = true;
	case sdl.K_s: k.state[0x8] = true;
	case sdl.K_d: k.state[0x9] = true;
	case sdl.K_f: k.state[0xE] = true;

	case sdl.K_z: k.state[0xA] = true;
	case sdl.K_x: k.state[0x0] = true;
	case sdl.K_c: k.state[0xB] = true;
	case sdl.K_v: k.state[0xF] = true;
	}
}

func (k* Keyboard) handleKeyUp(keycode sdl.Keycode) {
	switch keycode {
	case sdl.K_1: k.state[0x1] = false;
	case sdl.K_2: k.state[0x2] = false;
	case sdl.K_3: k.state[0x3] = false;
	case sdl.K_4: k.state[0xC] = false;

	case sdl.K_q: k.state[0x4] = false;
	case sdl.K_w: k.state[0x5] = false;
	case sdl.K_e: k.state[0x6] = false;
	case sdl.K_r: k.state[0xD] = false;

	case sdl.K_a: k.state[0x7] = false;
	case sdl.K_s: k.state[0x8] = false;
	case sdl.K_d: k.state[0x9] = false;
	case sdl.K_f: k.state[0xE] = false;

	case sdl.K_z: k.state[0xA] = false;
	case sdl.K_x: k.state[0x0] = false;
	case sdl.K_c: k.state[0xB] = false;
	case sdl.K_v: k.state[0xF] = false;
	}
}

func (k *Keyboard) IsPressed(key byte) bool {
	return k.state[key]
}

func (k *Keyboard) HasBeenReleased(key byte) bool {
	return k.last_state[key] == true && k.state[key] == false
}
