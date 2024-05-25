package se.programmeramera.chip8;

interface RegisterFunction{
    byte run(byte input);
}
  
public class Registers {
    private byte[] registers;

    public Registers() {
        super();

        this.registers = new byte[16];
    }

    public byte get(int register) {
        return this.registers[register];
    }

    public void set(int register, byte value) {
        this.registers[register] = value;
    }

    public void apply(int index, RegisterFunction func) {
        this.registers[index] = func.run(this.registers[index]);
    }
}
