// -----------------------------------------------------------------------
// <copyright file="HelpCommand.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using SystemX.Common;

namespace SystemX.CommandProcesser.Commands {
    public class HelpCommand : I_Command {
        public GameStatemanager Gm { get; set; }

        public string Name {
            get {
                return "HELP";
            }
        }

        public string Help {
            get {
                return "Returns a list of the available commands.";
            }
        }

        public void Execute(object sender, string[] args) {
            if (args[0].ToUpper() != Name)
                throw new CommandException(string.Format("Wrong command sent - '{0}'.", args[0].ToUpper()));

            if (args.Length == 1) {
                foreach (KeyValuePair<string, I_Command> cmdItem in CommandManager.CommandList) CommandManager.Write(cmdItem.Key);
                return;
            }

            try {
                if (CommandManager.CommandList.ContainsKey(args[1].ToUpper().Trim())) CommandManager.Write(CommandManager.CommandList[args[1].ToUpper().Trim()].Help);
            }
            catch (Exception ex) {
                throw new CommandException("Command threw an exception", ex);
            }
        }
    }
}