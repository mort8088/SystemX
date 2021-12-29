// -----------------------------------------------------------------------
// <copyright file="StringHelper.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace SystemX.Helpers {
    public static class StringHelper {
        public static char[] Newline;

        static StringHelper() {
            Newline = new char[2];

            Newline[0] = '\r';
            Newline[1] = '\n';
        }

        public static string[] SplitOnNewLine(string inStr) {
            return inStr.Split(Newline, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}