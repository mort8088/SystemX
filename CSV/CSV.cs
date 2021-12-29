// -----------------------------------------------------------------------
// <copyright file="CSV.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

namespace SystemX.DataFiles {
    /// <summary>
    ///     Static Class for loading CSV files
    /// </summary>
    public static class CSV {
        public static List<string[]> Load(string path) {
            List<string[]> parsedData;

            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) parsedData = Load(fileStream);
            return parsedData;
        }

        public static List<string[]> Load(Stream fileStream) {
            List<string[]> parsedData = new List<string[]>();

            try {
                using (StreamReader readFile = new StreamReader(fileStream)) {
                    string line;

                    while ((line = readFile.ReadLine()) != null) {
                        if (line.StartsWith("#"))
                            continue;

                        string[] row = line.Split(',');
                        parsedData.Add(row);
                    }
                }
            }
            catch {
                return new List<string[]>();
            }

            return parsedData;
        }
    }
}