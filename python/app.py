import sys, math, random, pygame
from pygame.locals import *

SCREEN_WIDTH = 64
SCREEN_HEIGHT = 32
SCALE = 10
BLACK = 0, 0, 0
RED = 255, 0, 0
GREEN = 0, 255, 0
FRAMES_PER_SECOND = 15
    
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

# def update():
#     # updates here

def draw(screen):
    # clear screen
    screen.fill(BLACK)
    # draw screen

def run():
    width = SCREEN_WIDTH * SCALE
    height = SCREEN_HEIGHT * SCALE
    size = width, height 

    screen = pygame.display.set_mode(size)
    clock = pygame.time.Clock()

    pygame.init()
    while True:
        clock.tick(FRAMES_PER_SECOND)
        handle_input()
        # update()
        # draw(screen)
        pygame.display.flip()

run()