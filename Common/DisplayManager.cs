// -----------------------------------------------------------------------
// <copyright file="DisplayManager.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Drawing;
using System.IO;
//using System.Windows.Forms;
using SystemX.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace SystemX.Common {
    // public enum of available screen modes - windowed or full screen
    public enum ScreenMode {
        Windowed = 0,
        FullScreen = 1
    }

    //------------------------------------------------------------------------------
    // Class: DisplayManager
    // Author: Neil Holmes & Andrew Green
    // Summary: handles set up and maintenance of the display. supports full screen 
    //          and windowed modes and provides functionality for correct display 
    //          of the game at any resolution or window size
    //------------------------------------------------------------------------------
    public class DisplayManager {
        /// <summary>
        ///     The aspect ratio we should be using for full screen display modes
        /// </summary>
        private readonly float _aspectRatio;

        private readonly Vector2 _gameResolution;

        /// <summary>
        ///     Global sprite batch instance to be used by the game
        /// </summary>
        private readonly SpriteBatchExtended _globalSpriteBatch;

        /// <summary>
        ///     game's graphics device manager
        /// </summary>
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        /// <summary>
        ///     Main render target - all graphics are rendered here and then scaled to the back buffer once per frame
        /// </summary>
        private readonly RenderTarget2D _mainRenderTarget;

        /// <summary>
        ///     parent game
        /// </summary>
        //private readonly Game _game;
        /// <summary>
        ///     parent window
        /// </summary>
        private readonly GameWindow _window;

        /// <summary>
        ///     A blank texture for use when drawing the black bars in letter boxed modes
        /// </summary>
        private Texture2D _blankTexture;

        private Rectangle _displaySize;

        /// <summary>
        ///     The screen resolution the game will try to display at when in full screen mode
        /// </summary>
        private Vector2 _fullScreenDisplaySize;

        /// <summary>
        ///     The size of the window that the game will display in when in windowed mode
        /// </summary>
        private Vector2 _windowedDisplaySize;

        /// <summary>
        ///     Creates the display manager and sets-up the back buffer and main render targets
        ///     then sets the requested screen mode
        /// </summary>
        /// <param name="game"></param>
        /// <param name="window"></param>
        /// <param name="graphicsDeviceManager"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="screenMode"></param>
        /// <param name="renderScale"></param>
        public DisplayManager(Game game, GameWindow window, GraphicsDeviceManager graphicsDeviceManager, int width, int height, ScreenMode screenMode, float renderScale) {
            int bestFullScreenWidth, bestFullScreenHeight;

            // store a reference to the parent game
            //_game = game;

            // store a reference to the parent window
            _window = window;

            // store a reference to the parent game's graphics device manager
            _graphicsDeviceManager = graphicsDeviceManager;

            // store a copy of the requested render scale
            RenderScale = renderScale;

            // create the transform matrix using the requested render scale
            TransformMatrix = Matrix.CreateScale(renderScale, renderScale, 1.0f);

            // tell the window that we want to allow resizing (remove if you don't want this!)
            window.AllowUserResizing = true;

            // stop the user from being able to make the window so small that we lose the graphics device ;)
            // this happens if the window height becomes zero so you can set the minimum size as low as you like as
            // long as you don't allow it to reach zero. if you turn off AllowUserResizing you don't need this
            // TODO: Fix this or at least be aware it will break.
            //Control.FromHandle(window.Handle).MinimumSize = new Size(320, 200);
            

            // subscribe to the game window's ClientSizeChanged event - again not needed if you turn off user resizing
            window.ClientSizeChanged += WindowClientSizeChanged;

#if NOVSYNC

    // Don't force vsync, but only for the development build, I want to see running at speed.
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
#else

            // force vsync, because not doing so is UGLY and LAZY and BAD and developers that don't use it should be SHOT :P
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
#endif

            // run at max framerate allowed. you can set a fixed time step if you like but your game should really be set
            // up to run at any render speed or on slow machines it will all slow down, not just drop frames
            game.IsFixedTimeStep = false;

            // grab the pixel aspect ratio from the current desktop display mode - we'll assume that the user has this set 
            // correctly for their monitor and use it to filter our full screen modes accordingly
            _aspectRatio = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio;

            // store the requested game resolution
            _gameResolution.X = width;
            _gameResolution.Y = height;

            // set the preferred windowed size for the game to be the game resolution (take rendering scale into account)
            _windowedDisplaySize.X = width * renderScale;
            _windowedDisplaySize.Y = height * renderScale;

            // check that the preferred window size is not larger than the desktop - if it is make it 10% smaller than the desktop
            // size so that the user can actually see it all and move it around/minimize/maximize it etc
            if (_windowedDisplaySize.X > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                _windowedDisplaySize.X = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * 0.9f;

            if (_windowedDisplaySize.Y > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
                _windowedDisplaySize.Y = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * 0.9f;

            // find the most suitable full screen resolution (take rendering scale into account)
            FindBestFullScreenMode((int)(width * renderScale), (int)(height * renderScale), _aspectRatio, out bestFullScreenWidth, out bestFullScreenHeight);
            _fullScreenDisplaySize.X = bestFullScreenWidth;
            _fullScreenDisplaySize.Y = bestFullScreenHeight;

            // set the requested screen mode (full screen or windowed)
            SetScreenMode(screenMode);

            // initialize the main render target and depth buffer that will be used for all game rendering
            _mainRenderTarget = CreateMainRenderTarget(renderScale);

            // create the global sprite batch that we will use for doing all 2d rendering in the game
            _globalSpriteBatch = new SpriteBatchExtended(game);
        }

        /// <summary>
        ///     Gets the game's X resolution
        /// </summary>
        public int GameResolutionX {
            get {
                return (int)GameResolution.X;
            }
        }

        /// <summary>
        ///     Gets the display manager's Y resolution
        /// </summary>
        public int GameResolutionY {
            get {
                return (int)GameResolution.Y;
            }
        }

        /// <summary>
        ///     Gets the display manager's X and Y resolution
        /// </summary>
        public Vector2 GameResolution {
            get {
                return _gameResolution;
            }
        }

        /// <summary>
        ///     Returns the rectangle being using to define the display size and
        ///     offset of the main render target within the back buffer
        /// </summary>
        public Rectangle DisplaySize {
            get {
                return _displaySize;
            }
        }

        /// <summary>
        ///     Returns the screen mode that the game is currently running in
        /// </summary>
        public ScreenMode CurrentScreenMode { get; private set; }

        /// <summary>
        ///     Gets the global render scale that we are using when drawing
        /// </summary>
        public float RenderScale { get; private set; }

        /// <summary>
        ///     Gets the global transformation matrix that should be used when drawing
        /// </summary>
        public Matrix TransformMatrix { get; private set; }

        /// <summary>
        ///     Gets the global sprite batch instance
        /// </summary>
        public SpriteBatchExtended GlobalSpriteBatch {
            get {
                return _globalSpriteBatch;
            }
        }

        /// <summary>
        ///     Returns a 5x5 White Texture
        /// </summary>
        public Texture2D BlankTexture {
            get {
                if (_blankTexture == null) {
                    _blankTexture = new Texture2D(_graphicsDeviceManager.GraphicsDevice, 5, 5, false, SurfaceFormat.Color);
                    Color[] color = new Color[25];
                    for (int i = 0; i < color.Length; i++) color[i] = Color.White;
                    _blankTexture.SetData(color);
                }

                return _blankTexture;
            }
        }

        /// <summary>
        ///     Returns the amount of scaling that is currently being applied to the
        ///     game's render resolution in order to display it on screen
        /// </summary>
        public float DisplayScale {
            get {
                if (CurrentScreenMode == ScreenMode.FullScreen) {
                    if (GameResolution.X != _fullScreenDisplaySize.X)
                        return GameResolution.X / _fullScreenDisplaySize.X;
                    if (GameResolution.Y != _fullScreenDisplaySize.Y)
                        return GameResolution.Y / _fullScreenDisplaySize.Y;

                    return 1.0f;
                }
                if (DisplaySize.Y > 0)
                    return GameResolution.X / _windowedDisplaySize.X;
                if (DisplaySize.X > 0)
                    return GameResolution.Y / _windowedDisplaySize.Y;

                return 1.0f;
            }
        }

        /// <summary>
        ///     Scans through all the available display modes and finds one that best
        ///     fits the game's requested resolution. DisplayManager will automatically
        ///     scale the game res to fit whatever resolution is closest if an exact
        ///     match is not found
        /// </summary>
        /// <param name="desiredWidth"></param>
        /// <param name="desiredHeight"></param>
        /// <param name="aspectRatio"></param>
        /// <param name="bestWidth"></param>
        /// <param name="bestHeight"></param>
        private void FindBestFullScreenMode(int desiredWidth, int desiredHeight, float aspectRatio, out int bestWidth, out int bestHeight) {
            // start off with some impossible numbers for the best width and height
            bestWidth = bestHeight = int.MaxValue;

            // run through all the available modes on the default adapter and find the one that's closes to the game's
            // width, height and format
            foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
                // check if this mode is closer to the requested size than any we already tested
                // NOTE: this test will always pick a resolution larger than the required size rather than smaller if no 
                // exact match for the requested resolution is not available
                if ((displayMode.Width - desiredWidth) >= 0 &&
                    displayMode.Width < bestWidth &&
                    (displayMode.Height - desiredHeight) >= 0 &&
                    displayMode.Height < bestHeight &&
                    displayMode.AspectRatio == aspectRatio) {
                    // found a better resolution match than any previous mode tested - store it's size
                    bestWidth = displayMode.Width;
                    bestHeight = displayMode.Height;
                }
            }

            // check if we found a good match and drop out if we have!
            if (bestWidth != int.MaxValue) return;

            // ok, if we get here then no available resolution was large enough to display the requested game resolution, or,
            // none that were large enough matched the aspect ration we were hoping for. scan through the list of modes again
            // and pick the largest available that matches the aspect ratio we are after
            bestWidth = bestHeight = 0;

            foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
                // check if this mode is larger than any previous mode tested - we can ignore the fact that this will
                // tend to find the largest size available because we've already proven that no available screen size
                // was large enough for the desired game resolution... just get the biggest mode we can!
                if (displayMode.Width >= bestWidth &&
                    displayMode.Height >= bestHeight &&
                    displayMode.AspectRatio == aspectRatio) {
                    // found a larger resolution than any previous mode tested - store it's size
                    bestWidth = displayMode.Width;
                    bestHeight = displayMode.Height;
                }
            }

            // check if we found a match and drop out if we have!
            if (bestWidth != 0) return;

            // ok, if we get here then trying to match the aspect ratio is almost certainly making it so that we can't find any
            // suitable screen resolution - let's try again, but this time we'll ignore the aspect ratio and just go for the best
            // size we can find...
            bestWidth = bestHeight = int.MaxValue;

            foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
                // check if this mode is closer to the requested size than any we already tested
                // NOTE: this test will always pick a resolution larger than the required size rather than smaller if an 
                // exact match for the requested resolution is not available
                if ((displayMode.Width - desiredWidth) >= 0 &&
                    displayMode.Width < bestWidth &&
                    (displayMode.Height - desiredHeight) >= 0 &&
                    displayMode.Height < bestHeight) {
                    // found a better resolution match than any previous mode tested - store it's size
                    bestWidth = displayMode.Width;
                    bestHeight = displayMode.Height;
                }
            }

            // check if we found a match and drop out if we have!
            if (bestWidth != int.MaxValue) return;

            // ok, we're almost out of options! just try and find *something* that we can use! We really don't care about the
            // size or aspect ratio at this point! this should not happen, but you never know! ;-)
            bestWidth = bestHeight = 0;

            foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) {
                // check if this mode is larger than any previous mode tested - we can ignore the fact that this will
                // tend to find the largest size available because we've already proven that no available screen size
                // was large enough for the desired game resolution... just get the biggest mode we can!
                if (displayMode.Width >= bestWidth &&
                    displayMode.Height >= bestHeight) {
                    // found a larger resolution than any previous mode tested - store it's size
                    bestWidth = displayMode.Width;
                    bestHeight = displayMode.Height;
                }
            }
        }

        /// <summary>
        ///     Event handler called when the window size is changed. Calculates the
        ///     new window size and updates the display manager's settings accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClientSizeChanged(object sender, EventArgs e) {
            // we only care if we are in windowed mode
            if (CurrentScreenMode != ScreenMode.Windowed) return;

            // get the updated width and height of the window
            _windowedDisplaySize.X = _window.ClientBounds.Width;
            _windowedDisplaySize.Y = _window.ClientBounds.Height;

            // Don't set the screen mode if the it has a size of 0 in either width or height
            if ((_window.ClientBounds.Height > 0) &&
                (_window.ClientBounds.Width > 0)) {
                // update the display settings to handle the new window size.
                SetScreenMode(CurrentScreenMode);
            }
        }

        /// <summary>
        ///     Helper function for creating the main render target
        /// </summary>
        /// <param name="renderScale"></param>
        /// <returns></returns>
        public RenderTarget2D CreateMainRenderTarget(float renderScale) {
            // create the main render target
            return new RenderTarget2D(
                _graphicsDeviceManager.GraphicsDevice,
                (int)(GameResolution.X * renderScale),
                (int)(GameResolution.Y * renderScale),
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24Stencil8);
        }

        /// <summary>
        ///     Helper function for setting full screen or windowed display mode
        /// </summary>
        /// <param name="screenMode"></param>
        public void SetScreenMode(ScreenMode screenMode) {
            bool Changed = false;
            // store the new screen mode
            CurrentScreenMode = screenMode;

            // process according to the new screen mode
            switch (CurrentScreenMode) {
                case ScreenMode.FullScreen:

                    // create a backbuffer that matches the resolution of the current display mode
                    if (_graphicsDeviceManager.PreferredBackBufferWidth != (int)_fullScreenDisplaySize.X) {
                        Changed = true;
                        _graphicsDeviceManager.PreferredBackBufferWidth = (int)_fullScreenDisplaySize.X;
                    }
                    if (_graphicsDeviceManager.PreferredBackBufferHeight != (int)_fullScreenDisplaySize.Y) {
                        Changed = true;
                        _graphicsDeviceManager.PreferredBackBufferHeight = (int)_fullScreenDisplaySize.Y;
                    }
                    _graphicsDeviceManager.IsFullScreen = true;
                    break;

                case ScreenMode.Windowed:

                    // create a backbuffer that matches the resolution of the current display mode
                    if (_graphicsDeviceManager.PreferredBackBufferWidth != (int)_windowedDisplaySize.X) {
                        Changed = true;
                        _graphicsDeviceManager.PreferredBackBufferWidth = (int)_windowedDisplaySize.X;
                    }
                    if (_graphicsDeviceManager.PreferredBackBufferHeight != (int)_windowedDisplaySize.Y) {
                        Changed = true;
                        _graphicsDeviceManager.PreferredBackBufferHeight = (int)_windowedDisplaySize.Y;
                    }
                    _graphicsDeviceManager.IsFullScreen = false;
                    break;
            }
            if (Changed)
                _graphicsDeviceManager.ApplyChanges();

            // calculate the display rectangle needed to fit the game to the requested display size
            CalculateDisplaySize();
        }

        /// <summary>
        ///     Calculates the display rectangle needed to copy the game from the
        ///     main render target to a back buffer of the requested display size
        /// </summary>
        private void CalculateDisplaySize() {
            // are we in windowed mode, or full screen mode?
            if (CurrentScreenMode == ScreenMode.Windowed) {
                // check if x size matches the window width
                if (GameResolution.X != _windowedDisplaySize.X) {
                    // x size does not match, set the maximum we can have and scale the y size to match it
                    float scale = _windowedDisplaySize.X / GameResolution.X;
                    _displaySize.Width = (int)_windowedDisplaySize.X;
                    _displaySize.Height = (int)(GameResolution.Y * scale);
                } else {
                    // x size matches, store it and the y size
                    _displaySize.Width = (int)GameResolution.X;
                    _displaySize.Height = (int)GameResolution.Y;
                }

                // check y size fits window height
                if (DisplaySize.Height > _windowedDisplaySize.Y) {
                    // y size does not fit, set the maximum we can have and scale the x size down to match it
                    float scale = _windowedDisplaySize.Y / DisplaySize.Height;
                    _displaySize.Width = (int)(DisplaySize.Width * scale);
                    _displaySize.Height = (int)_windowedDisplaySize.Y;
                }

                // calculate the x and y offsets for the display rectangle
                _displaySize.X = (int)((_windowedDisplaySize.X - DisplaySize.Width) * 0.5f);
                _displaySize.Y = (int)((_windowedDisplaySize.Y - DisplaySize.Height) * 0.5f);
            } else {
                // check x size matches the screen resolution
                if (GameResolution.X != _fullScreenDisplaySize.X) {
                    // x size does not match, set the maximum we can have and scale the y size to match it
                    float scale = _fullScreenDisplaySize.X / GameResolution.X;
                    _displaySize.Width = (int)_fullScreenDisplaySize.X;
                    _displaySize.Height = (int)(GameResolution.Y * scale);
                } else {
                    // x size matches, store it and the y size
                    _displaySize.Width = (int)GameResolution.X;
                    _displaySize.Height = (int)GameResolution.Y;
                }

                // check y size fits the screen resolution
                if (DisplaySize.Height > _fullScreenDisplaySize.Y) {
                    // y size does not fit, set the maximum we can have and scale the x size down to match it
                    float scale = _fullScreenDisplaySize.Y / DisplaySize.Height;
                    _displaySize.Width = (int)(DisplaySize.Width * scale);
                    _displaySize.Height = (int)_fullScreenDisplaySize.Y;
                }

                // calculate the x and y offsets for the display rectangle
                _displaySize.X = (int)((_fullScreenDisplaySize.X - DisplaySize.Width) * 0.5f);
                _displaySize.Y = (int)((_fullScreenDisplaySize.Y - DisplaySize.Height) * 0.5f);
            }
        }

        /// <summary>
        ///     Prepares everything for drawing a new frame
        /// </summary>
        public void StartDraw() {
            // set the main render target as the destination for all draw calls
            _graphicsDeviceManager.GraphicsDevice.SetRenderTarget(_mainRenderTarget);
        }

        /// <summary>
        ///     Calls the Normal End Draw method than saves a screenshot.
        /// </summary>
        public void EndDraw(string screenShotDir, ref int screenshotNumber) {
            EndDraw();

            if (!Directory.Exists(Path.Combine(screenShotDir, _window.Title)))
                Directory.CreateDirectory(Path.Combine(screenShotDir, _window.Title));

            string filePath;
            do {
                filePath = string.Format("{0}\\ScreenShot_{1:00}.png", _window.Title, screenshotNumber++);
            } while (File.Exists(Path.Combine(screenShotDir, filePath)));

            using (FileStream fs = new FileStream(Path.Combine(screenShotDir, filePath), FileMode.OpenOrCreate)) {
                _mainRenderTarget.SaveAsPng(fs, DisplaySize.Width, DisplaySize.Height);
                fs.Flush();
            }
        }

        /* removed to make monoGame work.
        public static void Screenshot(GraphicsDevice device)
        {
            // I don't use this one seeing as I'm writing to a back buffer that i can use to save the screen shot.
            // But i keep this around in case i need it some day.
            byte[] screenData = new byte[device.PresentationParameters.BackBufferWidth * device.PresentationParameters.BackBufferHeight * 4];

            device.GetBackBufferData(screenData);

            Texture2D t2D = new Texture2D(device, device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight, false, device.PresentationParameters.BackBufferFormat);

            t2D.SetData(screenData);

            int i = 0;
            string name = string.Format("ScreenShot{0}.png", i);
            while (File.Exists(name))
            {
                i += 1;
                name = string.Format("ScreenShot{0}.png",i);
            }

            Stream st = new FileStream(name, FileMode.Create);

            t2D.SaveAsPng(st, t2D.Width, t2D.Height);

            st.Close();

            t2D.Dispose();
        } 
        */

        /// <summary>
        ///     Copies the finished frame to the back buffer for display
        /// </summary>
        public void EndDraw() {
            Rectangle shape;

            // set the render target to point at the back buffer
            _graphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

            // start the sprite batch (we don't use the matrix for this as we don't want this final copy to be scaled!)
            _globalSpriteBatch.Begin();

            // copy the main render target into the back buffer 
            _globalSpriteBatch.Draw(_mainRenderTarget, DisplaySize, Color.White);

            // draw black bars if the main render target doesn't exactly fit the screen
            if (DisplaySize.Y > 0) {
                // draw the bar at the top of the screen
                shape.X = shape.Y = 0;
                shape.Width = DisplaySize.Width;
                shape.Height = DisplaySize.Y;
                _globalSpriteBatch.Draw(BlankTexture, shape, Color.Black);

                // draw the bar at the bottom of the screen
                shape.Y = DisplaySize.Y + DisplaySize.Height;
                _globalSpriteBatch.Draw(BlankTexture, shape, Color.Black);
            } else if (DisplaySize.X > 0) {
                // draw the bar on the left side of the screen
                shape.X = shape.Y = 0;
                shape.Width = DisplaySize.X;
                shape.Height = DisplaySize.Height;
                _globalSpriteBatch.Draw(BlankTexture, shape, Color.Black);

                // draw the bar on the right side of the screen
                shape.X = DisplaySize.X + DisplaySize.Width;
                _globalSpriteBatch.Draw(BlankTexture, shape, Color.Black);
            }

            // draw safe zone borders
            DrawSafeArea();

            // all done - end sprite batch
            _globalSpriteBatch.End();
        }

        /// <summary>
        ///     Returns the amount of scaling that is currently being applied to the
        ///     game's render resolution in order to display it on screen
        /// </summary>
        public void DrawSafeArea() {
            Rectangle rectangle = new Rectangle();
            Color color = new Color(127, 0, 0, 127);

            Viewport viewport = _graphicsDeviceManager.GraphicsDevice.Viewport;
            Rectangle safeRectangle = viewport.TitleSafeArea;

            // draw the top unsafe area
            rectangle.X = viewport.X;
            rectangle.Y = viewport.Y;
            rectangle.Width = viewport.Width;
            rectangle.Height = safeRectangle.Y;
            GlobalSpriteBatch.Draw(BlankTexture, rectangle, color);

            // draw the left side unsafe area
            rectangle.X = viewport.X;
            rectangle.Y = viewport.Y + safeRectangle.Y;
            rectangle.Width = safeRectangle.X;
            rectangle.Height = safeRectangle.Height;
            GlobalSpriteBatch.Draw(BlankTexture, rectangle, color);

            // draw the bottom unsafe area
            rectangle.X = viewport.X;
            rectangle.Y = viewport.Y + (safeRectangle.Y + safeRectangle.Height);
            rectangle.Width = viewport.Width;
            rectangle.Height = safeRectangle.Y;
            GlobalSpriteBatch.Draw(BlankTexture, rectangle, color);

            // draw the right side unsafe area
            rectangle.X = viewport.X + (safeRectangle.X + safeRectangle.Width);
            rectangle.Y = viewport.Y + safeRectangle.Y;
            rectangle.Width = safeRectangle.X;
            rectangle.Height = safeRectangle.Height;
            GlobalSpriteBatch.Draw(BlankTexture, rectangle, color);
        }
    }
}