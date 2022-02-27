// -----------------------------------------------------------------------
// <copyright file="GameScreen.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using SystemX.AudioLib;
using SystemX.Common;
using SystemX.GUI;
using SystemX.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SystemX.GameState {
    /// <summary>
    ///     Enum describes the screen transition state.
    /// </summary>
    public enum ScreenState {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden
    }

    /// <summary>
    ///     A screen is a single layer that has update and draw logic, and which
    ///     can be combined with other layers to build up a complex menu system.
    ///     For instance the main menu, the options menu, the "are you sure you
    ///     want to quit" message box, and the main game itself are all implemented
    ///     as screens.
    /// </summary>
    public abstract class GameScreen {
        private bool _otherScreenHasFocus;
        protected ContentManager Content;
        protected RenderEngine Renderer;
        protected Window Window;
        protected string WindowAsset;

        protected GameScreen() {
            IsExiting = false;
            ScreenState = ScreenState.TransitionOn;
            TransitionPosition = 1;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            ScreenSize = new Point(1280, 720);
            Window = new Window(ScreenSize);
        }

        protected GameScreen(string windowAsset)
            : this() {
            if (string.IsNullOrEmpty(windowAsset)) throw new ArgumentNullException("windowAsset");

            WindowAsset = Path.Combine("Data", "GUI", string.Format("{0}.GUI", windowAsset));
        }

        protected GameScreen(string windowAsset, Point screenSize)
            : this() {
            ScreenSize = screenSize;
            Window = new Window(ScreenSize);
            WindowAsset = Path.Combine("Data", "GUI", string.Format("{0}.GUI", windowAsset));
        }

        /// <summary>
        ///     Normally when one screen is brought up over the top of another,
        ///     the first screen will transition off to make room for the new
        ///     one. This property indicates whether the screen is only a small
        ///     pop-up, in which case screens underneath it do not need to bother
        ///     transitioning off.
        /// </summary>
        public bool IsPopup { get; protected set; }

        /// <summary>
        ///     Indicates how long the screen takes to
        ///     transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime { get; protected set; }

        /// <summary>
        ///     Indicates how long the screen takes to
        ///     transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime { get; protected set; }

        /// <summary>
        ///     Gets the current position of the screen transition, ranging
        ///     from zero (fully active, no transition) to one (transitioned
        ///     fully off to nothing).
        /// </summary>
        public float TransitionPosition { get; protected set; }

        /// <summary>
        ///     Gets the current alpha of the screen transition, ranging
        ///     from 1 (fully active, no transition) to 0 (transitioned
        ///     fully off to nothing).
        /// </summary>
        public float TransitionAlpha {
            get {
                return 1f - TransitionPosition;
            }
        }

        /// <summary>
        ///     Gets the current screen transition state.
        /// </summary>
        public ScreenState ScreenState { get; protected set; }

        /// <summary>
        ///     There are two possible reasons why a screen might be transitioning
        ///     off. It could be temporarily going away to make room for another
        ///     screen that is on top of it, or it could be going away for good.
        ///     This property indicates whether the screen is exiting for real:
        ///     if set, the screen will automatically remove itself as soon as the
        ///     transition finishes.
        /// </summary>
        public bool IsExiting { get; protected internal set; }

        /// <summary>
        ///     Checks whether this screen is active and can respond to user input.
        /// </summary>
        public bool IsActive {
            get {
                return !_otherScreenHasFocus &&
                       (ScreenState == ScreenState.TransitionOn ||
                        ScreenState == ScreenState.Active);
            }
        }

        /// <summary>
        ///     Gets the manager that this screen belongs to.
        /// </summary>
        public GameStatemanager ScreenManager { get; internal set; }

        public Point ScreenSize { get; set; }

        /// <summary>
        ///     Load graphics content for the screen.
        /// </summary>
        public virtual void LoadContent() {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Services, "Content");

            Renderer = new RenderEngine();

            if (!string.IsNullOrEmpty(WindowAsset)) {
                string strWindowAssetFile = Path.Combine(Content.RootDirectory, WindowAsset);

                if (File.Exists(strWindowAssetFile)) {
                    Window = Window.LoadXml(strWindowAssetFile, ScreenSize);

                    if (Window.Playlist.Count > 0) {
                        Audio.RegisterPlayList(WindowAsset, ref Window.Playlist);
                        Audio.PlayPlaylist(WindowAsset, true);
                    }

                    Window.RegisterVisuals(Renderer);
                    Renderer.LoadGraphics(ScreenManager);
                }
            }
        }

        /// <summary>
        ///     Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent() {
            Audio.KillAllSfx();

            //Audio.StopPlaylist();
            //Audio.UnloadContent();

            if (Window != null)
                Window.UnRegisterVisuals(Renderer);
        }

        /// <summary>
        ///     Allows the screen to run logic, such as updating the transition position.
        ///     Unlike HandleInput, this method is called regardless of whether the screen
        ///     is active, hidden, or in the middle of a transition.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool otherScreenFocus, bool coveredByOtherScreen) {
            if (Window != null) {
                Window.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                Renderer.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            _otherScreenHasFocus = otherScreenFocus;

            // Transition control
            if (IsExiting) {
                // If the screen is going away to die, it should transition off.
                ScreenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, TransitionOffTime, 1)) {
                    // When the transition finishes, remove the screen.
                    ScreenManager.RemoveScreen(this);
                }
            } else if (coveredByOtherScreen) {
                // If the screen is covered by another, it should transition off.
                ScreenState = UpdateTransition(gameTime, TransitionOffTime, 1) ? ScreenState.TransitionOff : ScreenState.Hidden;
            } else {
                // Otherwise the screen should transition on and become active.
                ScreenState = UpdateTransition(gameTime, TransitionOnTime, -1) ? ScreenState.TransitionOn : ScreenState.Active;
            }

            // End Transition control
        }

        /// <summary>
        ///     Helper for updating the screen transition position.
        /// </summary>
        private bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction) {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / time.TotalMilliseconds);

            // Update the transition position.
            TransitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if (((direction < 0) && (TransitionPosition <= 0)) ||
                ((direction > 0) && (TransitionPosition >= 1))) {
                TransitionPosition = MathHelper.Clamp(TransitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }

        /// <summary>
        ///     Allows the screen to handle user input. Unlike Update, this method
        ///     is only called when the screen is active, and not when some other
        ///     screen has taken the focus.
        /// </summary>
        public virtual void HandleInput(InputManager input) {
            if (Window != null) Window.BubbleInput(input);
        }

        /// <summary>
        ///     This is called when the screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime) {
            if (Window != null) Renderer.Draw(ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch);
        }

        /// <summary>
        ///     Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        ///     instantly kills the screen, this method respects the transition timings
        ///     and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen() {
            if (TransitionOffTime == TimeSpan.Zero) {
                // If the screen has a zero transition time, remove it immediately.
                ScreenManager.RemoveScreen(this);
            } else {
                // Otherwise flag that it should transition off and then exit.
                IsExiting = true;
            }
        }
    }
}