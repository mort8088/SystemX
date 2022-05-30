// -----------------------------------------------------------------------
// <copyright file="FontCommand.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using SystemX.CommandProcesser.Commands;
using SystemX.Common;

namespace SystemX.Fonts {
    public class FontCommand : I_Command {
        public GameStateManager Gm { get; set; }

        public string Name {
            get {
                return "FONT";
            }
        }

        public string Help {
            get {
                return string.Format("{1} LOAD %Name%,%AssetPath% - Load the file at %AssetPath% into the Reference %Name%{0}{1} LOAD %AssetPath% - Load the file at AssetPath use the filename(-.Ext) as the reference", Environment.NewLine, Name);
            }
        }

        public void Execute(object sender, string[] args) {
            if (args[0].ToUpper() != Name)
                throw new CommandException(string.Format("Wrong command sent - '{0}'.", args[0].ToUpper()));
            try {
                switch (args[1].ToUpper().Trim()) {
                    case "LOAD":
                        if (args.Length == 4)
                            Gm.Font.Load(args[2], args[3]);
                        else
                            Gm.Font.Load(args[2]);
                        break;
                    default:
                        throw new CommandException(string.Format("Wrong sub-command sent - '{0}'.", args[1].ToUpper()));
                }
            }
            catch (Exception ex) {
                throw new CommandException("Command threw an exception", ex);
            }
        }
    }
}