using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.GUI.Controls
{
    public class ProgressBar
    {
        private Texture2D _base, _top;
        public Color BaseColor { get; set; }
        public Color TopColor { get; set; }

        public void LoadGraphics(GraphicsDevice device)
        {
            _base = new Texture2D(device, 1, 1);
            _base.SetData(new[] {
                                    BaseColor
                                });

            _top = new Texture2D(device, 1, 1);
            _top.SetData(new[] {
                                   TopColor
                               });
        }

        public void Unload()
        {
            if (_base != null)
                _base.Dispose();
            if (_top != null)
                _top.Dispose();
        }

        public void Draw(SpriteBatch sb, float progress, Rectangle rect)
        {
            if (_base != null &&
                _top != null)
            {
                sb.Draw(_base, rect, Color.White);
                rect.Width = (int)(rect.Width * progress);
                sb.Draw(_top, rect, Color.White);
            }
        }
    }
}