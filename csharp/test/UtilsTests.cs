namespace Chip8.Tests;

public class UtilsTest
{
    [Fact]
    public void RegisterApplyTest()
    {
        var registers = new Chip8.Registers();
        registers[2] = 4;
        int x = 0;
        int y = 2;
        registers.Apply(x, vx => (byte)(vx | registers[y]));

        Assert.Equal(4, registers[0]);
    }
}
