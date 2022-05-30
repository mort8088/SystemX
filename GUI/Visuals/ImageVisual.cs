using System.Xml;
using SystemX.Common;
using SystemX.Extensions;
using SystemX.GUI.Controls;
using SystemX.GUI.Helpers;
using SystemX.SpriteSheets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.GUI.Visuals {
    public class ImageVisual : I_Visual {
        private Rectangle _layoutRect;
        private string _sheet;
        public Color DisabledTint = Color.White;
        public string ImagePath;

        [ContentSerializerIgnore]
        public I_SpriteSheetLibrary Texture;

        public Color Tint = Color.White;
        public bool UseParentSize;

        string I_Visual.KeyRef {
            get {
                return "IMAGE";
            }
        }

        [ContentSerializerIgnore]
        public I_Control Owner { get; set; }

        public string Name { get; set; }
        public Point Location { get; set; }
        public Point Size { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public Visibility Visibility { get; set; }

        public void LoadGraphics(GameStateManager gsm) {
            if (Texture == null)
                Texture = (I_SpriteSheetLibrary)gsm.Services.GetService(typeof(I_SpriteSheetLibrary));

            if (string.IsNullOrEmpty(_sheet))
                _sheet = "Main";

            if (!string.IsNullOrEmpty(ImagePath)) {
                if (Size == Point.Zero) {
                    if (!UseParentSize)
                        Size = new Point(Texture.Sheets[_sheet][ImagePath].Width, Texture.Sheets[_sheet][ImagePath].Height);
                }
            }
        }

        public void UnloadGraphics() {}

        public void Update(float elapsedSeconds) {}

        public virtual void Draw(GraphicsDevice device, SpriteBatchExtended spriteBatch) {
            if (Texture == null)
                return;

            if (string.IsNullOrEmpty(ImagePath))
                return;

            Color displayColor = Owner.Enabled ? Tint : DisabledTint;
            Point size = UseParentSize ? Owner.Size : Size;
            Point loc = Location;
            Point ownerLoc = Owner.Location;
            Point ownerSize = Owner.Size;
            Point finalLoc = LayoutHelper.DoLayout(HorizontalAlignment, VerticalAlignment, ref ownerLoc, ref ownerSize, ref loc, ref size);

            _layoutRect = new Rectangle {
                                            X = finalLoc.X,
                                            Y = finalLoc.Y,
                                            Width = size.X,
                                            Height = size.Y
                                        };

            spriteBatch.Draw(Texture.Sheets[_sheet].Page, _layoutRect, Texture.Sheets[_sheet][ImagePath].Source, displayColor);
        }

        public void ProcessAttributes(XmlAttributeCollection visualAttributeCollection) {
            ColorConverter colorConv = new ColorConverter();

            DisabledTint = visualAttributeCollection["DisabledTint"] != null
                               ? colorConv.ConvertFromInvariantString(visualAttributeCollection["DisabledTint"].Value)
                               : DisabledTint;

            Tint = visualAttributeCollection["Tint"] != null
                       ? colorConv.ConvertFromInvariantString(visualAttributeCollection["Tint"].Value)
                       : Tint;

            ImagePath = visualAttributeCollection["Src"] != null
                            ? visualAttributeCollection["Src"].Value
                            : "";

            UseParentSize = visualAttributeCollection["UseParentSize"] != null
                                ? bool.Parse(visualAttributeCollection["UseParentSize"].Value)
                                : UseParentSize;
        }
    }
}