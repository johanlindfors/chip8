using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Input = Microsoft.Xna.Framework.Input;

namespace Chip8;

public class Emulator : Game
{
    private const int WIDTH = 64;
    private const int HEIGHT = 32;

    private const int SCALE = 10;
    private GraphicsDeviceManager graphics;
 
    private Chip8.CPU cpu;
    private Screen screen;
    private readonly Chip8.IKeyboard keyboard;
    private readonly Chip8.Memory memory;
    private Input.KeyboardState oldState;
    private double updateCounter = 0, drawCounter = 0;
    public Emulator(byte[] gameData)
    {
        graphics = new GraphicsDeviceManager(this);
        graphics.IsFullScreen = false;
        graphics.PreferredBackBufferHeight = 320;
        graphics.PreferredBackBufferWidth = 640;
        graphics.SynchronizeWithVerticalRetrace = false;
        IsMouseVisible = true;

        this.keyboard = new Chip8.Keyboard();
        
        memory = new Chip8.Memory();
        memory.LoadData(gameData);

        this.IsFixedTimeStep = false;

        graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        screen = new Screen(this, WIDTH, HEIGHT, SCALE);
        this.Components.Add(screen);
        
        var random = new System.Random();
        var registers = new Chip8.Registers();
        this.cpu = new Chip8.CPU(memory, registers, random, keyboard, screen);

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        if (Input.GamePad.GetState(PlayerIndex.One).Buttons.Back == Input.ButtonState.Pressed || Input.Keyboard.GetState().IsKeyDown(Input.Keys.Escape)) {
            System.Console.WriteLine($"Updates: {updateCounter}");
            System.Console.WriteLine($"Draws: {drawCounter}");
            System.Console.WriteLine($"Instructions: {cpu.InstructionsCounter}");
            Exit();
        }

        HandleInput();

        cpu.EmulateCycle();

        if(this.cpu.DrawFlag) {    
            base.Update(gameTime);
            this.cpu.DrawFlag = false;
        }

        updateCounter++;
    }

    private void HandleInput() {
        var state = Input.Keyboard.GetState();
        if(state.GetPressedKeyCount() > 0) {
            foreach(var key in state.GetPressedKeys()) {
                TestKey(key, oldState, state);
            }
        } else if(state != oldState) {
            foreach(var key in oldState.GetPressedKeys()) {
                TestKey(key, oldState, state);
            }
        }
        oldState = state;
    }

    private void TestKey(Input.Keys key, Input.KeyboardState oldState, Input.KeyboardState newState) {
        if(newState.IsKeyDown(key) && oldState.IsKeyUp(key)) {
            keyboard.OnKeyPressed((int)key);
        } 
        if(oldState.IsKeyDown(key) && newState.IsKeyUp(key)) {
            keyboard.OnKeyReleased((int)key);
        }        
    }
}
