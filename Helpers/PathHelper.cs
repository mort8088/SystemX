// -----------------------------------------------------------------------
// <copyright file="PathHelper.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SystemX.Helpers {
    /// <summary>
    ///     Exposes useful paths
    /// </summary>
    public static class PathHelper {
        private static string _gameFolder = string.Empty;
        public static string RootDirectory;
        public static bool GameNameSet;

        static PathHelper() {
            // Attempt to get the location of the user's Saved Games folder from the registry
            // If that doesn't exist, fall back to "Documents\SavedGames" which is what XNA normally uses
            RootDirectory = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}", null) as string
                            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SavedGames");

            GameNameSet = false;
        }

        public static string GameFolder {
            get {
                if (string.IsNullOrEmpty(_gameFolder)) _gameFolder = Path.GetDirectoryName(Application.ExecutablePath);

                return _gameFolder;
            }
        }

        public static void SetGameName(string gameName) {
            // Append the directory name for our game's save data
            RootDirectory = Path.Combine(RootDirectory, gameName);

            // If the directory isn't there, make it
            if (!Directory.Exists(RootDirectory))
                Directory.CreateDirectory(RootDirectory);

            GameNameSet = true;
        }
    }
}