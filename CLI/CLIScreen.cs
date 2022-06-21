// -----------------------------------------------------------------------
// <copyright file="CLIScreen.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SystemX.CommandProcessor;
using SystemX.Extensions;
using SystemX.GameState;
using SystemX.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SystemX.CLI {
    public class CLIScreen : GameScreen {
        /// <summary>
        ///     Maximum lines that shows in Debug command window.
        /// </summary>
        private const int MaxLineCount = 20;

        /// <summary>
        ///     Cursor character.
        /// </summary>
        private const string Cursor = "_";

        /// <summary>
        ///     Default Prompt string.
        /// </summary>
        public const string DefaultPrompt = ">";

        // Key repeat duration in seconds for the first key press.
        private const float KeyRepeatStartDuration = 0.3f;

        // Key repeat duration in seconds after the first key press.
        private const float KeyRepeatDuration = 0.03f;
        private const string DefaultFont = "System";

        // Command history buffer.
        private readonly List<string> _commandHistory = new List<string>();
        private readonly string _font;

        // Selecting command history index.
        private int _commandHistoryIndex;

        // Current command line string and cursor position.
        private string _commandLine = string.Empty;
        private int _cursorIndex;
        private float _dt;

        // Timer for key repeating.
        private float _keyRepeatTimer;

        // Key that pressed last frame.
        private Keys _pressedKey;

        public CLIScreen() {
            Prompt = DefaultPrompt;
            _font = DefaultFont;
        }

        /// <summary>
        ///     Gets/Sets Prompt string.
        /// </summary>
        public string Prompt { get; set; }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen) {
            _dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(InputManager input) {
            if (TransitionPosition != 0)
                return;

            KeyboardState keyState = Keyboard.GetState();
            Keys[] keys = keyState.GetPressedKeys();

            bool shift = keyState.IsKeyDown(Keys.LeftShift) || keyState.IsKeyDown(Keys.RightShift);

            foreach (Keys key in keys) {
                if (!IsKeyPressed(input, key, _dt)) continue;

                char ch;
                if (KeyboardUtils.KeyToString(key, shift, out ch)) {
                    // Handle typical character input.
                    _commandLine = _commandLine.Insert(_cursorIndex, new string(ch, 1));
                    _cursorIndex++;
                } else {
                    switch (key) {
                        case Keys.LeftShift:
                        case Keys.RightShift:
                            break;
                        case Keys.OemTilde:
                        case Keys.Oem8:
                            ScreenManager.LogFile.WriteLine("Closing CLI screen.");
                            ExitScreen();
                            break;
                        case Keys.Home:
                            _cursorIndex = 0;
                            break;
                        case Keys.End:
                            _cursorIndex = _commandLine.Length;
                            break;
                        case Keys.Back:
                            if (_cursorIndex > 0)
                                _commandLine = _commandLine.Remove(--_cursorIndex, 1);
                            break;
                        case Keys.Delete:
                            if (_cursorIndex < _commandLine.Length)
                                _commandLine = _commandLine.Remove(_cursorIndex, 1);
                            break;
                        case Keys.Left:
                            if (_cursorIndex > 0)
                                _cursorIndex--;
                            break;
                        case Keys.Right:
                            if (_cursorIndex < _commandLine.Length)
                                _cursorIndex++;
                            break;
                        case Keys.Enter:

                            // Add the command to the history
                            _commandHistory.Add(_commandLine);
                            _commandHistoryIndex = _commandHistory.Count;

                            // Run the command.
                            CommandManager.Write(_commandLine);
                            CommandManager.Process(this, _commandLine);
                            _commandLine = string.Empty;
                            _cursorIndex = 0;
                            break;
                        case Keys.Up:

                            // Show command history.
                            if (_commandHistory.Count > 0) {
                                _commandHistoryIndex = Math.Max(0, _commandHistoryIndex - 1);

                                _commandLine = _commandHistory[_commandHistoryIndex];
                                _cursorIndex = _commandLine.Length;
                            }
                            break;
                        case Keys.Down:

                            // Show command history.
                            if (_commandHistory.Count > 0) {
                                _commandHistoryIndex = Math.Min(_commandHistory.Count, _commandHistoryIndex + 1);

                                if (_commandHistoryIndex == _commandHistory.Count) {
                                    _commandLine = "";
                                    _cursorIndex = _commandLine.Length;
                                } else {
                                    _commandLine = _commandHistory[_commandHistoryIndex];
                                    _cursorIndex = _commandLine.Length;
                                }
                            }
                            break;

                        //default:
                        //    commandLine = commandLine.Insert(cursorIndex, key.ToString());
                        //    cursorIndex += key.ToString().Length;

                        //    break;
                    }
                }
            }
        }

        private bool IsKeyPressed(InputManager input, Keys key, float dt) {
            // Treat it as pressed if given key was not pressed in previous frame.
            if (input.WasKeyUp(key)) {
                _keyRepeatTimer = KeyRepeatStartDuration;
                _pressedKey = key;
                return true;
            }

            // Handling key repeating if given key was pressed in previous frame.
            if (key == _pressedKey) {
                _keyRepeatTimer -= dt;
                if (_keyRepeatTimer <= 0.0f) {
                    _keyRepeatTimer += KeyRepeatDuration;
                    return true;
                }
            }

            return false;
        }

        public override void Draw(GameTime gameTime) {
            SpriteFont tmpFont = ScreenManager.Font[_font];
            SpriteBatchExtended spriteBatch = ScreenManager.SpriteBatch;
            Texture2D whiteTexture = ScreenManager.BlankTexture2D;
            Rectangle tmpTitleSafeArea = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea;

            // Compute command window size and draw.
            float w = tmpTitleSafeArea.Width;
            float topMargin = tmpTitleSafeArea.Top; // h * 0.1f;
            float leftMargin = tmpTitleSafeArea.Left; // w * 0.1f;

            Rectangle rect = new Rectangle {
                                               X = (int)leftMargin,
                                               Y = (int)topMargin,
                                               Width = (int)(w),
                                               Height = MaxLineCount * tmpFont.LineSpacing
                                           };

            Matrix mtx = Matrix.CreateTranslation(new Vector3(0, -rect.Height * (1.0f - TransitionAlpha), 0));

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, mtx);

            spriteBatch.Draw(whiteTexture, rect, new Color(0, 128, 0, 200));

            // Draw each lines.
            Vector2 pos = new Vector2(leftMargin, topMargin);
            foreach (string line in CommandManager.OutputBuffer) {
                spriteBatch.DrawString(tmpFont, line, pos, Color.LightGreen);
                pos.Y += tmpFont.LineSpacing;
            }

            // Draw prompt string.
            string leftPart = Prompt + _commandLine.Substring(0, _cursorIndex);
            Vector2 cursorPos = pos + tmpFont.MeasureString(leftPart);
            cursorPos.Y = pos.Y;

            spriteBatch.DrawString(tmpFont, string.Format("{0}{1}", Prompt, _commandLine), pos, Color.LimeGreen);
            spriteBatch.DrawString(tmpFont, Cursor, cursorPos, Color.White);

            spriteBatch.End();
        }
    }
}