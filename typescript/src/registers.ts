class Registers {
    private _registers : Uint8Array;

    constructor() {
        this._registers = new Uint8Array(16);
    }

    setRegister(index: number, data: number) : void {
        this._registers[index] = data & 0xFF;
    }

    getRegister(index: number) : number {
        return this._registers[index] & 0xFF;
    }

    apply(index: number, func: (data : number) => number) {
        this._registers[index] = func(this._registers[index]);
    }
}
