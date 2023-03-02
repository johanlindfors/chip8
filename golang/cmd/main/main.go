package main

import (
	"io/ioutil"
	"os"

	"chip8-emulator-go/pkg/chip8"
)

func main() {
	filename := os.Args[1]
	data, _ := ioutil.ReadFile(filename)
	chip8.RunEmulator(data) 
}
