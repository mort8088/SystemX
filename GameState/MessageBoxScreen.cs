// -----------------------------------------------------------------------
// <copyright file="MessageBoxScreen.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using SystemX.Extensions;
using SystemX.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.GameState
{
    public class MessageBoxScreen : GameScreen
    {
        private const string _usageText = "Enter = OK Esc = cancel";
        private readonly string _message;
        private readonly bool _includeUsageText;

        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;

        public string SpritePage = "Main";
        public string SpriteName = "gradient";
        public string Font = "System";
        public int LeftPadding
        {
            get { return this._padding.X; }
            set { this._padding.X = value; }
        }
        public int RightPadding
        {
            get { return this._padding.Width; }
            set { this._padding.Width = value; }
        }
        public int TopPadding
        {
            get { return this._padding.Y; }
            set { this._padding.Y = value; }
        }
        public int BottomPadding
        {
            get { return this._padding.Height; }
            set { this._padding.Height = value; }
        }
        public int HorizontalPadding
        {
            get { return this._padding.X; }
            set { this._padding.X = value; this._padding.Width = value; }
        }
        public int VerticalPadding
        {
            get { return this._padding.Y; }
            set { this._padding.Y = value; this._padding.Height = value; }
        }
        public int Padding
        {
            get { return this._padding.X; }
            set { this._padding.X = value; this._padding.Width = value; this._padding.Y = value; this._padding.Height = value; }
        }
        
        private Rectangle _padding = new Rectangle(32, 16, 32, 16);

        /// <summary>
        ///     Constructor automatically includes the standard usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message) : this(message, true) { }

        /// <summary>
        ///     Constructor lets the caller specify whether to include the standard usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message, bool includeUsageText)
        {
            this._includeUsageText = includeUsageText;
            this._message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }

        /// <summary>
        ///     Responds to user input, accepting or canceling the message box.
        /// </summary>
        public override void HandleInput(InputManager input)
        {
            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Canceled events, so they can tell which player triggered them.
            if (input.IsMenuSelect())
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, new EventArgs());

                ExitScreen();
            }
            else if (input.IsMenuCancel())
            {
                // Raise the canceled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, new EventArgs());

                ExitScreen();
            }
        }

        /// <summary>
        ///     Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatchExtended spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font[this.Font];

            // Darken down any other screens that were drawn beneath the pop-up.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);

            Vector2 textSize = font.MeasureString(this._message);

            Vector2 textPosition = (viewportSize - textSize) / 2;

            Vector2 UsagePosition = Vector2.Zero;

            // The background includes a border somewhat larger than the text itself.
            Rectangle backgroundRectangle = new Rectangle(
                (int)textPosition.X - this.LeftPadding,
                (int)textPosition.Y - this.TopPadding,
                (int)textSize.X + (this.LeftPadding + this.RightPadding),
                (int)textSize.Y + (this.TopPadding + this.BottomPadding));

            // Fade the pop-up alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(
                ScreenManager.SpriteSheets[SpritePage].Page,
                backgroundRectangle,
                ScreenManager.SpriteSheets[SpritePage][SpriteName].Source,
                color);

            // Draw the message box text.
            spriteBatch.GlifString(font, this._message, textPosition, color);

            if (_includeUsageText)
            {
                textSize = font.MeasureString(_usageText);
                textPosition = new Vector2(
                    viewport.TitleSafeArea.Right - (textSize.X + this.RightPadding),
                    viewport.TitleSafeArea.Bottom - (textSize.Y + this.BottomPadding));

                spriteBatch.GlifString(font, _usageText, textPosition, color);
            }

            spriteBatch.End();
        }
    }
}