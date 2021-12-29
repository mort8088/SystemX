using System.Collections.Generic;

namespace SystemX.SpriteSheets {
    public interface I_SpriteSheetLibrary {
        SpriteSheet this[string sheetName] { get; }
        Dictionary<string, SpriteSheet> Sheets { get; }
        void LoadSheet(string sheetName, string texture2DFileName, string dictionaryFileName);
    }
}