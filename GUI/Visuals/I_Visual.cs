using System.Xml;
using SystemX.Common;
using SystemX.Extensions;
using SystemX.GUI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.GUI.Visuals {
    public interface I_Visual {
        string KeyRef { get; }

        [ContentSerializerIgnore]
        I_Control Owner { get; set; }

        string Name { get; set; }
        Point Location { get; set; }
        Point Size { get; set; }
        HorizontalAlignment HorizontalAlignment { get; set; }
        VerticalAlignment VerticalAlignment { get; set; }
        Visibility Visibility { get; set; }
        void LoadGraphics(GameStateManager gsm);
        void UnloadGraphics();
        void Update(float elapsedSeconds);
        void Draw(GraphicsDevice device, SpriteBatchExtended sb);
        void ProcessAttributes(XmlAttributeCollection visualAttributeCollection);
    }
}