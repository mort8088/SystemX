// -----------------------------------------------------------------------
// <copyright file="Animation.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

namespace SystemX.Animation {
    /// <summary>
    ///     Represents an animated texture.
    /// </summary>
    /// <remarks>
    ///     Currently, this class assumes that each frame of animation is
    ///     as wide as each animation is tall. The number of frames in the
    ///     animation are inferred from this.
    /// </remarks>
    public class Animation {
        /// <summary>
        ///     Constructors a new animation.
        /// </summary>
        /// <param name="texture">SpriteSheet Reference name with from index as {0:00}.</param>
        /// <param name="frameCount">Gets the number of frames in the animation.</param>
        /// <param name="frameWidth">Gets the width of a frame in the animation.</param>
        /// <param name="frameHeight">Gets the height of a frame in the animation.</param>
        /// <param name="frameTime">Duration of time to show each frame.</param>
        /// <param name="isLooping">When the end of the animation is reached, should it continue playing from the beginning?</param>
        public Animation(string sheetRef, string texture, int frameCount, int frameWidth, int frameHeight, float frameTime, bool isLooping) {
            SheetRef = sheetRef;
            Texture = texture;
            FrameTime = frameTime;
            IsLooping = isLooping;
            FrameCount = frameCount;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
        }

        /// <summary>
        ///     Reference to which sprite sheet to use;
        /// </summary>
        public string SheetRef { get; set; }

        /// <summary>
        ///     Search string for frames in the animation e.g. Texture{0:00} where {0:00} is the frame index.
        /// </summary>
        public string Texture { get; private set; }

        /// <summary>
        ///     Duration of time to show each frame.
        /// </summary>
        public float FrameTime { get; private set; }

        /// <summary>
        ///     When the end of the animation is reached, should it
        ///     continue playing from the beginning?
        /// </summary>
        public bool IsLooping { get; private set; }

        /// <summary>
        ///     Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        ///     Gets the width of a frame in the animation.
        /// </summary>
        public int FrameWidth { get; private set; }

        /// <summary>
        ///     Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight { get; private set; }
    }
}