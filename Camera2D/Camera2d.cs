using System;
using Microsoft.Xna.Framework;

namespace SystemX.Camera2D
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera2d : I_2dCamera
    {
        private Vector2 positionValue;
        private Vector2 _BestPosition;
        private Vector2 _DisplaySize;
        private Rectangle _Bounds;

        #region I_2dCamera Members
        /// <summary>
        /// Get/Set the postion value of the camera
        /// </summary>
        public Vector2 Position
        {
            get { return positionValue; }
            set { positionValue = value; }
        }
        /// <summary>
        /// This is where the camera wants to be.
        /// </summary>
        public Vector2 BestPosition
        {
            set { _BestPosition = value; }
        }
        /// <summary>
        /// How Big the display is
        /// </summary>
        public Vector2 DisplaySize
        {
            get
            {
                return _DisplaySize;
            }
            set
            {
                _DisplaySize = value;
            }
        }
        /// <summary>
        /// Set this to stop the camera going off the map
        /// </summary>
        public Rectangle CameraBounds
        {
            get { return _Bounds; }
            set { _Bounds = value; }
        }
        #endregion

        private float maxScroll = 12;

        public float MaxScroll
        {
            get { return maxScroll; }
            set { maxScroll = Math.Min(value, 1); }
        }

        public Camera2d()
        {
        }

        public void Update()
        {
            MoveToBest();
        }

        private void CheckBounds()
        {
            if (_Bounds != null)
            {
                if (positionValue.X < _Bounds.Left) positionValue.X = _Bounds.Left;
                if (positionValue.X > _Bounds.Right) positionValue.X = _Bounds.Right;

                if (positionValue.Y < _Bounds.Top) positionValue.Y = _Bounds.Top;
                if (positionValue.Y > _Bounds.Bottom) positionValue.Y = _Bounds.Bottom;
            }
        }

        public void SetBounds(int width, int height)
        {
            Vector2 HalfDisplay = (_DisplaySize / 2);
            int halfDisplayWidth = (int)(HalfDisplay.X);
            int halfDisplayHeight = (int)(HalfDisplay.Y);

            int left = (int)(0 + halfDisplayWidth);
            int top = (int)(0 + halfDisplayHeight);
            int right = (int)(width - halfDisplayWidth);
            int bottom = (int)(height - halfDisplayHeight);

            _Bounds = new Rectangle(left, top, right, bottom);
        }

        public void MoveToBest()
        {
            if (!positionValue.Equals(_BestPosition))
            {
                float Dx = ((positionValue.X - _BestPosition.X) / 2);
                if (Dx < -MaxScroll) Dx = -MaxScroll;
                if (Dx > MaxScroll) Dx = MaxScroll;
                MoveLeft(ref Dx);

                float Dy = ((positionValue.Y - _BestPosition.Y) / 2);
                if (Dy < -MaxScroll) Dy = -MaxScroll;
                if (Dy > MaxScroll) Dy = MaxScroll;
                MoveUp(ref Dy);
            }
            CheckBounds();
        }

        public void MoveRight(ref float dist)
        {
            if (dist != 0)
            {
                positionValue.X += dist;
                CheckBounds();
            }
        }

        public void MoveLeft(ref float dist)
        {
            if (dist != 0)
            {
                positionValue.X -= dist;
                CheckBounds();
            }
        }

        public void MoveUp(ref float dist)
        {
            if (dist != 0)
            {
                positionValue.Y -= dist;
                CheckBounds();
            }
        }

        public void MoveDown(ref float dist)
        {
            if (dist != 0)
            {
                positionValue.Y += dist;
                CheckBounds();
            }
        }

    }
}