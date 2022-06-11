// -----------------------------------------------------------------------
// <copyright file="EchoCommand.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using SystemX.Common;

namespace SystemX.CommandProcessor.Commands {
    public class EchoCommand : I_Command {
        private string _tmp = string.Empty;
        public GameStateManager Gm { get; set; }

        public string Name {
            get {
                return "ECHO";
            }
        }

        public string Help {
            get {
                return "Output to screen.";
            }
        }

        public void Execute(object sender, string[] args) {
            if (args[0].ToUpper() != Name)
                throw new CommandException(string.Format("Wrong command sent - '{0}'.", args[0].ToUpper()));

            _tmp = string.Empty;
            for (int i = 1; i < args.Length; i++) _tmp += args[i] + " ";
            CommandManager.Write(_tmp);
        }
    }
}