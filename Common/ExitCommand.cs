// -----------------------------------------------------------------------
// <copyright file="ExitCommand.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using SystemX.CommandProcessor.Commands;

namespace SystemX.Common {
    public class ExitCommand : I_Command {
        public GameStateManager Gm { get; set; }

        public string Name {
            get {
                return "EXIT";
            }
        }

        public string Help {
            get {
                return string.Format("{0} - Exit the Game.", Name);
            }
        }

        public void Execute(object sender, string[] args) {
            if (args[0].ToUpper() != Name)
                throw new CommandException(string.Format("Wrong command sent - '{0}'.", args[0].ToUpper()));

            try {
                Gm.Exit();
            }
            catch (Exception ex) {
                throw new CommandException("Command threw an exception", ex);
            }
        }
    }
}