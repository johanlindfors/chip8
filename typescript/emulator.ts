/// <reference path="cpu.ts"/>
/// <reference path="keyboard.ts"/>
/// <reference path="display.ts"/>

// variables
const SPRITE_SIZE = 10
const FPS = 60;

class Emulator {
    ctx: CanvasRenderingContext2D;
    width: number;
    height: number;
    cpu: CPU;
    keyboard: IKeyboard;
    display: IDisplay;
    memory: ILoadProgram;
        
    constructor() {
        let surface = <HTMLCanvasElement>document.getElementById("surface");
        this.ctx = surface.getContext("2d")!;
        this.width = surface.clientWidth;
        this.height = surface.clientHeight;
        this.keyboard = new Keyboard();
        this.display = new Display(64, 32);
        let memory = new Memory();
        this.memory = memory;
        let audio = new AudioPlayer();
        this.cpu = new CPU(this.keyboard, this.display, memory, audio);
    }

    initialize() : void {
        let client = new ApiClient();
        client.getRom("15PUZZLE.ch8")
            .then(rom => {
                const theRom = new Uint8Array(rom);
                this.memory.loadProgram(0x0200, theRom);
            });
    }
    

    clearScreen() : void {
        this.ctx.fillStyle = "red";
        this.ctx.fillRect(0, 0, this.width, this.height);
    }

    handleInput(evt: KeyboardEvent, isKeyDown: boolean) {
        if(isKeyDown)
            this.keyboard.onKeyPressed(evt.code);
        else
            this.keyboard.onKeyReleased(evt.code);
    }

    update() : void {
        this.cpu.tick();
    }

    draw() : void {
        //this.clearScreen();
        this.display.draw(this.ctx);
    }
}

let emulator = new Emulator();
emulator.initialize();

function keyDown(evt) : void {
    emulator.handleInput(evt, true);
}

function keyUp(evt) : void {
    emulator.handleInput(evt, false);
}

function gameLoop() : void {
    emulator.update();
    emulator.draw();
    window.requestAnimationFrame(gameLoop);
}

window.onload = function() {
    document.addEventListener("keydown", keyDown);
    document.addEventListener("keyup", keyUp);
    window.requestAnimationFrame(gameLoop);
}
