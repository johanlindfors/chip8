import sys, math, random, pygame
from pygame.locals import *

SCREEN_WIDTH = 64
SCREEN_HEIGHT = 32
SCALE = 10
BLACK = 0, 0, 0
WHITE = 255, 255, 255
FRAME_TICKS = 16667

class Memory:
    memory = bytearray(4096)

    def __init__(self, size):
        self.size = size

    def loadRom(self, rom: bytearray):
        for index in range(len(rom)):
            self.set(0x200 + index, rom[index])

    def set(self, address:int , value: int):
        self.memory[address] = value

    def get(self, address: int):
        return self.memory[address]
    
    def getOpCode(self, pc: int) -> int:
        return self.memory[pc] << 8 | self.memory[pc + 1]

class Display:
    pixels = bytearray(SCREEN_WIDTH * SCREEN_HEIGHT)
    drawFlag = False

    def __init__(self, width, height):
        self.width = width
        self.height = height

    def setPixel(self, x, y):
        self.pixels[x + (y * SCREEN_WIDTH)] ^= 1

    def getPixel(self, x, y) -> bool:
        return self.pixels[x + (y * SCREEN_WIDTH)] == 1

    def setDrawFlag(self):
        self.drawFlag = True

    def draw(self, screen : pygame.Surface):
        if self.drawFlag:
            screen.fill(BLACK, (0, 0, self.width, self.height))
            for index in range(0, len(self.pixels)):
                if self.pixels[index] == 1:
                    x = index % SCREEN_WIDTH
                    y = index // SCREEN_WIDTH
                    screen.fill(WHITE, (x * SCALE, y * SCALE, SCALE, SCALE))
            self.drawFlag = False

class Sound:
    def __init__(self):
        pass

    def start(self):
        print("start sound")

    def stop(self):
        print("stop sound")

    def isPlaying() -> bool:
        return False

class Input:
    def __init__(self):
        pass

    def read(self):
        print("read input")

class CPU:
    memory: Memory
    audio: Sound
    soundTimer: int
    delayTimer: int
    pc: int

    def __init__(self, name):
        self.name = name
        self.soundTimer = 0
        self.delayTimer = 0
        pc = 0x200
    
    def attachMemory(self, memory: Memory):
        self.memory = memory
    
    def doWork(self):
        print("do work " + self.name)

    def tick(self):
        if self.delayTimer > 0:
            self.delayTimer -= 1
        if self.soundTimer > 0:
            self.soundTimer -= 1
            if self.audio.isPlaying == False:
                self.audio.start()
        elif self.audio.isPlaying == True:
            self.audio.stop()
            
        microseconds = FRAME_TICKS
        delta = 0
        while True:
            delta = self.emulateCycle()
            if microseconds <= 0 or delta == 0:
                break

    def emulateCycle(self) -> int:
        opCode = self.memory.getOpCode(self.pc)
        x = (opCode & 0x0F00) >> 8
        y = (opCode & 0x00F0) >> 4
        vx = self.memory.get(x)
        vy = self.memory.get(y)
        n = opCode & 0x000F
        nn = opCode & 0x00FF
        nnn = opCode & 0x0FFF
        self.doWork()
        return 0