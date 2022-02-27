// -----------------------------------------------------------------------
// <copyright file="Control.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml;
using SystemX.GUI.Visuals;
using SystemX.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SystemX.GUI.Controls
{
    /// <summary>
    ///     Base Control class implements I_Control
    /// </summary>
    public abstract class Control : I_Control
    {
        protected bool _enabled;
        private bool _hasFocus;
        protected Point _location = Point.Zero;
        protected Point _size = Point.Zero;
        protected bool _visible;

        protected Control()
        {
            _visible = true;
            _enabled = true;
            NamedVisuals = new Dictionary<string, I_Visual>();
            Visuals = new List<I_Visual>();
        }

        public abstract string KeyRef { get; }
        public List<I_Visual> Visuals { get; private set; }
        public Dictionary<string, I_Visual> NamedVisuals { get; private set; }
        public string Name { get; set; }

        public Point Location
        {
            get
            {
                // if Parent is null assume Ctrl is Window.
                return Parent == null ?
                           new Point(_location.X, _location.Y) :
                           new Point(Parent.Location.X + _location.X, Parent.Location.Y + _location.Y);
            }
            set
            {
                if (_location == value) return;
                _location = value;
                RecalculateBounds();
            }
        }

        [ContentSerializerIgnore]
        public I_Control Parent { get; set; }

        public Point Size
        {
            get
            {
                // if Parent is null assume Ctrl is Window.
                return _size;
            }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    RecalculateBounds();
                }
            }
        }

        public Rectangle Bounds { get; set; }
        public Rectangle TouchBounds { get; set; }
        public string Text { get; set; }
        public float Value { get; set; }
        public string Tag { get; set; }

        public bool Enabled
        {
            // if Parent is null assume Ctrl is Window.
            get
            {
                return Parent == null ? _enabled : Parent.Enabled && _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public bool Visible
        {
            // if Parent is null assume Ctrl is Window.
            get
            {
                return Parent == null ? _visible : Parent.Visible && _visible;
            }
            set
            {
                _visible = value;
            }
        }

        public bool HasFocus
        {
            get
            {
                return _hasFocus;
            }
            set
            {
                if (value != _hasFocus)
                {
                    if (value)
                        GainFocus();
                    else
                        LoseFocus();
                }
                _hasFocus = value;
            }
        }

        public event Action<I_Control, Point> LeftClicked;
        public event Action<I_Control, Point> RightClicked;
        public event Action<I_Control, Point> MouseOver;
        public event Action<I_Control, Point> MouseEnter;
        public event Action<I_Control, Point> MouseOut;

        public virtual void ProcessAttributes(XmlAttributeCollection visualAttributeCollection) { }

        public virtual void RegisterVisuals(RenderEngine renderer)
        {
            foreach (I_Visual visual in Visuals)
            {
                visual.Owner = this;
                renderer.Visuals.Add(visual);

                if (string.IsNullOrEmpty(visual.Name) || NamedVisuals.ContainsKey(visual.Name))
                    continue;

                NamedVisuals.Add(visual.Name, visual);
            }
        }

        public virtual void UnRegisterVisuals(RenderEngine renderer)
        {
            foreach (I_Visual guiVisual in Visuals) renderer.Visuals.Remove(guiVisual);
        }

        /// <summary>
        /// </summary>
        /// <param name="elapsedSeconds"></param>
        public virtual void Update(float elapsedSeconds) { }

        /// <summary>
        ///     Handle any input needed by the control
        /// </summary>
        /// <param name="input">input structure for this cycle</param>
        public virtual void HandleInput(InputManager input) { }

        /// <summary>
        ///     Recalculate Bounds of the control for checking the mouse position against.
        /// </summary>
        public void RecalculateBounds()
        {
            Bounds = new Rectangle(Location.X, Location.Y, Size.X, Size.Y);
            TouchBounds = Bounds;
        }

        /// <summary>
        ///     Method called to trigger the LeftClicked Event
        /// </summary>
        /// <param name="pos">Screen coordinates of the mouse</param>
        public virtual void OnLeftClick(Point pos)
        {
            if (LeftClicked != null)
                LeftClicked(this, pos);
        }

        /// <summary>
        ///     Method called to trigger the RightClicked Event
        /// </summary>
        /// <param name="pos">Screen coordinates of the mouse</param>
        public virtual void OnRightClick(Point pos)
        {
            if (RightClicked != null)
                RightClicked(this, pos);
        }

        /// <summary>
        ///     Method called to trigger the MouseOver Event
        /// </summary>
        /// <param name="pos">Screen coordinates of the mouse</param>
        public virtual void OnMouseOver(Point pos)
        {
            if (MouseOver != null)
                MouseOver(this, pos);
        }

        /// <summary>
        ///     Method called to trigger the MouseEnter Event
        /// </summary>
        /// <param name="pos">Screen coordinates of the mouse</param>
        public virtual void OnMouseEnter(Point pos)
        {
            if (MouseEnter != null)
                MouseEnter(this, pos);
        }

        /// <summary>
        ///     Method called to trigger the MouseOut Event
        /// </summary>
        /// <param name="pos">Screen coordinates of the mouse</param>
        public virtual void OnMouseOut(Point pos)
        {
            if (MouseOut != null)
                MouseOut(this, pos);
        }

        protected virtual void LoseFocus() { }
        protected virtual void GainFocus() { }
    }
}