package se.programmeramera.chip8;

import java.awt.event.ActionEvent;
import java.awt.event.KeyEvent;

import javax.swing.AbstractAction;
import javax.swing.JComponent;
import javax.swing.JPanel;
import javax.swing.KeyStroke;

public class KeyboardDevice implements Keyboard {
    private boolean[] keys;

    public KeyboardDevice() {
        this.keys = new boolean[16];
    }

    public void addKeyBindings(JPanel panel) {
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_1, 0, false), "1 pressed");
        panel.getActionMap().put("1 pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x1] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_1, 0, true), "1 released");
        panel.getActionMap().put("1 released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x1] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_2, 0, false), "2 pressed");
        panel.getActionMap().put("2 pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x2] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_2, 0, true), "2 released");
        panel.getActionMap().put("2 released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x2] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_3, 0, false), "3 pressed");
        panel.getActionMap().put("3 pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x3] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_3, 0, true), "3 released");
        panel.getActionMap().put("3 released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x3] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_4, 0, false), "4 pressed");
        panel.getActionMap().put("4 pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xC] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_4, 0, true), "4 released");
        panel.getActionMap().put("4 released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xC] = false;
            }
        });

        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_Q, 0, false), "Q pressed");
        panel.getActionMap().put("Q pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x4] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_Q, 0, true), "Q released");
        panel.getActionMap().put("Q released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x4] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_W, 0, false), "W pressed");
        panel.getActionMap().put("W pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x5] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_W, 0, true), "W released");
        panel.getActionMap().put("W released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x5] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_E, 0, false), "E pressed");
        panel.getActionMap().put("E pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x6] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_E, 0, true), "E released");
        panel.getActionMap().put("E released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x6] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_R, 0, false), "R pressed");
        panel.getActionMap().put("R pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xD] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_R, 0, true), "R released");
        panel.getActionMap().put("R released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xD] = false;
            }
        });

        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_A, 0, false), "A pressed");
        panel.getActionMap().put("A pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x7] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_A, 0, true), "A released");
        panel.getActionMap().put("A released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x7] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_S, 0, false), "S pressed");
        panel.getActionMap().put("S pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x8] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_S, 0, true), "S released");
        panel.getActionMap().put("S released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x8] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_D, 0, false), "D pressed");
        panel.getActionMap().put("D pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x9] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_D, 0, true), "D released");
        panel.getActionMap().put("D released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x9] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_F, 0, false), "F pressed");
        panel.getActionMap().put("F pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xE] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_F, 0, true), "F released");
        panel.getActionMap().put("F released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xE] = false;
            }
        });

        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_Z, 0, false), "Z pressed");
        panel.getActionMap().put("Z pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xA] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_Z, 0, true), "Z released");
        panel.getActionMap().put("Z released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xA] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_X, 0, false), "X pressed");
        panel.getActionMap().put("X pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x0] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_X, 0, true), "X released");
        panel.getActionMap().put("X released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0x0] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_C, 0, false), "C pressed");
        panel.getActionMap().put("C pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xB] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_C, 0, true), "C released");
        panel.getActionMap().put("C released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xB] = false;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_V, 0, false), "V pressed");
        panel.getActionMap().put("V pressed", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xF] = true;
            }
        });
        panel.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_V, 0, true), "V released");
        panel.getActionMap().put("V released", new AbstractAction() {
            @Override
            public void actionPerformed(ActionEvent e) {
                keys[0xF] = false;
            }
        });
    }

    public boolean isKeyPressed(Integer key) {
        return keys[key];
    }

    public boolean[] getKeys() {
        return this.keys;
    }
}
