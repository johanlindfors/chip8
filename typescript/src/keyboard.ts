interface IKeyboard {
    onKeyPressed(keyCode : string) : void;
    onKeyReleased(keyCode : string) : void;
    isKeyPressed(index : number) : boolean;
    getAllKeys() : boolean[];
}

class Keyboard implements IKeyboard {
    private _keys : boolean[];

    constructor() {
        this._keys = new Array(16).fill(false);
    }

    onKeyPressed(keyCode: string): void {
        this.setKeyState(keyCode, true);
    }

    onKeyReleased(keyCode: string): void {
        this.setKeyState(keyCode, false);
    }

    isKeyPressed(index: number): boolean {
        return this._keys[index];
    }

    getAllKeys(): boolean[] {
        return this._keys;
    }

    private setKeyState(keyCode: string, value : boolean) : void{
        switch(keyCode) {
            case "Digit1":
                this._keys[0x1] = value;
                break;
            case "Digit2":
                this._keys[0x2] = value;
                break;
            case "Digit3":
                this._keys[0x3] = value;
                break;
            case "Digit4":
                this._keys[0xC] = value;
                break;

            case "KeyQ":
                this._keys[0x4] = value;
                break;
            case "KeyW":
                this._keys[0x5] = value;
                break;
            case "KeyE":
                this._keys[0x6] = value;
                break;
            case "KeyR":
                this._keys[0xD] = value;
                break;
    
            case "KeyA":
                this._keys[0x7] = value;
                break;
            case "KeyS":
                this._keys[0x8] = value;
                break;
            case "KeyD":
                this._keys[0x9] = value;
                break;
            case "KeyF":
                this._keys[0xE] = value;
                break;
    
            case "KeyZ":
                this._keys[0xA] = value;
                break;
            case "KeyX":
                this._keys[0x0] = value;
                break;
            case "KeyC":
                this._keys[0xB] = value;
                break;
            case "KeyV":
                this._keys[0xF] = value;
                break;
            default:
                break;
        }
    }
}
