import array as arr
import sys, math, random, pygame
from pygame.locals import *

class Memory:
    memory = bytearray(4096)

    def __init__(self, size):
        self.size = size

    def set(self, address, value):
        self.memory[address] = value

    def get(self, address):
        return self.memory[address]

SCREEN_WIDTH = 64
SCREEN_HEIGHT = 32
SCALE = 10
BLACK = 0, 0, 0
WHITE = 255, 255, 255

class Display:
    pixels = bytearray(SCREEN_WIDTH * SCREEN_HEIGHT)
    drawFlag = False

    def __init__(self, width, height):
        self.width = width
        self.height = height

    def setPixel(self, x, y):
        self.pixels[x + (y * SCREEN_WIDTH)] ^= 1

    def getPixel(self, x, y):
        return self.pixels[x + (y * SCREEN_WIDTH)]

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

    def play(self):
        print("play sound")

class Input:
    def __init__(self):
        pass

    def read(self):
        print("read input")

class CPU:
    def __init__(self, name, age):
        self.name = name
        self.age = age
    
    def doWork(self):
        print("do work " + self.name)