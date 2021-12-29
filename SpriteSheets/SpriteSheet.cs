using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SystemX.SpriteSheets {
    public class SpriteSheet {
        public const string TexturesPath = @"Textures\";
        private readonly Dictionary<string, int> _spriteNames = new Dictionary<string, int>();
        private readonly List<Sprite> _sprs = new List<Sprite>();

        public SpriteSheet() {}

        public SpriteSheet(ContentManager content) {
            Content = content;
        }

        public ContentManager Content { get; set; }
        public Texture2D Page { get; private set; }

        public Sprite this[int index] {
            get {
                return _sprs[index];
            }
        }

        public Sprite this[string index] {
            get {
                return _sprs[_spriteNames[index]];
            }
        }

        public int Count {
            get {
                return _sprs.Count;
            }
        }

        public void Load(string texture2DFileName, string dictionaryFileName) {
            Dictionary<string, Rectangle> spritepositions = Content.Load<Dictionary<string, Rectangle>>(TexturesPath + dictionaryFileName);
            Page = Content.Load<Texture2D>(TexturesPath + texture2DFileName);

            int i = 0;
            foreach (string item in spritepositions.Keys) {
                _spriteNames.Add(item, i++);

                Sprite tmpSpr = new Sprite {
                                               Source = spritepositions[item],
                                               Width = spritepositions[item].Width,
                                               Height = spritepositions[item].Height
                                           };

                _sprs.Add(tmpSpr);
            }
        }

        public int NameToIndex(string name) {
            return _spriteNames[name];
        }
    }
}