// -----------------------------------------------------------------------
// <copyright file="MenuScreen.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SystemX.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.GameState
{
    public abstract class MenuScreen : GameScreen
    {
        private const float menuSpeed = 3f;
        private readonly List<MenuEntry> menuEntries = new List<MenuEntry>();
        protected SpriteFont font;
        protected Rectangle HighlightBox;
        protected string menuTitle = "Main Menu";
        private int selectedEntry;
        protected Vector2 TitlePosition = Vector2.Zero;
        protected Color TitleColor = Color.Red;

        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }
        protected IList<MenuEntry> MenuEntries
        {
            get
            {
                return menuEntries;
            }
        }
        public override void LoadContent()
        {
            font = ScreenManager.Font["menufont"];

            base.LoadContent();
        }
        protected virtual void OnSelectEntry(int entryIndex)
        {
            menuEntries[entryIndex].OnSelectEntry();
        }
        protected virtual void OnCancel()
        {
            ExitScreen();
        }
        protected virtual void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }
        public override void HandleInput(InputManager input)
        {
            if (ScreenManager.Settings["Misc"]["MouseEnabled"].GetValueAsBool())
            {
                for (int i = 0; i < menuEntries.Count; i++)
                {
                    MenuEntry menuItem = menuEntries[i];

                    if (menuItem.Hotbox.Contains(input.GetMouseCoordinates()))
                    {
                        selectedEntry = i;
                        if (input.IsMouseLeftClick()) OnSelectEntry(selectedEntry);
                        break;
                    }
                }
            }

            if (input.IsMenuUp())
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }
            else if (input.IsMenuDown())
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            if (input.IsMenuSelect()) OnSelectEntry(selectedEntry);
            else if (input.IsMenuCancel()) OnCancel();
        }
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            // GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Rectangle TitleSafeArea = graphics.Viewport.TitleSafeArea;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                // Draw the selected entry in yellow, otherwise white.
                Color color = isSelected ? menuEntry.TextColor : menuEntry.HighlightColor;

                float scale = 1f + (0.1f * menuEntry.selectionFade);

                // Modify the alpha to fade text out during transitions.
                color *= TransitionAlpha;

                // Draw text, centered on the middle of each line.
                Vector2 origin = new Vector2(0, font.LineSpacing / 2);
                if (isSelected)
                {
                    // Vector2 margin = font.MeasureString("*") * scale;
                    // Vector2 MeasureString = font.MeasureString(menuEntry.Text);
                    Rectangle TargetBlock = menuEntry.Hotbox;

                    if (HighlightBox != TargetBlock)
                    {
                        HighlightBox.X += (int)MathHelper.Clamp(TargetBlock.X - HighlightBox.X, -menuSpeed, menuSpeed);
                        HighlightBox.Y += (int)MathHelper.Clamp(TargetBlock.Y - HighlightBox.Y, -menuSpeed, menuSpeed);
                        HighlightBox.Width += (int)(MathHelper.Clamp(TargetBlock.Width - HighlightBox.Width, -menuSpeed * 5, menuSpeed * 5) * scale);
                        HighlightBox.Height += (int)(MathHelper.Clamp(TargetBlock.Height - HighlightBox.Height, -menuSpeed * 5, menuSpeed * 5) * scale);
                    }

                    spriteBatch.Draw(ScreenManager.BlankTexture2D, HighlightBox, menuEntry.BackgroundColor);
                }

                spriteBatch.DrawString(font, menuEntry.Text, menuEntry.Position, color, 0, origin, scale, SpriteEffects.None, 0);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titleOrigin = Vector2.Zero; // font.MeasureString(this.menuTitle) / 2;
            Vector2 titlePosition = TitlePosition;
            Color titleColor = TitleColor * TransitionAlpha;
            float titleScale = 2f * TransitionAlpha;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }
        protected virtual void UpdateMenuEntryLocations()
        {
            const float overlap = 0.5f;
            int numEntries = menuEntries.Count;

            for (int i = 0; i < menuEntries.Count; i++)
            {
                float transitionOffset = MathHelper.Clamp((TransitionPosition - (1 - overlap) * i / numEntries) / overlap, 0, 1);

                MenuEntry menuEntry = menuEntries[i];
                Vector2 position = menuEntry.TargetPosition;

                position.X = MathHelper.SmoothStep(menuEntry.TargetPosition.X, -200, transitionOffset);

                // scale += MathHelper.SmoothStep(0, 4, transitionOffset);
                // color = new Color(color.R, color.G, color.B, (byte)((1 - transitionOffset) * 255));

                menuEntry.Position = position;

                // position.Y += font.MeasureString(menuEntry.Text).Y / 3;
            }
        }
    }
}