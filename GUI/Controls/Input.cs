using System.Xml;
using SystemX.GUI.Helpers;
using SystemX.GUI.Visuals;
using SystemX.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SystemX.GUI.Controls
{
    public class Input : Control, IKeyboardSubscriber {
        private bool _password = false;

        public override string KeyRef 
        {
            get { return "INPUT"; }
        }

        public override void ProcessAttributes(XmlAttributeCollection visualAttributeCollection)
        {
            ColorConverter colorConv = new ColorConverter();

            _password = visualAttributeCollection["Password"] != null
                           ? bool.Parse(visualAttributeCollection["Password"].Value)
                           : _password;

            // Add in the Ctrl's Visuals

            #region image visual
            ImageVisual imgVis = new ImageVisual
            {
                Owner = this,
                Name = "",
                Location = Point.Zero,
                Size = Size,
                HorizontalAlignment = HorizontalAlignment.None,
                VerticalAlignment = VerticalAlignment.None,
                ImagePath = "Blank",
                UseParentSize = true
            };
            imgVis.Tint = visualAttributeCollection["background-color"] != null
                              ? colorConv.ConvertFromInvariantString(visualAttributeCollection["background-color"].Value)
                              : Color.White;
            imgVis.DisabledTint = Color.Lerp(imgVis.Tint, Color.Black, 0.5f);

            Visuals.Add(imgVis);
            #endregion

            #region Text visual
            TextVisual txtVis = new TextVisual
            {
                Owner = this,
                Name = "",
                Location = Point.Zero,
                Size = Size,
                HorizontalAlignment = HorizontalAlignment.None,
                VerticalAlignment = VerticalAlignment.Middle,
                Source = TextSource.ControlText,
                FontPath = visualAttributeCollection["Font"].Value,
                AllowParentResize = true
            };

            txtVis.TextColor = visualAttributeCollection["Color"] != null
                                   ? colorConv.ConvertFromInvariantString(visualAttributeCollection["Color"].Value)
                                   : txtVis.TextColor;

            Visuals.Add(txtVis);
            #endregion
        }

        protected override void GainFocus() {
            
        }

        protected override void LoseFocus() {
            
        }

        void IKeyboardSubscriber.RecieveTextInput(char inputChar)
        {
            throw new System.NotImplementedException();
        }

        void IKeyboardSubscriber.RecieveTextInput(string text)
        {
            throw new System.NotImplementedException();
        }

        void IKeyboardSubscriber.RecieveCommandInput(char command)
        {
            throw new System.NotImplementedException();
        }

        void IKeyboardSubscriber.RecieveSpecialInput(Keys key)
        {
            throw new System.NotImplementedException();
        }

        bool IKeyboardSubscriber.Selected
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }
    }
}