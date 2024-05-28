import sys, math, random, pygame
from pygame.locals import *

import chip8

FRAMES_PER_SECOND = 60
    
def handle_input():
    for event in pygame.event.get():
        if event.type == pygame.QUIT: 
            sys.exit()
        # elif event.type == KEYDOWN:
        #     if speed.x == 0:
        #         if event.key == K_LEFT:
        #             do_work()
        #         elif event.key == K_RIGHT:
        #             # move right
        #     if speed.y == 0:
        #         if event.key == K_UP:
        #             # move up
        #         elif event.key == K_DOWN:
        #             # move down
        #     if event.key == K_ESCAPE:
        #         sys.exit()              

def update(cpu):
    # updates here
    cpu.doWork()

def run():
    width = chip8.SCREEN_WIDTH * chip8.SCALE
    height = chip8.SCREEN_HEIGHT * chip8.SCALE
    size = width, height 

    screen = pygame.display.set_mode(size)
    clock = pygame.time.Clock()

    cpu = chip8.CPU("Johan", 50)
    display = chip8.Display(64, 32)
    display.setPixel(10,10)
    display.setPixel(11,11)
    display.setDrawFlag()

    pygame.init()
    while True:
        clock.tick(FRAMES_PER_SECOND)
        handle_input()
        update(cpu)
        display.draw(screen)
        pygame.display.flip()

run()