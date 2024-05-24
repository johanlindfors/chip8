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
        
    constructor() {
        let surface = <HTMLCanvasElement>document.getElementById("surface");
        let input = <HTMLCanvasElement>document.getElementById("selectRom");
        input.addEventListener("change", (e) => {
            this.loadRom(e);
        });

        this.ctx = surface.getContext("2d")!;
        this.width = surface.clientWidth;
        this.height = surface.clientHeight;
        this.keyboard = new Keyboard();
        this.display = new Display(64, 32);
        let audio = new AudioPlayer();
        this.cpu = new CPU(this.keyboard, this.display, audio);
    }

    loadRom = async (event) => {
        let fileReader = new FileReader();
        fileReader.readAsArrayBuffer(event.target.files[0]);

        fileReader.onload = () => {
            let rom = new Uint8Array(fileReader.result as ArrayBuffer);
            let memory = new Memory();
            memory.loadProgram(0x0200, rom);
            this.cpu.attachMemory(memory);
        };

        fileReader.onerror = (error) => {
            console.error(error);
        };;
    };

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
        this.display.draw(this.ctx);
    }
}

let emulator = new Emulator();

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
