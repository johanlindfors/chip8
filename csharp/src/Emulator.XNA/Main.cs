// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8;

/// <summary>
/// The program.
/// </summary>
public class Program
{
    /// <summary>
    /// The main starting point of the application.
    /// </summary>
    /// <param name="args">Any eventual arguments.</param>
    /// <returns>Error code.</returns>
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            System.Console.WriteLine("You must supply a game argument, example: /roms/INVADERS.ch8");
            return 1;
        }
        else
        {
            return Utils.Load(args[0])
                .Map(bytes =>
                {
                    var emulator = new Chip8.Emulator(bytes);
                    emulator.Run();
                    return 0;
                })
                .Reduce(() => 1);
        }
    }
}
