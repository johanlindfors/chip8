package se.programmeramera.chip8;

import javax.swing.JPanel;

public interface Keyboard {
    void addKeyBindings(JPanel panel);

    boolean isKeyPressed(Integer key);

    boolean[] getKeys();
}
