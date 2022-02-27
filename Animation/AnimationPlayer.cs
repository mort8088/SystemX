// -----------------------------------------------------------------------
// <copyright file="AnimationPlayer.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using SystemX.Extensions;
using SystemX.SpriteSheets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.Animation
{
    /// <summary>
    ///     Controls playback of an Animation.
    /// </summary>
    public struct AnimationPlayer
    {
        /// <summary>
        ///     The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private float _time;

        /// <summary>
        ///     Gets the animation which is currently playing.
        /// </summary>
        public Animation Anim { get; private set; }

        /// <summary>
        ///     Gets the index of the current frame in the animation.
        /// </summary>
        public int FrameIndex { get; private set; }

        /// <summary>
        ///     Gets a texture origin at the bottom centre of each frame.
        /// </summary>
        public Vector2 Origin
        {
            get
            {
                return new Vector2(Anim.FrameWidth / 2.0f, Anim.FrameHeight / 2.0f);
            }
        }

        /// <summary>
        ///     Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(Animation animation)
        {
            // If this animation is already running, do not restart it.
            if (Anim == animation)
                return;

            // Start the new animation.
            Anim = animation;
            FrameIndex = 1;
            _time = 0.0f;
        }

        public void Update(GameTime gameTime)
        {
            Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void Update(float totalSeconds)
        {
            if (Anim == null)
                return;//throw new NotSupportedException("No animation is currently playing.");

            // Process passing time.
            _time += totalSeconds;
            while (_time > Anim.FrameTime)
            {
                _time -= Anim.FrameTime;

                // Advance the frame index; looping or clamping as appropriate.
                if (Anim.IsLooping)
                {
                    FrameIndex = ((FrameIndex + 1) % (Anim.FrameCount + 1));
                    if (FrameIndex == 0) FrameIndex = 1;
                }
                else FrameIndex = Math.Min(FrameIndex + 1, Anim.FrameCount);
            }
        }

        /// <summary>
        ///     Advances the time position and draws the current frame of the animation.
        /// </summary>
        public void Draw(SpriteBatchExtended spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (Anim == null)
                return;//throw new NotSupportedException("No animation is currently playing.");

            // Draw the current frame.
            spriteBatch.Draw(AnimationFactory.SpriteSheetLibrary[Anim.SheetRef].Page,
                position,
                AnimationFactory.SpriteSheetLibrary[Anim.SheetRef][string.Format(Anim.Texture, FrameIndex)].Source,
                Color.White,
                0.0f,
                Origin,
                1.0f,
                spriteEffects,
                0.0f);
        }
    }
}