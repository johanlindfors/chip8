package se.programmeramera.chip8;

import javax.swing.JPanel;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Dimension;

public class DisplayDevice extends JPanel implements Display {
    private int screenWidth = 64;
    private int screenHeight = 32;
    private int scale = 10;
    private boolean drawFlag;
    boolean[] pixels;

    public DisplayDevice() {
        this.setIgnoreRepaint(true);
        this.clear();
        this.drawFlag = false;
    }

    @Override
    protected void paintComponent(Graphics g) {
        //super.paintComponent(g);
        if(this.drawFlag) {
            System.out.println("Drawing screen...");
            for (int index = 0; index < pixels.length; index++) {
                if (this.pixels[index]) {
                    int x = index % this.screenWidth;
                    int y = Math.floorDiv(index, this.screenWidth);
                    g.fillRect(x * scale, y * scale, scale, scale);
                }
            }
            //this.drawFlag = false;
        }
    }

    @Override
    public Dimension getPreferredSize() {
        return new Dimension(screenWidth * scale, screenHeight * scale);
    }

    @Override
    public int getDisplayHeight() {
        return this.screenHeight;
    }

    @Override
    public int getDisplayWidth() {
        return this.screenWidth;
    }

    @Override
    public void clear() {
        this.pixels = new boolean[screenWidth * screenHeight];
    }

    @Override
    public void setPixel(int xCoord, int yCoord) {
        this.pixels[xCoord + yCoord * this.screenWidth] = !this.pixels[xCoord + yCoord * this.screenWidth];
    }

    @Override
    public boolean getPixel(int xCoord, int yCoord) {
        return this.pixels[xCoord + yCoord * screenWidth];
    }

    @Override
    public void setDrawFlag() {
        this.drawFlag = true;
    }
}
