// -----------------------------------------------------------------------
// <copyright file="ClsCommand.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using SystemX.Common;

namespace SystemX.CommandProcessor.Commands {
    public class ClsCommand : I_Command {
        public GameStateManager Gm { get; set; }

        public string Name {
            get {
                return "CLS";
            }
        }

        public string Help {
            get {
                return "Clear Screen.";
            }
        }

        public void Execute(object sender, string[] args) {
            if (args[0].ToUpper() != Name)
                throw new CommandException(string.Format("Wrong command sent - '{0}'.", args[0].ToUpper()));

            CommandManager.Outputbuffer.Clear();
        }
    }
}