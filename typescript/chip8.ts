// variables
const SPRITE_SIZE = 10
const FPS = 60;

class Emulator {
    ctx: CanvasRenderingContext2D;
    width: number;
    height: number;
        
    constructor() {
        let surface = <HTMLCanvasElement>document.getElementById("surface");
        this.ctx = surface.getContext("2d");
        this.width = surface.clientWidth;
        this.height = surface.clientHeight;
    }

    clearScreen() : void {
        this.ctx.fillStyle = "black";
        this.ctx.fillRect(0, 0, this.width, this.height);
    }

    handleInput(evt: KeyboardEvent) {
        switch(evt.keyCode){
            case 37:
                break;
            case 39:
                break;
        }
    }

    update() : void {

    }

    draw() : void {
        this.clearScreen();
    }
}

let emulator = new Emulator();

function keyDown(evt) : void {
    emulator.handleInput(evt);
}

function gameLoop() : void {
    emulator.update();
    emulator.draw();
}

window.onload = function() {
    document.addEventListener("keydown", keyDown);
    setInterval(gameLoop, 1000 / FPS);
}