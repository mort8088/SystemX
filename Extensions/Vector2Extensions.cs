// -----------------------------------------------------------------------
// <copyright file="Vector2Extensions.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;

namespace SystemX.Extensions {

    /// <summary>
    ///     Static class for Vector2 extension methods.
    /// </summary>
    /// <remarks>
    ///     kkkk
    /// </remarks>
    public static class Vector2Extensions {

        /// <summary>
        ///     Return the magnitude of a Vector2.
        /// </summary>
        public static double Mag(this Vector2 v) {
            return  Math.Sqrt(v.X * v.X + v.Y * v.Y); 
        }

        /// <summary>
        ///     Return the magnitude of a Vector2.
        /// </summary>
        public static double Mag2(this Vector2 v) {
            return (v.X * v.X + v.Y * v.Y);
        }    
    }
}