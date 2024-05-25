// Copyright (c) Coderox AB. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

/// <summary>
/// Represents an audio interface for controlling audio playback.
/// </summary>
public interface IAudio
{
    /// <summary>
    /// Gets a value indicating whether audio is currently playing.
    /// </summary>
    bool IsPlaying { get; }

    /// <summary>
    /// Starts audio playback.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops audio playback.
    /// </summary>
    void Stop();
}
