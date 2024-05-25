package se.programmeramera.chip8;

public interface Keyboard {
    boolean isKeyPressed(byte key);

    byte[] getKeys();
}
