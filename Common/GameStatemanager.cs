// -----------------------------------------------------------------------
// <copyright file="GameStateManager.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SystemX.AudioLib;
using SystemX.CLI;
using SystemX.CommandProcesser;
using SystemX.Config;
using SystemX.Debug;
using SystemX.Extensions;
using SystemX.Fonts;
using SystemX.GameState;
using SystemX.Helpers;
using SystemX.Input;
using SystemX.Logger;
using SystemX.SpriteSheets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.Common {
    public class GameStateManager : Game {
        private readonly string _gameName;
        private readonly List<GameScreen> _screens = new List<GameScreen>();
        private readonly Stack _screenStack = new Stack();
        private GraphicsDeviceManager _graphics;
        private bool _isInitialized;

        #region Time Ruler
        private TimeRuler _tr;
        #endregion

        // handle to the display manager
        internal DisplayManager DisplayManager;

        #region Logging
        public LoggerService LogFile;
        #endregion

        #region Font Manager
        public I_Fonts Font { get; private set; }
        #endregion

        public SpriteBatchExtended SpriteBatch {
            get {
                return DisplayManager.GlobalSpriteBatch;
            }
        }

        protected override void Initialize() {
            // create the display manager and register it as a service
            DisplayManager = new DisplayManager(
                this,
                Window,
                _graphics,
                Settings["Video"]["ScreenWidth"].GetValueAsInt(),
                Settings["Video"]["ScreenHeight"].GetValueAsInt(),
                (ScreenMode)Enum.Parse(typeof(ScreenMode), Settings["Video"]["ScreenMode"].GetValueAsString()),
                1.0f);

            Services.AddService(typeof(DisplayManager), DisplayManager);

            Input = new InputManager(
                new Point(
                    Settings["Video"]["ScreenWidth"].GetValueAsInt(),
                    Settings["Video"]["ScreenHeight"].GetValueAsInt()
                    )
                );
            Input.OnAltEnterHandler += SwitchWindowFullScreen;
            Input.OnScreenshotHandler += SaveScreenshot;

            if (Settings.SettingGroups.ContainsKey("Mouse")) {
                if (Settings["Mouse"].Settings.ContainsKey("Type")) {
                    MouseType = (MouseDisplayType)Enum.Parse(typeof(MouseDisplayType), Settings["Mouse"]["Type"].GetValueAsString());
                    switch (MouseType) {
                        case MouseDisplayType.None:
                            IsMouseVisible = false;
                            MouseOffset = Point.Zero;
                            MouseSprite = string.Empty;
                            break;
                        case MouseDisplayType.Hardware:
                            IsMouseVisible = true;
                            MouseOffset = Point.Zero;
                            MouseSprite = string.Empty;
                            break;
                        case MouseDisplayType.Software:
                            IsMouseVisible = false;
                            MouseOffset = new Point(Settings["Mouse"]["OffsetX"].GetValueAsInt(), Settings["Mouse"]["OffsetY"].GetValueAsInt());
                            MouseSprite = Settings["Mouse"]["Sprite"].GetValueAsString();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            if (Settings.SettingGroups.ContainsKey("Misc")) {
                if ((Settings["Misc"].Settings.ContainsKey("AllowCLI")) && Settings["Misc"]["AllowCLI"].GetValueAsBool()) 
                    Input.OnTildeHandler += ActivateCLI;

                if (Settings["Misc"].Settings.ContainsKey("MouseEnabled")) 
                    IsMouseVisible = Settings["Misc"]["MouseEnabled"].GetValueAsBool();
            }
            Audio.Initialize(Services);
#if TRACE
            _tr.Initialize(); 
            _tr.Visible = true;
            _tr.ShowLog = true;
#endif
            base.Initialize();

            // Call Autoexec.
            CommandManager.Process(this, @"autoexec");

            _isInitialized = true;
        }

        protected override void Dispose(bool disposing) {
            UpdateDefaultSettings();

            base.Dispose(disposing);

            LogFile.WriteLine("Game State Manager Stopped.");
            LogFile.Flush();

            if (LogFile != null)
                LogFile.Dispose();
        }

        protected override void LoadContent() {
            LogFile.WriteLine("Loading Content");
            if (Settings.SettingGroups.ContainsKey("SpriteSheets")) {
                foreach (KeyValuePair<string, Setting> sheetDef in Settings["SpriteSheets"].Settings) {
                    string sheetName = sheetDef.Key;
                    string textureFile = sheetDef.Value.GetValueAsStringArray()[0];
                    string dictionaryFile = sheetDef.Value.GetValueAsStringArray()[1];
                    LogFile.Write("Loading spritesheet({0}, {1}, {2})", sheetName, textureFile, dictionaryFile);
                    _spriteSheetLibrary.LoadSheet(sheetName, textureFile, dictionaryFile);
                    LogFile.WriteLine(" - DONE.");
                }
            }

            // Load system font
            LogFile.Write("Loading system font");
            Font.Load("System", "System");
            LogFile.WriteLine(" - DONE.");

            if (Settings.SettingGroups.ContainsKey("Fonts")) {
                foreach (KeyValuePair<string, Setting> fontDef in Settings["Fonts"].Settings) {
                    LogFile.Write("Loading Font {0} from {1}", fontDef.Key, fontDef.Value.GetValueAsString());
                    Font.Load(fontDef.Key, fontDef.Value.GetValueAsString());
                    LogFile.WriteLine(" - DONE.");
                }
            }

            // Tell each of the screens to load their content.
            foreach (GameScreen screen in _screens) {
                LogFile.WriteLine("Calling .LoadContent() on {0}", screen.GetType());
                screen.LoadContent();
            }
        }

        protected override void UnloadContent() {
            LogFile.WriteLine("Unloading Content");

            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in _screens) {
                LogFile.WriteLine("Calling .UnloadContent() on {0}", screen.GetType());
                screen.UnloadContent();
            }

            Audio.UnloadContent();
        }

        protected override void Update(GameTime gameTime) {
            if (!IsActive) {
                _tr.ResetLog();
                return;
            }

            //if ((gameTime.TotalGameTime.Seconds % 10) == 0.0d) 
            //    _tr.ResetLog();

            _tr.StartFrame();
            _tr.BeginMark(0, "Update", Color.Blue);

            // Calculate FPS
            if (ShowFPS) {
                _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _totalFrames++;

                if (_elapsedTime >= 1.0f) {
                    _fps = _totalFrames;
                    _totalFrames = 0;
                    _elapsedTime = 0;
                }
            }

            // End Calculate FPS

            Audio.Update();

            Input.Update(DisplayManager.DisplaySize);

            foreach (GameScreen screen in _screens)
                _screenStack.Push(screen);

            bool otherScreenHasFocus = !IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (_screenStack.Count > 0) {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = (GameScreen)_screenStack.Pop();

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active) {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus) {
                        screen.HandleInput(Input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-pop-up, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            base.Update(gameTime);

            // End measuring the Update method
            _tr.EndMark(0, "Update");

            if (_screens.Count == 0)
                Exit();
        }

        protected override void Draw(GameTime gameTime) {
            if (!IsActive) return;

            // Begin measuring our Draw method
            _tr.BeginMark(0, "Draw", Color.Red);

            base.Draw(gameTime);

            DisplayManager.StartDraw();
#if DEBUG
            GraphicsDevice.Clear(Color.HotPink);
#else
            GraphicsDevice.Clear(Color.Black);
#endif
            foreach (GameScreen screen in _screens) {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }

            // End measuring our Draw method
            _tr.EndMark(0, "Draw");
#if TRACE
            _tr.Draw(gameTime);
#endif

            if (MouseType == MouseDisplayType.Software) {
                SpriteBatch.Begin();
                Point tmp = Input.GetMouseCoordinats();
                _mouseDrawPos = new Vector2(tmp.X - MouseOffset.X, tmp.Y - MouseOffset.Y);
                SpriteBatch.DrawSprite(MouseSprite, _mouseDrawPos, Color.White);
                SpriteBatch.End();
            }

            #region Draw FPS
            if ((!string.IsNullOrWhiteSpace(FpsFont)) && ShowFPS) {
                SpriteBatch.Begin();
                SpriteBatch.DrawString(
                    Font[FpsFont],
                    _fps.ToString(CultureInfo.InvariantCulture),
                    new Vector2(0, DisplayManager.GameResolutionY - Font[FpsFont].LineSpacing),
                    Color.Red,
                    0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    0);
                SpriteBatch.End();
            }
            #endregion

            if (_takeScreenshot) {
                DisplayManager.EndDraw(ScreenShotDir, ref _screenshotNumber);
                _takeScreenshot = false;
            } else
                DisplayManager.EndDraw();
        }

        public void AddScreen(GameScreen screen) {
            screen.ScreenManager = this;
            screen.IsExiting = false;
            screen.ScreenSize =
                new Point(
                    Settings["Video"]["ScreenWidth"].GetValueAsInt(),
                    Settings["Video"]["ScreenHeight"].GetValueAsInt()
                    );

            // If we have a graphics device, tell the screen to load content.
            if (_isInitialized) screen.LoadContent();

            _screens.Add(screen);

            LogFile.WriteLine("Screen Added - {0}", screen.GetType().ToString());
        }

        public void RemoveScreen(GameScreen screen) {
            // If we have a graphics device, tell the screen to unload content.
            if (_isInitialized) screen.UnloadContent();

            _screens.Remove(screen);

            LogFile.WriteLine("Screen Removed - {0}", screen.GetType().ToString());
        }

        public GameScreen[] GetScreens() {
            return _screens.ToArray();
        }

        public void FadeBackBufferToBlack(float alpha) {
            Viewport viewport = GraphicsDevice.Viewport;

            DisplayManager.GlobalSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, DisplayManager.TransformMatrix);

            DisplayManager.GlobalSpriteBatch.Draw(
                DisplayManager.BlankTexture,
                new Rectangle(0, 0, viewport.Width, viewport.Height),
                Color.Black * alpha);

            DisplayManager.GlobalSpriteBatch.End();
        }

        private void SwitchWindowFullScreen(object sender, EventArgs e) {
            LogFile.WriteLine("Switching screen mode from {0}", DisplayManager.CurrentScreenMode.ToString());

            switch (DisplayManager.CurrentScreenMode) {
                case ScreenMode.Windowed:
                    DisplayManager.SetScreenMode(ScreenMode.FullScreen);
                    break;
                case ScreenMode.FullScreen:
                    DisplayManager.SetScreenMode(ScreenMode.Windowed);
                    break;
            }
        }

        private void SaveScreenshot(object sender, EventArgs e) {
            _takeScreenshot = true;
        }

        private void ActivateCLI(object sender, EventArgs e) {
            bool foundCLI = _screens.Any(screen => screen.GetType() == typeof(CLIScreen));

            if (!foundCLI) {
                LogFile.WriteLine("Opening CLI screen.");
                AddScreen(new CLIScreen());
            }
        }

        #region Screenshot
        private bool _takeScreenshot;
        public string ScreenShotDir { get; set; }
        private int _screenshotNumber = 1;
        #endregion

        #region Frames Per Second
        private float _elapsedTime, _totalFrames, _fps;

        public string FpsFont { get; set; }
        public bool ShowFPS { get; set; }
        #endregion

        #region Input manager
        public InputManager Input { get; set; }

        public MouseDisplayType MouseType { get; set; }
        public string MouseSprite { get; set; }
        public Point MouseOffset { get; set; }
        private Vector2 _mouseDrawPos;
        #endregion

        #region SpriteSheet Manager
        private SpriteSheetLibrary _spriteSheetLibrary;

        public I_SpriteSheetLibrary SpriteSheets {
            get {
                return _spriteSheetLibrary;
            }
        }

        public Texture2D BlankTexture2D {
            get {
                return DisplayManager.BlankTexture;
            }
        }
        #endregion

        #region Settings
        public bool UsingDefaultSettings { get; set; }
        public I_Config Settings { get; set; }
        #endregion

        #region Creator
        public GameStateManager() {
            _gameName = "Default";
            DefaultInit();
        }

        public GameStateManager(string name) {
            _gameName = name;
            DefaultInit();
        }

        private void DefaultInit() {
            PathHelper.SetGameName(_gameName);

            ScreenShotDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            Window.Title = string.Format("{0}", _gameName);

            IsMouseVisible = true;

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            LogFile = new LoggerService(Path.Combine(PathHelper.GameFolder, string.Format("{0}.log", _gameName)));
            Services.AddService(typeof(I_LoggerService), LogFile);
            LogFile.WriteLine("Game State Manager Started.");

            CommandManager.Init(this);

            if (File.Exists(Path.Combine(PathHelper.RootDirectory, string.Format("{0}.ini", _gameName)))) {
                UsingDefaultSettings = false;
                Settings = new ConfigIni(Path.Combine(PathHelper.RootDirectory, string.Format("{0}.ini", _gameName)));
            } else {
                /* 
                 * Create defaults 
                 */
                UsingDefaultSettings = true;
                Settings = new ConfigIni();

                // Video settings
                Settings.AddSettingsGroup("Video");
                Settings["Video"].AddSetting("ScreenWidth", 1280);
                Settings["Video"].AddSetting("ScreenHeight", 720);
                Settings["Video"].AddSetting("ScreenMode", ScreenMode.Windowed.ToString());

                // Audio settings
                Settings.AddSettingsGroup("Audio");
                Settings["Audio"].AddSetting("Mute", false);
                Settings["Audio"].AddSetting("MusicVolume", 0.1f);
                Settings["Audio"].AddSetting("SfxVolume", 0.5f);

                // Mouse Settings
                Settings.AddSettingsGroup("Mouse");
                Settings["Mouse"].AddSetting("Type", MouseDisplayType.Hardware.ToString());
                Settings["Mouse"].AddSetting("Sprite", string.Empty);
                Settings["Mouse"].AddSetting("OffsetX", 0);
                Settings["Mouse"].AddSetting("OffsetY", 0);

                // Miscellaneous Settings
                Settings.AddSettingsGroup("Misc");

                Settings["Misc"].AddSetting("MouseEnabled", true);
#if DEBUG
                Settings["Misc"].AddSetting("AllowCLI", true);
#else
                Settings["Misc"].AddSetting("AllowCLI", false);
#endif

                UpdateDefaultSettings();
            }
            Services.AddService(typeof(I_Config), Settings);

            Services.AddService(typeof(I_Fonts), Font = new FontList(Content));

            Services.AddService(typeof(I_SpriteSheetLibrary), _spriteSheetLibrary = new SpriteSheetLibrary(Content));

            Services.AddService(typeof(TimeRuler), _tr = new TimeRuler(this));
        }

        protected void UpdateDefaultSettings() {
            Settings.Save(Path.Combine(PathHelper.RootDirectory, string.Format("{0}.ini", _gameName)));
        }
        #endregion
    }
}