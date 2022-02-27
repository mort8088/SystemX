// -----------------------------------------------------------------------
// <copyright file="I_Command.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using SystemX.Common;

namespace SystemX.CommandProcesser.Commands {
    public interface I_Command {
        /// <summary>
        ///     The GameStatemanager that this command is running in.
        /// </summary>
        GameStatemanager Gm { get; set; }

        /// <summary>
        ///     Name of the command used to make the call
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Text displayed when the user calls the Help command for a specific command.
        /// </summary>
        string Help { get; }

        /// <summary>
        ///     The actual method called to execute the command
        /// </summary>
        /// <param name="sender">The object that this command is being performed on/by/to</param>
        /// <param name="args">Arguments for this command</param>
        void Execute(object sender, string[] args);
    }
}