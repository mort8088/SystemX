using System;
using System.Xml;
using SystemX.Common;
using SystemX.Extensions;
using SystemX.GUI.Controls;
using SystemX.GUI.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.GUI.Visuals
{
    public class CircleVisual : I_Visual
    {
        private Vector3[] _angularDirs;
        private Vector3 _center = Vector3.Zero;
        private BasicEffect _fx;
        private short[] _inds;
        private float _innerRad;
        private Point _loc = Point.Zero;
        private Matrix _matProj;
        private float _outerRad = 1f;
        private int _scrWidth, _scrHeight;
        private VertexPositionColor[] _verts;
        private bool _vertsDirty;
        public Color Color = Color.White;
        public int Density = 8;
        public bool Visible { get; set; }

        public float InnerRadius
        {
            get
            {
                return _innerRad;
            }
            set
            {
                if (value <= _outerRad)
                {
                    _innerRad = value;
                    _vertsDirty = true;
                }
            }
        }

        public float OuterRadius
        {
            get
            {
                return _outerRad;
            }
            set
            {
                if (value >= _innerRad)
                {
                    _outerRad = value;
                    _vertsDirty = true;
                }
            }
        }

        public int ScreenWidth
        {
            get
            {
                return _scrWidth;
            }
            set
            {
                _scrWidth = value;
                _matProj = Matrix.CreateOrthographicOffCenter(0, _scrWidth, _scrHeight, 0, 0, 1);
            }
        }

        public int ScreenHeight
        {
            get
            {
                return _scrHeight;
            }
            set
            {
                _scrHeight = value;
                _matProj = Matrix.CreateOrthographicOffCenter(0, _scrWidth, _scrHeight, 0, 0, 1);
            }
        }

        string I_Visual.KeyRef
        {
            get
            {
                return "CIRCLE";
            }
        }

        [ContentSerializerIgnore]
        public I_Control Owner { get; set; }

        public string Name { get; set; }

        public Point Location
        {
            get
            {
                _loc.X = (int)_center.X;
                _loc.Y = (int)_center.Y;
                return _loc;
            }
            set
            {
                _loc = value;
                _center.X = _loc.X;
                _center.Y = _loc.Y;
            }
        }

        public Point Size { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }
        public Visibility Visibility { get; set; }

        public void LoadGraphics(GameStateManager gsm)
        {
            _fx = new BasicEffect(gsm.GraphicsDevice)
            {
                Projection = _matProj,
                View = Matrix.Identity,
                VertexColorEnabled = true
            };

            _verts = new VertexPositionColor[Density * 2];
            GenerateGenAngles();
            UpdateVerts();
            GenerateInds();
        }

        public void UnloadGraphics()
        {
            if (_fx != null &&
                !_fx.IsDisposed)
                _fx.Dispose();
        }

        public void Update(float elapsedSeconds) {
            if (!Visible)
                return;

            if (_vertsDirty)
            {
                UpdateVerts();
                _vertsDirty = false;
            }
            
        }

        public void Draw(GraphicsDevice device, SpriteBatchExtended sb)
        {
            if (!Visible)
                return;

            sb.End();

            _fx.World = Matrix.CreateTranslation(_center);
            _fx.Projection = _matProj;

            foreach (EffectPass effectPass in _fx.CurrentTechnique.Passes)
            {
                effectPass.Apply();

                device.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    _verts, 0, _verts.Length,
                    _inds, 0, _inds.Length / 3);
            }

            sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
        }

        public void ProcessAttributes(XmlAttributeCollection visualAttributeCollection)
        {
            ColorConverter colorConv = new ColorConverter();

            Density = visualAttributeCollection["Density"] != null
                          ? int.Parse(visualAttributeCollection["Density"].Value)
                          : Density;

            Color = visualAttributeCollection["Color"] != null
                        ? colorConv.ConvertFromInvariantString(visualAttributeCollection["Color"].Value)
                        : Color;
        }

        public void SetRadius(float innerRadius, float outerRadius)
        {
            if (outerRadius > innerRadius)
            {
                _outerRad = outerRadius;
                _innerRad = innerRadius;
                _vertsDirty = true;
            }
        }

        private void GenerateGenAngles()
        {
            _angularDirs = new Vector3[Density];
            float theta = 0f;

            for (short i = 0; i < Density; i++)
            {
                _angularDirs[i] = new Vector3((float)Math.Cos(theta), (float)Math.Sin(theta), 0);
                _angularDirs[i].Normalize();
                _angularDirs[i].Z = 0;
                theta += MathHelper.TwoPi / Density;
            }
        }

        private void UpdateVerts()
        {
            int index = 0;
            for (short i = 0; i < Density; i++)
            {
                _verts[index++] = new VertexPositionColor(_angularDirs[i] * _innerRad, Color);
                _verts[index++] = new VertexPositionColor(_angularDirs[i] * _outerRad, Color);
            }
        }

        private void GenerateInds()
        {
            _inds = new short[Density * 6];

            int index = 0;
            for (short i = 0; i < Density; i++)
            {
                _inds[index] = (short)(i * 2);
                _inds[index + 1] = (short)(_inds[index] + 1);
                _inds[index + 2] = (short)(((i + 1) * 2) % _verts.Length);

                _inds[index + 3] = _inds[index + 2];
                _inds[index + 4] = _inds[index + 1];
                _inds[index + 5] = (short)(_inds[index + 2] + 1);

                index += 6;
            }
        }
    }
}