package chip8

import (
	"time"

	"github.com/veandco/go-sdl2/sdl"
)

var SPRITE_CHARS = []uint8{
	0xF0, 0x90, 0x90, 0x90, 0xF0, //0
	0x20, 0x60, 0x20, 0x20, 0x70, //1
	0xF0, 0x10, 0xF0, 0x80, 0xF0, //2
	0xF0, 0x10, 0xF0, 0x10, 0xF0, //3
	0x90, 0x90, 0xF0, 0x10, 0x10, //4
	0xF0, 0x80, 0xF0, 0x10, 0xF0, //5
	0xF0, 0x80, 0xF0, 0x90, 0xF0, //6
	0xF0, 0x10, 0x20, 0x40, 0x40, //7
	0xF0, 0x90, 0xF0, 0x90, 0xF0, //8
	0xF0, 0x90, 0xF0, 0x10, 0xF0, //9
	0xF0, 0x90, 0xF0, 0x90, 0x90, //A
	0xE0, 0x90, 0xE0, 0x90, 0xE0, //B
	0xF0, 0x80, 0x80, 0x80, 0xF0, //C
	0xE0, 0x90, 0x90, 0x90, 0xE0, //D
	0xF0, 0x80, 0xF0, 0x80, 0xF0, //E
	0xF0, 0x80, 0xF0, 0x80, 0x80, //F
}

const (
	SPRITE_CHARS_ADDR     uint16 = 0x0000
	FRAME_TICKS           uint16 = 16666
	RAM_SIZE              uint16 = 4096
	PROGRAM_START_ADDRESS uint16 = 0x0200
	REGISTER_COUNT        uint8  = 16
	STACK_DEPTH           uint8  = 16
	SCALE                        = 10.0
	COLS                  uint8  = 64
	ROWS                  uint8  = 32
)

type Emulator struct {
	cpu      CPU
	display  Display
	keyboard Keyboard
	audio    Audio
}

func RunEmulator(program []uint8) {
	if err := sdl.Init(sdl.INIT_EVERYTHING); err != nil {
		panic(err)
	}
	defer sdl.Quit()

	window, err := sdl.CreateWindow("Hello chip8!", sdl.WINDOWPOS_UNDEFINED, sdl.WINDOWPOS_UNDEFINED, 640, 320, sdl.WINDOW_SHOWN)
	if err != nil {
		return
	}
	defer window.Destroy()

	renderer, rendererError := sdl.CreateRenderer(window, -1, 0)
	if rendererError != nil {
		panic(rendererError)
	}
	display := Display{renderer: renderer}
	keyboard := Keyboard{}
	audio := NewAudio()
	memory := Memory{}
	memory.LoadData(PROGRAM_START_ADDRESS, program)
	memory.LoadData(SPRITE_CHARS_ADDR, SPRITE_CHARS)
	emulator := Emulator{
		cpu:      CPU{memory: memory, pc: PROGRAM_START_ADDRESS},
		display:  display,
		keyboard: keyboard,
		audio:    *audio}

	running := true
	for running {
		keyboard.update()
		for event := sdl.PollEvent(); event != nil; event = sdl.PollEvent() {
			switch t := event.(type) {
			case *sdl.QuitEvent:
				running = false
			case *sdl.KeyboardEvent:
				keycode := t.Keysym.Sym
				if keycode == sdl.K_ESCAPE {
					running = false
				}
				switch t.State {
				case sdl.PRESSED:
					keyboard.handleKeyDown(keycode)
				case sdl.RELEASED:
					keyboard.handleKeyUp(keycode)
				}
			}
		}
		emulator.cpu.tick(&keyboard, &display, audio)
		if display.drawFlag {
			display.draw()
		}
		time.Sleep(time.Millisecond * 1000 / 60)
	}
}
