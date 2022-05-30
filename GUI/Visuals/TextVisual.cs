using System.Globalization;
using System.Xml;
using SystemX.Common;
using SystemX.Extensions;
using SystemX.Fonts;
using SystemX.GUI.Controls;
using SystemX.GUI.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.GUI.Visuals
{
    public class TextVisual : I_Visual
    {
        private I_Fonts _font;
        public bool AllowParentResize;
        public Color DisabledColor = Color.Gray;
        public string FontPath;
        public string ManualText = "";
        public TextSource Source;
        public Color StrokeColor = Color.Transparent;
        public Point StrokeOffset = new Point(1, 1);
        public Color TextColor = Color.Black;

        string I_Visual.KeyRef
        {
            get
            {
                return "TEXT";
            }
        }

        [ContentSerializerIgnore]
        public I_Control Owner { get; set; }

        public string Name { get; set; }

        // Absolute location if Alignment is None, otherwise margin
        // Always relative to Owner location
        public Point Location { get; set; }

        // Does Nothing
        public Point Size { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public Visibility Visibility { get; set; }

        public void LoadGraphics(GameStateManager gsm)
        {
            if (_font == null)
                _font = (I_Fonts)gsm.Services.GetService(typeof(I_Fonts));
        }

        public void UnloadGraphics() { }
        public void Update(float elapsedSeconds) { }

        public void Draw(GraphicsDevice device, SpriteBatchExtended spriteBatch)
        {
            if (_font == null)
                return;

            if (string.IsNullOrEmpty(FontPath))
                return;

            string text = "";
            switch (Source)
            {
                case TextSource.Manual:
                    text = ManualText;
                    break;
                case TextSource.ControlText:
                    text = Owner.Text;
                    break;
                case TextSource.ControlValue:
                    if (!float.IsNaN(Owner.Value))
                        text = Owner.Value.ToString(CultureInfo.InvariantCulture);
                    break;
            }

            if (string.IsNullOrEmpty(text))
                return;

            text = text.Replace("\\\\n", "\n");
            text = text.Replace("\\n", "\n");

            Vector2 tmp = _font[FontPath].MeasureString(text);
            Point textSize = new Point((int)tmp.X, (int)tmp.Y);
            Point ownerSize = Owner.Size;

            if (AllowParentResize)
            {
                bool recalc = false;

                if (ownerSize.X < textSize.X)
                {
                    ownerSize.X = textSize.X;
                    recalc = true;
                }

                if (ownerSize.Y < textSize.Y)
                {
                    ownerSize.Y = textSize.Y;
                    recalc = true;
                }

                if (recalc)
                {
                    Owner.Size = ownerSize;
                    Owner.RecalculateBounds();
                }
            }

            Point ownerLoc = Owner.Location;
            Point loc = Location;
            Point finalLoc = LayoutHelper.DoLayout(HorizontalAlignment, VerticalAlignment, ref ownerLoc, ref ownerSize, ref loc, ref textSize);

            if (StrokeColor != Color.Transparent)
                spriteBatch.DrawString(_font[FontPath], text, new Vector2(finalLoc.X + StrokeOffset.X, finalLoc.Y + StrokeOffset.Y), StrokeColor);

            spriteBatch.DrawString(_font[FontPath], text, new Vector2(finalLoc.X, finalLoc.Y), Owner.Enabled ? TextColor : DisabledColor);
        }

        public void ProcessAttributes(XmlAttributeCollection visualAttributeCollection)
        {
            ColorConverter colorConv = new ColorConverter();

            ManualText = visualAttributeCollection["Text"].Value;

            if (ManualText.Equals("{TEXT}"))
                Source = TextSource.ControlText;
            else if (ManualText.Equals("{VALUE}"))
                Source = TextSource.ControlValue;
            else
                Source = TextSource.Manual;

            FontPath = visualAttributeCollection["Font"].Value;

            TextColor = visualAttributeCollection["Color"] != null
                            ? colorConv.ConvertFromInvariantString(visualAttributeCollection["Color"].Value)
                            : TextColor;

            AllowParentResize = visualAttributeCollection["AllowParentResize"] != null
                                    ? bool.Parse(visualAttributeCollection["AllowParentResize"].Value)
                                    : AllowParentResize;

            DisabledColor = visualAttributeCollection["DisabledColor"] != null
                                ? colorConv.ConvertFromInvariantString(visualAttributeCollection["DisabledColor"].Value)
                                : DisabledColor;

            StrokeColor = visualAttributeCollection["StrokeColor"] != null
                              ? colorConv.ConvertFromInvariantString(visualAttributeCollection["StrokeColor"].Value)
                              : StrokeColor;

            StrokeOffset = visualAttributeCollection["StrokeOffset"] != null
                               ? new Point(int.Parse(visualAttributeCollection["StrokeOffset"].Value), int.Parse(visualAttributeCollection["StrokeOffset"].Value))
                               : StrokeOffset;
        }
    }
}