// -----------------------------------------------------------------------
// <copyright file="BinaryReaderExtension.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;

namespace SystemX.Extensions {
    public static class BinaryReaderExtension {
        public static float ReadFloat(this BinaryReader br) {
            return br.ReadSingle();
        }
    }
}