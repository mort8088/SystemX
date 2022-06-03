// -----------------------------------------------------------------------
// <copyright file="MenuEntry.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;

namespace SystemX.GameState
{
    public class MenuEntry
    {
        public float selectionFade;

        public MenuEntry(string text)
        {
            Text = text;
            TextColor = Color.White;
            HighlightColor = Color.Orange;
            BackgroundColor = Color.Black;
        }

        // public MenuEntry(string text, Vector2 position, Vector2 targetPosition, Rectangle hotbox, Color textColor, Color highlightColor, Color backgroundColor)
        // {
        //     this.Text = text;
        //     this.Position = position;
        //     this.TargetPosition = targetPosition;
        //     this.Hotbox = hotbox;
        //     this.TextColor = textColor;
        //     this.HighlightColor = highlightColor;
        //     this.BackgroundColor = backgroundColor;

        // }

        public string Text { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 TargetPosition { get; set; }
        public Rectangle Hotbox { get; set; }
        public Color TextColor { get; set; }
        public Color HighlightColor { get; set; }
        public Color BackgroundColor { get; set; }
        public event EventHandler<EventArgs> Selected;

        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }
    }
}