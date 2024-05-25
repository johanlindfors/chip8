package se.programmeramera.chip8;

public class KeyboardDevice implements Keyboard {
    private byte[] keys;

    public KeyboardDevice() {
        this.keys = new byte[16];
    }

    public boolean isKeyPressed(byte key) {
        return false;
    }

    public byte[] getKeys() {
        return this.keys;
    }
}
