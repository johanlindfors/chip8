// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8.Tests;

/// <summary>
/// Mock implementation of a random number generator.
/// </summary>
public class RandomNumberGeneratorMock()
    : IRandomNumberGenerator
{
    private Queue<byte> numbers = new Queue<byte>();

    /// <summary>
    /// Populate the queue.
    /// </summary>
    /// <param name="numbers">With these numbers.</param>
    public void PopulateRandomQueue(byte[] numbers)
    {
        foreach (var number in numbers)
        {
            this.numbers.Enqueue(number);
        }
    }

    /// <summary>
    /// Returns a value from the queue.
    /// </summary>
    /// <returns>A value from the queue.</returns>
    /// <exception cref="InvalidOperationException">Will be thrown if queue is empty.</exception>
    public byte NextByte()
    {
        if (this.numbers.Any())
        {
            return this.numbers.Dequeue();
        }

        throw new InvalidOperationException();
    }
}
