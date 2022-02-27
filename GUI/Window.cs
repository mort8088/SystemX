using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using SystemX.GUI.Controls;
using SystemX.GUI.Helpers;
using SystemX.GUI.Visuals;
using SystemX.Input;
using mort8088.XML;
using Microsoft.Xna.Framework;

namespace SystemX.GUI
{
    public class Window : Control
    {
        private const int HalfMouseSize = 1;
        private readonly Dictionary<string, I_Control> _controls = new Dictionary<string, I_Control>();
        private I_Control _focusedControl;
        private I_Control _mouseOverControl;
        private Rectangle _mouseRect;
        public List<string> Playlist;

        public Window()
        {
            Name = string.Empty;
            Location = Point.Zero;
            Size = Point.Zero;
            Bounds = Rectangle.Empty;
            TouchBounds = Rectangle.Empty;
            Parent = null;
            Text = string.Empty;
            Value = float.NaN;
            Enabled = true;
            Visible = true;
            Tag = string.Empty;

            Playlist = new List<string>();
            _controls = new Dictionary<string, I_Control>();
            _mouseRect = new Rectangle(0, 0, HalfMouseSize * 2, HalfMouseSize * 2);
        }

        public Window(Point defaultSize)
            : this()
        {
            Size = defaultSize;
        }

        public override string KeyRef
        {
            get
            {
                return "WINDOW";
            }
        }

        public Dictionary<string, I_Control> Controls
        {
            get
            {
                return _controls;
            }
        }

        public override void RegisterVisuals(RenderEngine renderer)
        {
            foreach (I_Visual visual in Visuals)
            {
                visual.Owner = this;
                renderer.Visuals.Add(visual);

                if (!string.IsNullOrEmpty(visual.Name) &&
                    !NamedVisuals.ContainsKey(visual.Name)) NamedVisuals.Add(visual.Name, visual);
            }

            foreach (I_Control ctrl in Controls.Values)
            {
                ctrl.Parent = this;
                ctrl.RegisterVisuals(renderer);
            }
        }

        public override void Update(float elapsedSeconds)
        {
            if (!Enabled) return;

            foreach (KeyValuePair<string, I_Control> e in Controls) e.Value.Update(elapsedSeconds);
        }

        // For use when mouse reading is handled externally
        public bool BubbleInput(InputManager input)
        {
            if (!Enabled) return false;

            Point pos = input.GetMouseCoordinats();

            _mouseRect.X = pos.X - HalfMouseSize;
            _mouseRect.Y = pos.Y - HalfMouseSize;

            bool triggered = false;

            // Check if the mouse is still over the control
            if (_mouseOverControl != null)
            {
                if (_mouseOverControl.Enabled &&
                    !_mouseOverControl.TouchBounds.Intersects(_mouseRect))
                {
                    // If not run onMouseOut
                    _mouseOverControl.OnMouseOut(pos);

                    // and clear MouseOverControl
                    _mouseOverControl = null;
                }
                else if (_mouseOverControl.Enabled &&
                         _mouseOverControl.TouchBounds.Intersects(_mouseRect)) _mouseOverControl.OnMouseOver(pos);
            }

            foreach (I_Control ctrl in Controls.Values)
            {
                if (!ctrl.Enabled) continue;

                if (ctrl.TouchBounds.Intersects(_mouseRect))
                {
                    if (input.IsMouseRightClick())
                    {
                        ctrl.OnRightClick(pos);
                        return true;
                    }

                    if (input.IsMouseLeftClick())
                    {
                        ctrl.OnLeftClick(pos);

                        if (_focusedControl != ctrl)
                        {
                            if (_focusedControl != null)
                                _focusedControl.HasFocus = false;
                            _focusedControl = ctrl;
                            _focusedControl.HasFocus = true;
                        }

                        return true;
                    }

                    if (_mouseOverControl != ctrl)
                    {
                        _mouseOverControl = ctrl;
                        ctrl.OnMouseEnter(pos);
                        triggered = true;
                    }
                }
            }

            if (_focusedControl != null)
                _focusedControl.HandleInput(input);

            if (Enabled && TouchBounds.Contains(_mouseRect))
            {
                if (input.IsMouseRightClick())
                {
                    OnRightClick(pos);
                    triggered = true;
                }

                if (input.IsMouseLeftClick())
                {
                    OnLeftClick(pos);
                    triggered = true;
                }
            }

            return triggered;
        }

        public I_Control GetEnabledChildAtPoint(Vector2 point)
        {
            foreach (I_Control ctrl in Controls.Values)
            {
                if (!ctrl.Enabled) continue;

                _mouseRect.X = (int)point.X - HalfMouseSize;
                _mouseRect.Y = (int)point.Y - HalfMouseSize;

                if (ctrl.TouchBounds.Intersects(_mouseRect)) return ctrl;
            }

            return null;
        }

        public I_Control GetFirstChildAtPoint(Vector2 point)
        {
            foreach (I_Control ctrl in Controls.Values)
            {
                _mouseRect.X = (int)point.X - HalfMouseSize;
                _mouseRect.Y = (int)point.Y - HalfMouseSize;

                if (ctrl.TouchBounds.Intersects(_mouseRect)) return ctrl;
            }

            return null;
        }

        public static Window LoadXml(string filename, Point windowSize)
        {
            Window returnValue = new Window(windowSize);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);
            XmlNode windowNode = xmlDoc.SelectSingleNode("Window");

            if (windowNode != null)
            {
                returnValue.Playlist.AddRange(XmlHelper.ReadAttrib((XmlElement)windowNode, "MUSIC", "").Trim().Split(new[] {
                                                                                                                               ','
                                                                                                                           }, StringSplitOptions.RemoveEmptyEntries));

                ProcessAttributes(ref returnValue, (XmlElement)windowNode, windowSize, windowSize);

                returnValue.Visuals.AddRange(ProcessVisuals(windowNode.SelectSingleNode("Visual"), returnValue));

                ProcessControls(ref returnValue, xmlDoc.SelectSingleNode("//Templates"), windowNode);
            }

            return returnValue;
        }

        private static void ProcessControls(ref Window owner, XmlNode templates, XmlNode windowNode)
        {
            foreach (XmlNode node in windowNode.ChildNodes)
            {
                XmlElement ctrlNode;
                if (node.NodeType == XmlNodeType.Element)
                    ctrlNode = (XmlElement)node;
                else
                    continue;

                if ((ctrlNode.Name.ToUpper() == "VISUAL") ||
                    (ctrlNode.Name.ToUpper() == "TEMPLATES"))
                    continue;

                I_Control ctrl = ControlsFactory.Instance.CreateControl(ctrlNode.Name.ToUpper());
                ctrl.Parent = owner;

                ProcessAttributes(ref ctrl, ctrlNode, owner.Size, Point.Zero);

                if (ctrlNode.HasAttribute("Template") &&
                    templates.HasChildNodes)
                {
                    XmlElement template = (XmlElement)templates.SelectSingleNode("//Template[@Name='" + XmlHelper.ReadAttrib(ctrlNode, "Template", "MissingNode") + "']");
                    if (template != null) ctrl.Visuals.AddRange(ProcessVisuals(template, ctrl));
                }

                ctrl.Visuals.AddRange(ProcessVisuals(ctrlNode.SelectSingleNode("Visual"), ctrl));

                if (ctrl.Visuals == null ||
                    ctrl.Visuals.Count == 0)
                    throw new NullReferenceException("No visuals have been defined for \"" + ctrl.Name + "\"");

                owner.Controls.Add(!string.IsNullOrWhiteSpace(ctrl.Name) ? ctrl.Name : owner.Controls.Count.ToString(CultureInfo.InvariantCulture), ctrl);
            }
        }

        private static IEnumerable<I_Visual> ProcessVisuals(XmlNode visuals, I_Control owner)
        {
            List<I_Visual> output = new List<I_Visual>();

            if (visuals != null)
            {
                foreach (XmlNode node in visuals.ChildNodes)
                {
                    XmlElement visual;
                    if (node.NodeType == XmlNodeType.Element)
                        visual = (XmlElement)node;
                    else
                        continue;

                    I_Visual data = VisualsFactory.Instance.CreateVisual(visual.Name.ToUpper());

                    data.Owner = owner;

                    data.Name = XmlHelper.ReadAttrib(visual, "Name", "");

                    data.Location = !string.IsNullOrEmpty(XmlHelper.ReadAttrib(visual, "Location", "")) ?
                                        LayoutHelper.ParsePoint(XmlHelper.ReadAttrib(visual, "Location"), owner.Size) :
                                        Point.Zero;

                    //data.Location = LayoutHelper.ParsePoint(XmlHelper.ReadAttrib(visual, "Location"), owner.Location);

                    data.Size = !string.IsNullOrEmpty(XmlHelper.ReadAttrib(visual, "Size", "")) ?
                                    LayoutHelper.ParsePoint(XmlHelper.ReadAttrib(visual, "Size"), owner.Size) :
                                    owner.Size;

                    //data.Size = LayoutHelper.ParsePoint(XmlHelper.ReadAttrib(visual, "Size"), owner.Size);
                    
                    data.HorizontalAlignment = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), XmlHelper.ReadAttrib(visual, "Align", "None"));
                    data.VerticalAlignment = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), XmlHelper.ReadAttrib(visual, "VAlign", "None"));
                    data.Visibility = (Visibility)Enum.Parse(typeof(Visibility), XmlHelper.ReadAttrib(visual, "Visibility", "EnabledDisabled"));

                    data.ProcessAttributes(visual.Attributes);

                    output.Add(data);
                }
            }
            return output;
        }

        private static void ProcessAttributes<T>(ref T control, XmlElement node, Point parentSize, Point defaultSize) where T : I_Control
        {
            control.Location = !string.IsNullOrEmpty(XmlHelper.ReadAttrib(node, "Location", "")) ?
                                   LayoutHelper.ParsePoint(XmlHelper.ReadAttrib(node, "Location"), parentSize) :
                                   Point.Zero;

            control.Size = !string.IsNullOrEmpty(XmlHelper.ReadAttrib(node, "Size", "")) ?
                               LayoutHelper.ParsePoint(XmlHelper.ReadAttrib(node, "Size"), parentSize) :
                               defaultSize;

            control.RecalculateBounds();

            control.Name = XmlHelper.ReadAttrib(node, "Name", "");
            control.Text = XmlHelper.ReadAttrib(node, "Text", "");
            control.Value = XmlHelper.ReadAttrib(node, "Value", float.NaN);
            control.Enabled = XmlHelper.ReadAttrib(node, "Enabled", true);
            control.Visible = XmlHelper.ReadAttrib(node, "Visible", true);
            control.Tag = XmlHelper.ReadAttrib(node, "Tag", "");

            control.ProcessAttributes(node.Attributes);
        }
    }
}