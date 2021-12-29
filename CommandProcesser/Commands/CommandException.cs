// -----------------------------------------------------------------------
// <copyright file="CommandException.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using SystemX.ExceptionHelper;

namespace SystemX.CommandProcesser.Commands {
    public class CommandException : ErrorException {
        public CommandException(string errorMessage)
            : base(errorMessage) {}

        public CommandException(string errorMessage, Exception innerEx)
            : base(errorMessage, innerEx) {}
    }
}