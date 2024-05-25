package se.programmeramera.chip8;

import javax.swing.JPanel;
import java.awt.Graphics;
import java.awt.Image;
import java.awt.Dimension;

public class DisplayDevice extends JPanel implements Display {
    private int screenWidth = 64;
    private int screenHeight = 32;
    private int scale = 10;
    private boolean drawFlag;
    boolean[] pixels;
    Image offScreenImage;
    Graphics offScreenGraphics;

    public DisplayDevice() {
        this.setIgnoreRepaint(true);
        this.clear();
        this.drawFlag = false;
    }

    @Override
    protected void paintComponent(Graphics g) {
        super.paintComponent(g);

        if(this.offScreenImage == null){
            this.offScreenImage = createImage(screenWidth * scale, screenHeight * scale);
            this.offScreenGraphics = this.offScreenImage.getGraphics();    
        }

        if(this.drawFlag) {
            if(this.offScreenGraphics != null)
                this.offScreenGraphics.clearRect(0, 0, screenWidth * scale, screenHeight * scale);
            for (int index = 0; index < pixels.length; index++) {
                if (this.pixels[index]) {
                    int x = index % this.screenWidth;
                    int y = Math.floorDiv(index, this.screenWidth);
                    offScreenGraphics.fillRect(x * scale, y * scale, scale, scale);
                }
            }
            this.drawFlag = false;
        }
        g.drawImage(offScreenImage, 0, 0, this);
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
        if(this.offScreenGraphics != null)
            this.offScreenGraphics.clearRect(0, 0, screenWidth * scale, screenHeight * scale);
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
