// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// The implementation of a screen component.
/// </summary>
public class Screen : DrawableGameComponent, Chip8.IScreen
{
    private readonly int width;
    private readonly int height;
    private readonly int scale;
    private readonly uint[] pixels;
    private Texture2D canvas;
    private Rectangle tracedSize;
    private SpriteBatch spriteBatch;
    private RenderTarget2D target;
    private bool drawFlag;

    /// <summary>
    /// Initializes a new instance of the <see cref="Screen"/> class.
    /// </summary>
    /// <param name="game">Instance of the game.</param>
    /// <param name="width">Width of screen.</param>
    /// <param name="height">Height of screen.</param>
    /// <param name="scale">Scale of screen.</param>
    public Screen(Game game, int width, int height, int scale)
        : base(game)
    {
        this.width = width;
        this.height = height;
        this.scale = scale;
        this.pixels = new uint[this.width * this.height];
        this.Clear();

        this.tracedSize = new Rectangle(0, 0, width, height);
        this.canvas = new Texture2D(this.GraphicsDevice, width, height, false, SurfaceFormat.Color);

        this.target = new RenderTarget2D(this.GraphicsDevice, width, height);
        this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
    }

    /// <summary>
    /// Gets the width of the screen.
    /// </summary>
    public int Width => this.width;

    /// <summary>
    /// Gets the height of the screen.
    /// </summary>
    public int Height => this.height;

    /// <summary>
    /// Clears the screen.
    /// </summary>
    public void Clear()
    {
        var length = this.height * this.width;
        for (int i = 0; i < length; i++)
        {
            this.pixels[i] = 0;
        }
    }

    /// <summary>
    /// The update of the rendering loop.
    /// </summary>
    /// <param name="gameTime">Time since last update.</param>
    public override void Update(GameTime gameTime)
    {
        this.canvas.SetData<uint>(this.pixels, 0, this.tracedSize.Width * this.tracedSize.Height);

        base.Update(gameTime);
    }

    /// <summary>
    /// Actual rendering.
    /// </summary>
    /// <param name="gameTime">Time since last render.</param>
    public override void Draw(GameTime gameTime)
    {
        if (this.drawFlag)
        {
            this.GraphicsDevice.SetRenderTarget(this.target);
            this.GraphicsDevice.Clear(Color.Black);

            this.spriteBatch.Begin();
            this.spriteBatch.Draw(this.canvas, new Rectangle(0, 0, this.tracedSize.Width, this.tracedSize.Height), Color.White);
            this.spriteBatch.End();

            this.drawFlag = false;
        }

        this.GraphicsDevice.SetRenderTarget(null);
        this.GraphicsDevice.Clear(Color.Black);

        this.spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        this.spriteBatch.Draw(this.target, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);
        this.spriteBatch.End();

        base.Draw(gameTime);
    }

    /// <summary>
    /// Sets the pixel at provided coordinates.
    /// </summary>
    /// <param name="xCoord">X-value.</param>
    /// <param name="yCoord">Y-value.</param>
    public void SetPixel(int xCoord, int yCoord)
    {
        this.pixels[(yCoord * this.width) + xCoord] ^= 0xFFFFFFFF;
    }

    /// <summary>
    /// Gets the pixel at provided coordinates.
    /// </summary>
    /// <param name="xCoord">X-value.</param>
    /// <param name="yCoord">Y-value.</param>
    /// <returns>The value of the pixel.</returns>
    public uint GetPixel(int xCoord, int yCoord)
    {
        return this.pixels[(yCoord * this.width) + xCoord];
    }

    /// <summary>
    /// Sets the draw flag.
    /// </summary>
    public void SetDrawFlag()
    {
        this.drawFlag = true;
    }
}
