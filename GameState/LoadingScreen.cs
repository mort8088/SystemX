// -----------------------------------------------------------------------
// <copyright file="LoadingScreen.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using SystemX.Common;
using Microsoft.Xna.Framework;

namespace SystemX.GameState {
    public class LoadingScreen : GameScreen {
        private const string StrName = "Loading";
        private readonly bool _loadingIsSlow;
        private readonly GameScreen[] _screensToLoad;
        private bool _otherScreensAreGone;

        private LoadingScreen(GameStateManager screenManager, bool loadingIsSlow, GameScreen[] screensToLoad)
            : base(StrName) {
            _loadingIsSlow = loadingIsSlow;
            _screensToLoad = screensToLoad;
            ScreenManager = screenManager;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        public static void Load(GameStateManager screenManager, bool loadingIsSlow, params GameScreen[] screensToLoad) {
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            screenManager.AddScreen(new LoadingScreen(screenManager, loadingIsSlow, screensToLoad));
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (!_otherScreensAreGone) return;
            ScreenManager.RemoveScreen(this);

            foreach (GameScreen screen in _screensToLoad.Where(screen => screen != null))
            {
                ScreenManager.AddScreen(screen);
            }

            ScreenManager.ResetElapsedTime();
        }

        public override void Draw(GameTime gameTime) {
            if ((ScreenState == ScreenState.Active) && (ScreenManager.GetScreens().Length == 1))
                _otherScreensAreGone = true;

            if (!_loadingIsSlow) return;
            if (Window == null) return;

            Window.Text = "Loading...";
            Renderer.Draw(ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch);
        }
    }
}