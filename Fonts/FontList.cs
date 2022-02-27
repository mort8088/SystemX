// -----------------------------------------------------------------------
// <copyright file="FontList.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.Fonts {
    public class FontList : I_Fonts {
        private readonly ContentManager _contentManager;
        private readonly Dictionary<string, SpriteFont> _fontList;
        private readonly List<string> _fontNameLookUp;

        public FontList(ContentManager content) {
            _contentManager = content;
            _fontList = new Dictionary<string, SpriteFont>();
            _fontNameLookUp = new List<string>();
        }

        #region I_Fonts Members
        public int Count {
            get {
                return _fontNameLookUp.Count;
            }
        }

        public SpriteFont this[int index] {
            get {
                return _fontList[_fontNameLookUp[index]];
            }
        }

        public SpriteFont this[string name] {
            get {
                return _fontList[name];
            }
        }

        public void Load(string name, string assetName) {
            // Load the font using the CM object
            SpriteFont loadedFont = _contentManager.Load<SpriteFont>(string.Format("Fonts\\{0}", assetName));

            if (loadedFont.DefaultCharacter == null)
                loadedFont.DefaultCharacter = ' ';

            // If the font exists overwrite
            if (_fontNameLookUp.Contains(name))
                _fontList[name] = loadedFont;
            else // Else add the loaded font to the font list
            {
                _fontList.Add(name, loadedFont);

                // add the asset name to the font name look up.
                _fontNameLookUp.Add(name);
            }
        }

        public void Load(string assetName) {
            // Load the font using the CM object
            Load(assetName, assetName);
        }
        #endregion
    }
}