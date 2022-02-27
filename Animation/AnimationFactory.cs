// -----------------------------------------------------------------------
// <copyright file="AnimationFactory.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using SystemX.DataFiles;
using SystemX.SpriteSheets;
using Microsoft.Xna.Framework;
using MyPathHelper = SystemX.Helpers.PathHelper;

namespace SystemX.Animation {
    public static class AnimationFactory {
        private static List<string[]> _animationData;
        public static I_SpriteSheetLibrary SpriteSheetLibrary { get; private set; }

        public static void Initialise(I_SpriteSheetLibrary spriteSheetLibrary) {
            SpriteSheetLibrary = spriteSheetLibrary;
            string AnimationFile = Path.Combine("Content", "Textures", "Animation.dat" );

            if (!File.Exists(Path.Combine(MyPathHelper.GameFolder, AnimationFile)))
                throw new InvalidDataException("Missing Animation.dat");

            // Find and load Animation data
            using (Stream fileStream = TitleContainer.OpenStream(AnimationFile)) 
                _animationData = CSV.Load(fileStream);
        }

        public static Animation Load(string animationReference) {
            int found = 0;

            for (int i = 0; i < _animationData.Count; i++) {
                if (_animationData[i][0] != animationReference)
                    continue;

                found = i;
                break;
            }

            Animation returnValue = new Animation(
                "Main", // TODO: change this out so it's loaded from the file.
                _animationData[found][1],
                int.Parse(_animationData[found][2]),
                int.Parse(_animationData[found][3]),
                int.Parse(_animationData[found][4]),
                float.Parse(_animationData[found][5]),
                bool.Parse(_animationData[found][6])
                );

            return returnValue;
        }
    }
}