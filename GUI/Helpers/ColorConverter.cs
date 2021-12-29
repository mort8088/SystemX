using Microsoft.Xna.Framework;

namespace SystemX.GUI.Helpers {
    internal class ColorConverter {
        private readonly int[] rgba = new int[4];
        private string[] pieces = new string[4];

        internal Color ConvertFromInvariantString(string p) {
            try {
                pieces = p.Split(',');

                for (int i = 0; i < 4; i++) {
                    if (!int.TryParse(pieces[i], out rgba[i]))
                        return Color.Red;
                }

                return new Color(rgba[0], rgba[1], rgba[2], rgba[3]);
            }
            catch {
                return Color.Pink;
            }
        }
    }
}