using System;
using System.Collections.Generic;
using System.Xml;
using SystemX.GUI.Visuals;
using SystemX.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SystemX.GUI.Controls {
    public interface I_Control {
        /// <summary>
        ///     The XML Node Name that this Control will be created from.
        /// </summary>
        string KeyRef { get; }

        List<I_Visual> Visuals { get; }
        Dictionary<string, I_Visual> NamedVisuals { get; }
        string Name { get; set; }
        Point Location { get; set; }
        Point Size { get; set; }
        Rectangle Bounds { get; set; }
        Rectangle TouchBounds { get; set; }

        [ContentSerializerIgnore]
        I_Control Parent { get; set; }

        string Text { get; set; }
        float Value { get; set; }
        string Tag { get; set; }
        bool Enabled { get; set; }
        bool Visible { get; set; }
        bool HasFocus { get; set; }
        event Action<I_Control, Point> LeftClicked;
        event Action<I_Control, Point> RightClicked;
        event Action<I_Control, Point> MouseOver;
        event Action<I_Control, Point> MouseEnter;
        event Action<I_Control, Point> MouseOut;
        void ProcessAttributes(XmlAttributeCollection visualAttributeCollection);
        void RegisterVisuals(RenderEngine renderer);
        void UnRegisterVisuals(RenderEngine renderer);
        void Update(float elapsedSeconds);
        void HandleInput(InputManager input);
        void RecalculateBounds();
        void OnLeftClick(Point pos);
        void OnRightClick(Point pos);
        void OnMouseOver(Point pos);
        void OnMouseEnter(Point pos);
        void OnMouseOut(Point pos);
    }
}