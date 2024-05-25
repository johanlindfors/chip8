package se.programmeramera.chip8;
import java.nio.file.Files;
import java.nio.file.Paths;

import javax.swing.JFrame;
import javax.swing.JPanel;

public class Emulator {

    private JPanel display;
    private Thread gameLoop;
    private boolean isRunning;
    private CPU cpu;

    public Emulator(String filename) {
        createAndShowUI(filename);
    }

    public static void main(String[] args) {
        String filename = args[0];
        new Emulator(filename);
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
        Keyboard keyboard = new KeyboardDevice();
        keyboard.addKeyBindings(display);

        //this.addKeyBindings();
        this.setupGameLoop();

        frame.add(display);
        frame.pack();
        frame.setVisible(true);

        try {
            Memory memory = new Memory();
            byte[] rom = Files.readAllBytes(Paths.get(filename));
            memory.loadData(rom);
            this.cpu = new CPU(display, keyboard, memory);

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
                this.cpu.tick();
                this.display.repaint();
                try {
                    Thread.sleep(15);
                } catch (InterruptedException ex) {
                }
            }
        });
    }
}
