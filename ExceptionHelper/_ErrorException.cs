// -----------------------------------------------------------------------
// <copyright file="_ErrorException.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace SystemX.ExceptionHelper {
    [Serializable]
    public class ErrorException : Exception {
        public ErrorException(string errorMessage)
            : base(errorMessage) {}

        public ErrorException(string errorMessage, Exception innerEx)
            : base(errorMessage, innerEx) {}

        public string ErrorMessage {
            get {
                return Message;
            }
        }
    }
}