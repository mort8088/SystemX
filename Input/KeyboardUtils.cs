﻿// -----------------------------------------------------------------------
// <copyright file="KeyboardUtils.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace SystemX.Input
{
    /// <summary>
    /// Helper class for keyboard input.
    /// </summary>
    public static class KeyboardUtils
    {
        // key:Keys, value:CharPair
        static private readonly Dictionary<Keys, CharPair> KeyMap = new Dictionary<Keys, CharPair>();

        static KeyboardUtils()
        {
            InitializeKeyMap();
        }

        /// <summary>
        /// Gets a character from key information.
        /// </summary>
        /// <param name="key">Pressing key</param>
        /// <param name="shitKeyPressed">Is shift key pressed?</param>
        /// <param name="character">Converted character from key input.</param>
        /// <returns>Returns true when it gets a character</returns>
        public static bool KeyToString(Keys key, bool shitKeyPressed, out char character)
        {
            bool result = false;
            character = ' ';
            CharPair charPair;

            if ((key >= Keys.A && key <= Keys.Z) || key == Keys.Space)
            {
                // Use as is if it is A～Z, or Space key.
                character = (shitKeyPressed) ? (char)key : char.ToLower((char)key);
                result = true;
            }
            else if (KeyMap.TryGetValue(key, out charPair))
            {
                // Otherwise, convert by key map.
                if (!shitKeyPressed)
                {
                    character = charPair.NormalChar;
                    result = true;
                }
                else if (charPair.ShiftChar.HasValue)
                {
                    character = charPair.ShiftChar.Value;
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Initialize character map.
        /// </summary>
        static void InitializeKeyMap()
        {
            // First row of US keyboard.
            AddKeyMap(Keys.OemTilde, "`~");
            AddKeyMap(Keys.D1, "1!");
            AddKeyMap(Keys.D2, "2@");
            AddKeyMap(Keys.D3, "3#");
            AddKeyMap(Keys.D4, "4$");
            AddKeyMap(Keys.D5, "5%");
            AddKeyMap(Keys.D6, "6^");
            AddKeyMap(Keys.D7, "7&");
            AddKeyMap(Keys.D8, "8*");
            AddKeyMap(Keys.D9, "9(");
            AddKeyMap(Keys.D0, "0)");
            AddKeyMap(Keys.OemMinus, "-_");
            AddKeyMap(Keys.OemPlus, "=+");

            // Second row of US keyboard.
            AddKeyMap(Keys.OemOpenBrackets, "[{");
            AddKeyMap(Keys.OemCloseBrackets, "]}");
            AddKeyMap(Keys.OemPipe, "\\|");

            // Third row of US keyboard.
            AddKeyMap(Keys.OemSemicolon, ";:");
            AddKeyMap(Keys.OemQuotes, "'\"");
            AddKeyMap(Keys.OemComma, ",<");
            AddKeyMap(Keys.OemPeriod, ".>");
            AddKeyMap(Keys.OemQuestion, "/?");

            // Keypad keys of US keyboard.
            AddKeyMap(Keys.NumPad1, "1");
            AddKeyMap(Keys.NumPad2, "2");
            AddKeyMap(Keys.NumPad3, "3");
            AddKeyMap(Keys.NumPad4, "4");
            AddKeyMap(Keys.NumPad5, "5");
            AddKeyMap(Keys.NumPad6, "6");
            AddKeyMap(Keys.NumPad7, "7");
            AddKeyMap(Keys.NumPad8, "8");
            AddKeyMap(Keys.NumPad9, "9");
            AddKeyMap(Keys.NumPad0, "0");
            AddKeyMap(Keys.Add, "+");
            AddKeyMap(Keys.Divide, "/");
            AddKeyMap(Keys.Multiply, "*");
            AddKeyMap(Keys.Subtract, "-");
            AddKeyMap(Keys.Decimal, ".");
        }

        /// <summary>
        ///　Added key and character map.
        /// </summary>
        /// <param name="key">Keyboard key</param>
        /// <param name="charPair">
        /// Character, If it is two characters, first character is for not holding the shift key,
        /// and the second character for holding the shift key.</param>
        static void AddKeyMap(Keys key, string charPair)
        {
            char char1 = charPair[0];
            char? char2 = null;
            if (charPair.Length > 1)
                char2 = charPair[1];

            KeyMap.Add(key, new CharPair(char1, char2));
        }

        class CharPair
        {
            public readonly char NormalChar;
            public char? ShiftChar;

            public CharPair(char normalChar, char? shiftChar)
            {
                NormalChar = normalChar;
                ShiftChar = shiftChar;
            }
        }
    }
}
