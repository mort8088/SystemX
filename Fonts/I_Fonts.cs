// -----------------------------------------------------------------------
// <copyright file="I_Fonts.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Xna.Framework.Graphics;

namespace SystemX.Fonts {
    public interface I_Fonts {
        SpriteFont this[int index] { get; }
        SpriteFont this[string name] { get; }
        int Count { get; }
        void Load(string assetName);
        void Load(string name, string assetName);
    }
}