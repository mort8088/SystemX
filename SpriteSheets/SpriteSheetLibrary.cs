using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace SystemX.SpriteSheets {
    public class SpriteSheetLibrary : I_SpriteSheetLibrary {
        private readonly ContentManager _content;
        private readonly Dictionary<string, SpriteSheet> _sheets = new Dictionary<string, SpriteSheet>();

        public SpriteSheetLibrary(ContentManager content) {
            _content = content;
        }

        #region I_SpriteSheetLibrary Members
        public SpriteSheet this[string sheetName] {
            get {
                return _sheets[sheetName];
            }
        }

        public Dictionary<string, SpriteSheet> Sheets {
            get {
                return _sheets;
            }
        }

        public void LoadSheet(string sheetName, string texture2DFileName, string dictionaryFileName) {
            if (_sheets.ContainsKey(sheetName)) return;

            SpriteSheet tmp = new SpriteSheet(_content);
            tmp.Load(texture2DFileName, dictionaryFileName);

            _sheets.Add(sheetName, tmp);
        }
        #endregion
    }
}