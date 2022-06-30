using System;
using SystemX.GUI.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SystemX.Input {
    public enum MouseMode {
        Relative,
        Absolute
    }

    public class InputManager {
        private Rectangle _currentDisplay;
        private GamePadState _currentGamePadStates;
        private KeyboardState _currentKeyboardStates;
        private MouseState _currentMouseState;
        private GamePadState _lastGamePadStates;
        private KeyboardState _lastKeyboardStates;
        private Point _mousePos;
        private MouseMode _mouseReadMode = MouseMode.Absolute;
        private MouseState _previousMouseState;
        private Point _screenSize;

        /// <summary>
        ///     Constructs a new input manager, set up input objects
        /// </summary>
        public InputManager(Point screenSize) {
            _screenSize = screenSize;
        }

        public void Update(Rectangle screenSize) {
            _mousePos = Point.Zero;
            _screenSize = new Point {
                                        X = screenSize.Width,
                                        Y = screenSize.Height
                                    };
            _currentDisplay = screenSize;

            switch (_mouseReadMode) {
                case MouseMode.Relative:
                    _previousMouseState = _currentMouseState;
                    _currentMouseState = Mouse.GetState();
                    Mouse.SetPosition((_currentDisplay.Width >> 1), (_currentDisplay.Height >> 1));
                    break;
                case MouseMode.Absolute:
                    _previousMouseState = _currentMouseState;
                    _currentMouseState = Mouse.GetState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _lastKeyboardStates = _currentKeyboardStates;
            _currentKeyboardStates = Keyboard.GetState();

            _lastGamePadStates = _currentGamePadStates;
            _currentGamePadStates = GamePad.GetState(PlayerIndex.One);

            #region Check for special Windows key-presses
//#if WINDOWS // This will only happen on a windows system
            if (((IsKeyPress(Keys.LeftAlt)) || IsKeyPress(Keys.RightAlt)) && (IsKeyPress(Keys.Enter)))
            {
                OnAltEnter();
            }

            if (IsNewKeyPress(Keys.F12))
            {
                OnScreenshot();
            }

            if (IsNewKeyPress(Keys.OemTilde) || IsNewKeyPress(Keys.Oem8))
            {
                OnTilde();
            }
//#endif
            #endregion
        }

        // Keyboard
        public bool IsNewKeyPress(Keys key) {
            return (_currentKeyboardStates.IsKeyDown(key) && _lastKeyboardStates.IsKeyUp(key));
        }

        public bool IsKeyPress(Keys key) {
            return (_currentKeyboardStates.IsKeyDown(key));
        }

        public bool IsAnyKeyPress() {
            return _currentKeyboardStates.GetPressedKeys().Length > 0;
        }

        // GamePad
        public bool IsNewButtonPress(Buttons button) {
            return (_currentGamePadStates.IsButtonDown(button) && _lastGamePadStates.IsButtonUp(button));
        }

        public bool GamePadIsConnected() {
            return _currentGamePadStates.IsConnected;
        }

        // Mouse
        public void SetMouseReadMode(MouseMode setTo) {
            _mouseReadMode = setTo;

            if (setTo == MouseMode.Relative)
                Mouse.SetPosition((_currentDisplay.Width >> 1), (_currentDisplay.Height >> 1));
        }

        public MouseMode GetMouseReadMode() {
            return _mouseReadMode;
        }

        public Point GetMouseCoordinates() {
            if (_mousePos == Point.Zero) {
                float xPec = (_currentMouseState.X - _currentDisplay.X) / (float)_currentDisplay.Width * 100.0f;
                float yPec = (_currentMouseState.Y - _currentDisplay.Y) / (float)_currentDisplay.Height * 100.0f;

                string strPos = string.Format("{0:0.##\\%}, {1:0.##\\%}", xPec, yPec);

                _mousePos = LayoutHelper.ParsePoint(strPos, _screenSize);
            }
            return _mousePos;
        }

        public Point GetMouseDelta() {
            switch (_mouseReadMode) {
                case MouseMode.Absolute:
                    return new Point {
                                         X = 0,
                                         Y = 0

                                         //X = currentMouseState.X - previousMouseState.X,
                                         //Y = currentMouseState.Y - previousMouseState.Y
                                     };
                case MouseMode.Relative:
                    return new Point {
                                         X = (_currentDisplay.Width >> 1) - _currentMouseState.X,
                                         Y = (_currentDisplay.Height >> 1) - _currentMouseState.Y
                                     };
            }

            return Point.Zero;
        }

        public int GetMouseWheel() {
            return _currentMouseState.ScrollWheelValue;
        }

        public int GetMouseWheelDelta() {
            return _currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;
        }

        public bool IsMouseXButton1Click() {
            return (_currentMouseState.XButton1 == ButtonState.Released) && (_previousMouseState.XButton1 == ButtonState.Pressed);
        }

        public bool IsMouseXButton2Click() {
            return (_currentMouseState.XButton2 == ButtonState.Released) && (_previousMouseState.XButton2 == ButtonState.Pressed);
        }

        public bool IsMouseLeftClick() {
            return (_currentMouseState.LeftButton == ButtonState.Released) && (_previousMouseState.LeftButton == ButtonState.Pressed);
        }

        public bool IsMouseRightClick() {
            return (_currentMouseState.RightButton == ButtonState.Released) && (_previousMouseState.RightButton == ButtonState.Pressed);
        }

        public bool IsMouseMiddleClick() {
            return (_currentMouseState.MiddleButton == ButtonState.Released) && (_previousMouseState.MiddleButton == ButtonState.Pressed);
        }

        public bool IsMouseLeftHold() {
            return (_currentMouseState.LeftButton == ButtonState.Pressed) && (_previousMouseState.LeftButton == ButtonState.Pressed);
        }

        public bool IsMouseRightHold() {
            return (_currentMouseState.RightButton == ButtonState.Pressed) && (_previousMouseState.RightButton == ButtonState.Pressed);
        }

        public bool IsMouseMiddleHold() {
            return (_currentMouseState.MiddleButton == ButtonState.Pressed) && (_previousMouseState.MiddleButton == ButtonState.Pressed);
        }

        public bool IsLeftMouseDown() {
            return _currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool IsMiddleMouseDown() {
            return _currentMouseState.MiddleButton == ButtonState.Pressed;
        }

        public bool IsRightMouseDown() {
            return _currentMouseState.RightButton == ButtonState.Pressed;
        }

        // Specials
        public bool IsMenuSelect() {
            return IsNewKeyPress(Keys.Space) || IsNewKeyPress(Keys.Enter) || IsNewButtonPress(Buttons.A) || IsNewButtonPress(Buttons.Start);
        }

        public bool IsMenuCancel() {
            return IsNewKeyPress(Keys.Escape) || IsNewButtonPress(Buttons.B) || IsNewButtonPress(Buttons.Back);
        }

        public bool IsMenuUp() {
            return IsNewKeyPress(Keys.Up) || IsNewButtonPress(Buttons.DPadUp) || IsNewButtonPress(Buttons.LeftThumbstickUp);
        }

        public bool IsMenuDown() {
            return IsNewKeyPress(Keys.Down) || IsNewButtonPress(Buttons.DPadDown) || IsNewButtonPress(Buttons.LeftThumbstickDown);
        }

        public bool IsMenuRight() {
            return IsNewKeyPress(Keys.Right) || IsNewButtonPress(Buttons.DPadRight) || IsNewButtonPress(Buttons.LeftThumbstickRight);
        }

        public bool IsMenuLeft() {
            return IsNewKeyPress(Keys.Left) || IsNewButtonPress(Buttons.DPadLeft) || IsNewButtonPress(Buttons.LeftThumbstickLeft);
        }

        public bool IsPauseGame() {
            return IsNewKeyPress(Keys.Escape) || IsNewButtonPress(Buttons.Back) || IsNewButtonPress(Buttons.Start);
        }

        public event EventHandler<EventArgs> OnAltEnterHandler;
        public event EventHandler<EventArgs> OnScreenshotHandler;
        public event EventHandler<EventArgs> OnTildeHandler;

        internal bool WasKeyUp(Keys key) {
            return _lastKeyboardStates.IsKeyUp(key);
        }

        protected virtual void OnAltEnter() {
            if (OnAltEnterHandler != null) 
                OnAltEnterHandler(this, EventArgs.Empty);
        }

        protected virtual void OnScreenshot() {
            if (OnScreenshotHandler != null) 
                OnScreenshotHandler(this, EventArgs.Empty);
        }

        protected virtual void OnTilde() {
            if (OnTildeHandler != null) 
                OnTildeHandler(this, EventArgs.Empty);
        }
    }
}