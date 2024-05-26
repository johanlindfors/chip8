interface IDisplay {
    clear() : void;
    get Width() : number;
    get Height() : number;
    getPixel(xCoord : number, yCoord : number) : boolean;
    setPixel(xCoord : number, yCoord : number) : void;
    setDrawFlag() : void;

    draw(ctx : CanvasRenderingContext2D) : void;
}

class Display implements IDisplay {
    _pixels : boolean[];
    _width : number;
    _height : number;
    _drawFlag : boolean;

    constructor(width : number, height : number) {
        this._width = width;
        this._height = height;
        this._pixels = new Array(width * height).fill(false);
    }

    get Width(): number {
        return this._width;
    }

    get Height(): number {
        return this._height;
    }
    
    getPixel(xCoord: number, yCoord: number): boolean {
        return this._pixels[(yCoord * this.Width) + xCoord];
    }
    
    setPixel(xCoord: number, yCoord: number): void {
        this._pixels[(yCoord * this.Width) + xCoord] = !this._pixels[(yCoord * this.Width) + xCoord];
    }

    clear() : void {
        this._pixels = new Array(this.Width * this.Height).fill(false);
    }

    setDrawFlag(): void {
        this._drawFlag = true;
    }

    draw(ctx : CanvasRenderingContext2D) : void {
        if(this._drawFlag) {
            ctx.fillStyle = "black";
            ctx.clearRect(0, 0, this.Width * 10, this.Height * 10);
            ctx.fillStyle = "black";
            for (let index = 0; index < this._pixels.length; index++) {
                if(this._pixels[index]) {
                    let x = (index % this.Width) * 10;
                    let y = Math.floor(index / this.Width) * 10;
                    ctx.fillRect(x, y, 10, 10);
                }
            }
            this._drawFlag = false;
        }
    }
}
