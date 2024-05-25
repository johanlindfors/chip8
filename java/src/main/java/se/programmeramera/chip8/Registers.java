package se.programmeramera.chip8;

interface RegisterFunction{
    Integer run(Integer input);
}
  
public class Registers {
    private Integer[] registers;

    public Registers() {
        super();

        this.registers = new Integer[16];
    }

    public Integer get(int register) {
        return this.registers[register];
    }

    public void set(int register, Integer value) {
        this.registers[register] = value;
    }

    public void apply(int index, RegisterFunction func) {
        this.registers[index] = func.run(this.registers[index]);
    }
}
