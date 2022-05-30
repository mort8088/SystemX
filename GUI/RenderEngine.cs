using System.Collections.Generic;
using SystemX.Common;
using SystemX.Extensions;
using SystemX.GUI.Controls;
using SystemX.GUI.Visuals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.GUI
{
    public class RenderEngine
    {
        public readonly List<I_Visual> Visuals = new List<I_Visual>();
        private RasterizerState _rasterizerState;

        public void LoadGraphics(GameStateManager gsm)
        {
            foreach (I_Visual guiVisual in Visuals) guiVisual.LoadGraphics(gsm);

            _rasterizerState = new RasterizerState
            {
                ScissorTestEnable = true
            };
        }

        public void Update(float elapsedSeconds)
        {
            foreach (I_Visual visual in Visuals) visual.Update(elapsedSeconds);
        }

        private bool _draw;
        private Rectangle _currentRect;
        private bool _isChecked;
        public void Draw(GraphicsDevice device, SpriteBatchExtended spriteBatch)
        {
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            //Set up the spritebatch to draw using scissoring (for text cropping)
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _rasterizerState);

            //Copy the current scissor rectangle so we can restore it after
            _currentRect = spriteBatch.GraphicsDevice.ScissorRectangle;

            foreach (I_Visual visual in Visuals)
            {
                if (!visual.Owner.Visible)
                    continue;

                _draw = true;
                _isChecked = (((visual.Owner is CheckBox) && ((CheckBox)visual.Owner).Checked));

                // Determine if we need to draw this item
                switch (visual.Visibility)
                {
                    case Visibility.Enabled:
                        _draw = visual.Owner.Enabled;
                        break;
                    case Visibility.Disabled:
                        _draw = !visual.Owner.Enabled;
                        break;

                    case Visibility.Checked:
                        _draw = _isChecked;
                        break;

                    case Visibility.Unchecked:
                        _draw = !_isChecked;
                        break;
                }

                if (!_draw) continue;

                //Set the current scissor rectangle
                if (visual.Size == Point.Zero) spriteBatch.GraphicsDevice.ScissorRectangle = visual.Owner.Bounds;
                else
                {
                    spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(visual.Owner.Location.X + visual.Location.X,
                                                                                visual.Owner.Location.Y + visual.Location.Y,
                                                                                visual.Size.X,
                                                                                visual.Size.Y);
                }

                // Draw the visual
                visual.Draw(device, spriteBatch);
            }

            //Reset scissor rectangle to the saved value
            spriteBatch.GraphicsDevice.ScissorRectangle = _currentRect;

            //End the spritebatch
            spriteBatch.End();
        }
    }
}