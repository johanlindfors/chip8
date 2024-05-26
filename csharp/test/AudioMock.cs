// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

/// <summary>
/// Represents a mock implementation of the <see cref="IAudio"/> interface.
/// </summary>
internal class AudioMock : IAudio
{
    /// <summary>
    /// Gets a value indicating whether audio is currently playing.
    /// </summary>
    public bool IsPlaying { get; private set; }

    /// <summary>
    /// Starts the audio playback.
    /// </summary>
    public void Start()
    {
        this.IsPlaying = true;
    }

    /// <summary>
    /// Stops the audio playback.
    /// </summary>
    public void Stop()
    {
        this.IsPlaying = false;
    }
}
