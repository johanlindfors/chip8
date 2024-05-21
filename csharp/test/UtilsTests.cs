// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Chip8.Tests;

/// <summary>
/// Tests for utilities.
/// </summary>
public class UtilsTest
{
    [Fact]
    private void RegisterApplyTest()
    {
        var register = new Chip8.Register();
        register[2] = 4;
        int x = 0;
        int y = 2;
        register.Apply(x, vx => (byte)(vx | register[y]));

        Assert.Equal(4, register[0]);
    }
}
