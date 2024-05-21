// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Input = Microsoft.Xna.Framework.Input;

/// <summary>
/// The emulator implementation.
/// </summary>
public class Emulator : Game
{
    private const int WIDTH = 64;
    private const int HEIGHT = 32;
    private const int SCALE = 10;
    private readonly IKeyboard keyboard;
    private readonly Memory memory;
    private GraphicsDeviceManager graphics;
    private CPU cpu;
    private Input.KeyboardState oldState;
    private double updateCounter = 0;
    private double drawCounter = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="Emulator"/> class.
    /// </summary>
    /// <param name="gameData">The data to run.</param>
    public Emulator(byte[] gameData)
    {
        this.graphics = new GraphicsDeviceManager(this)
        {
            IsFullScreen = false,
            PreferredBackBufferHeight = 320,
            PreferredBackBufferWidth = 640,
            SynchronizeWithVerticalRetrace = false,
        };
        this.IsMouseVisible = true;

        this.keyboard = new Keyboard();

        this.memory = new Memory();
        this.memory.LoadData(gameData);

        this.IsFixedTimeStep = false;

        this.graphics.ApplyChanges();
    }

    /// <summary>
    /// Initialization.
    /// </summary>
    protected override void Initialize()
    {
        var screen = new Screen(this, WIDTH, HEIGHT, SCALE);
        this.Components.Add(screen);

        var random = new RandomNumberGenerator();
        var register = new Register();
        this.cpu = new CPU(this.memory, register, random, this.keyboard, screen);

        this.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

        base.Initialize();
    }

    /// <summary>
    /// Update path of game loop.
    /// </summary>
    /// <param name="gameTime">Used for timing.</param>
    protected override void Update(GameTime gameTime)
    {
        if (Input.GamePad.GetState(PlayerIndex.One).Buttons.Back == Input.ButtonState.Pressed || Input.Keyboard.GetState().IsKeyDown(Input.Keys.Escape))
        {
            System.Console.WriteLine($"Updates: {this.updateCounter}");
            System.Console.WriteLine($"Draws: {this.drawCounter}");
            System.Console.WriteLine($"Instructions: {this.cpu.InstructionsCounter}");
            this.Exit();
        }

        this.HandleInput();

        this.cpu.EmulateCycle();
        base.Update(gameTime);
        this.updateCounter++;
    }

    /// <summary>
    /// Draw path of game loop.
    /// </summary>
    /// <param name="gameTime">Used for timing.</param>
    protected override void Draw(GameTime gameTime)
    {
        if (this.cpu.DrawFlag)
        {
            base.Draw(gameTime);
            this.cpu.DrawFlag = false;
        }

        this.drawCounter++;
    }

    private void HandleInput()
    {
        var state = Input.Keyboard.GetState();
        foreach (var key in this.oldState.GetPressedKeys())
        {
            this.TestKey(key, this.oldState, state);
        }

        this.oldState = state;
    }

    private void TestKey(Input.Keys key, Input.KeyboardState oldState, Input.KeyboardState newState)
    {
        if (newState.IsKeyDown(key))
        {
            this.keyboard.OnKeyPressed((int)key);
        }
        else if (oldState.IsKeyDown(key) && newState.IsKeyUp(key))
        {
            this.keyboard.OnKeyReleased((int)key);
        }
    }
}
