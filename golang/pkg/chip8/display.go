package chip8

import (
	"fmt"

	"github.com/veandco/go-sdl2/sdl"
)

type Output interface {
	draw()
	clear()
	flipPixel(addr uint16)
	setDrawFlag(value bool)
}

type Display struct {
	display  [64 * 32]bool
	drawFlag bool
	renderer *sdl.Renderer
}

func (d *Display) draw() {
	if d.drawFlag {
		d.renderer.SetDrawColor(0, 0, 0, 255)
		d.renderer.Clear()
		d.renderer.SetDrawColor(255, 255, 255, 255)
		for y := 0; y < 32; y++ {
			for x := 0; x < 64; x++ {
				if d.display[y*64+x] {
					d.renderer.FillRect(&sdl.Rect{X: int32(x * 10), Y: int32(y * 10), W: 10, H: 10})
				}
			}
		}
		d.renderer.Present()
		d.drawFlag = false
	}
}

func (d *Display) clear() {
	d.display = [len(d.display)]bool{false}
	d.drawFlag = true
}

func (d *Display) flipPixel(addr uint16) {
	fmt.Printf("Flipping pixel %v\n", addr)
	d.display[addr] = !d.display[addr]
}

func (d *Display) setDrawFlag(value bool) {
	d.drawFlag = value
}
