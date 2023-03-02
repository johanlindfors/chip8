package chip8

type Memory struct {
	ram		[4096]byte
}

func (c *Memory) LoadData(addr uint16, data []byte) {
	for i := 0; i < len(data); i++ {
		c.ram[int(addr) + i] = data[i]
	} 
}

func (c *Memory) get(addr uint16) byte {
	return c.ram[addr]
}

func (c *Memory) set(addr uint16, value byte) {
	c.ram[addr] = value
}
