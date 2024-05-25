package se.programmeramera.chip8;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Paths;

import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.SwingUtilities;

public class Emulator {

    private JPanel display;
    private Thread gameLoop;
    private boolean isRunning;
    private CPU cpu;

    public Emulator() {
        String filename = "./resources/roms/test/chip8-test-suite.ch8";
        createAndShowUI(filename);
    }

    public static void main(String[] args) {
        SwingUtilities.invokeLater(Emulator::new);
        // Emulator emulator = new Emulator(filename);
    }

    /**
     * Here we will create our swing UI as well as initialise and setup our
     * sprites, scene, and game loop and other buttons etc
     */
    private void createAndShowUI(String filename) {
        JFrame frame = new JFrame("Chip8 Emulator");
        frame.setTitle("Chip8 Emulator");
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        
        DisplayDevice display = new DisplayDevice();
        this.display = display;

        //this.addKeyBindings();
        this.setupGameLoop();

        frame.add(display);
        frame.pack();
        frame.setVisible(true);

        try {
            Memory memory = new Memory();
            byte[] rom = Files.readAllBytes(Paths.get(filename));
            memory.loadData(rom);
            this.cpu = new CPU(display, new KeyboardDevice(), memory);

            // after setting the frame visible we start the game loop, this could be done in a button or wherever you want
            this.isRunning = true;
            this.gameLoop.start();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    /**
     * This method would actually create and setup the game loop The game loop
     * will always be encapsulated in a Thread, Timer or some sort of construct
     * that generates a separate thread in order to not block the UI
     */
    private void setupGameLoop() {
        // initialise the thread 
        gameLoop = new Thread(() -> {
            // while the game "is running" and the isRunning boolean is set to true, loop forever
            while (isRunning) {
                // here we do 2 very important things which might later be expanded to 3:
                // 1. We call Scene#update: this essentially will iterate all of our Sprites in our game and update their movments/position in the game via Sprite#update()
                //this.scene.update();
                this.cpu.tick();

                // TODO later on one might add a method like this.scene.checkCollisions in which you check if 2 sprites are interesecting and do something about it
                // 2. We then call JPanel#repaint() which will cause JPanel#paintComponent to be called and thus we will iterate all of our sprites
                // and invoke the Sprite#render method which will draw them to the screen
                this.display.repaint();

                // here we throttle our game loop, because we are using a while loop this will execute as fast as it possible can, which might not be needed
                // so here we call Thread#slepp so we can give the CPU some time to breathe :)
                try {
                    Thread.sleep(15);
                } catch (InterruptedException ex) {
                }
            }
        });
    }

    private void addKeyBindings() {
        // here we would use KeyBindings (https://docs.oracle.com/javase/tutorial/uiswing/misc/keybinding.html) and add them to our Scene/JPanel
        // these would allow us to manipulate our Sprite objects using the keyboard below is 2 examples for using the A key to make our player/Sprite go left
        // or the D key to make the player/Sprite go to the right
        // this.scene.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_A, 0, false), "A pressed");
        // this.scene.getActionMap().put("A pressed", new AbstractAction() {
        //     @Override
        //     public void actionPerformed(ActionEvent e) {
        //         player.LEFT = true;
        //     }
        // });
        // this.scene.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_A, 0, true), "A released");
        // this.scene.getActionMap().put("A released", new AbstractAction() {
        //     @Override
        //     public void actionPerformed(ActionEvent e) {
        //         player.LEFT = false;
        //     }
        // });
        // this.scene.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_D, 0, false), "D pressed");
        // this.scene.getActionMap().put("D pressed", new AbstractAction() {
        //     @Override
        //     public void actionPerformed(ActionEvent e) {
        //         player.RIGHT = true;
        //     }
        // });
        // this.scene.getInputMap(JComponent.WHEN_IN_FOCUSED_WINDOW).put(KeyStroke.getKeyStroke(KeyEvent.VK_D, 0, true), "D released");
        // this.scene.getActionMap().put("D released", new AbstractAction() {
        //     @Override
        //     public void actionPerformed(ActionEvent e) {
        //         player.RIGHT = false;
        //     }
        // });
    }
}
