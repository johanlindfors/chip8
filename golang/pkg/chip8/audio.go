package chip8

// typedef unsigned char Uint8;
// void SineWave(void *userdata, Uint8 *stream, int len);
import "C"
import (
	"log"
	"math"
	"reflect"
	"unsafe"

	"github.com/veandco/go-sdl2/sdl"
)

type Audio struct {
	dev  sdl.AudioDeviceID
	spec sdl.AudioSpec
}

const (
	toneHz   = 440
	sampleHz = 48000
	dPhase   = 2 * math.Pi * toneHz / sampleHz
)

//export SineWave
func SineWave(userdata unsafe.Pointer, stream *C.Uint8, length C.int) {
	n := int(length)
	hdr := reflect.SliceHeader{Data: uintptr(unsafe.Pointer(stream)), Len: n, Cap: n}
	buf := *(*[]C.Uint8)(unsafe.Pointer(&hdr))

	var phase float64
	for i := 0; i < n; i += 2 {
		phase += dPhase
		sample := C.Uint8((math.Sin(phase) + 0.999999) * 128)
		buf[i] = sample
		buf[i+1] = sample
	}
}

func NewAudio() *Audio {
	var dev sdl.AudioDeviceID
	var err error

	spec := &sdl.AudioSpec{
		Freq:     sampleHz,
		Format:   sdl.AUDIO_U8,
		Channels: 2,
		Samples:  sampleHz,
		Callback: sdl.AudioCallback(C.SineWave),
	}
	if dev, err = sdl.OpenAudioDevice("", false, spec, nil, 0); err != nil {
		log.Println(err)
		panic(err)
	}
	sdl.PauseAudioDevice(dev, true)
	log.Printf("Audio created device %v", dev)

	return &Audio{dev: dev, spec: *spec}
}

func (a *Audio) start() {
	log.Printf("Audio start device %v", a.dev)
	sdl.PauseAudioDevice(a.dev, false)
}

func (a *Audio) stop() {
	log.Println("Audio stop")
	sdl.PauseAudioDevice(a.dev, true)
}

func (a *Audio) isPlaying() bool {
	return sdl.GetAudioDeviceStatus(a.dev) == sdl.AUDIO_PLAYING
}
