package se.programmeramera.chip8;

public interface Display {
    /// <summary>
    /// Gets the width of the screen.
    /// </summary>
    int getDisplayWidth();

    /// <summary>
    /// Gets the height of the screen.
    /// </summary>
    int getDisplayHeight();

    /// <summary>
    /// Clears the screen.
    /// </summary>
    void clear();

    /// <summary>
    /// Sets the pixel a the specified coordinate.
    /// </summary>
    /// <param name="xCoord">The x-coordinate.</param>
    /// <param name="yCoord">The y-coordinate.</param>
    void setPixel(int xCoord, int yCoord);

    /// <summary>
    /// Gets the pixel a the specified coordinate.
    /// </summary>
    /// <param name="xCoord">The x-coordinate.</param>
    /// <param name="yCoord">The y-coordinate.</param>
    /// <returns>The value at the coordinate.</returns>
    boolean getPixel(int xCoord, int yCoord);

    void setDrawFlag();
}
