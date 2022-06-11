// -----------------------------------------------------------------------
// <copyright file="ScreenShotCommand.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.IO;
using SystemX.CommandProcessor.Commands;

namespace SystemX.Common {
    public class ScreenShotCommand : I_Command {
        public GameStateManager Gm { get; set; }

        public string Name {
            get {
                return "SCREENSHOT";
            }
        }

        public string Help {
            get {
                return string.Format("{0} PATH %Path% - Set the Save location for screenshots to %Path%", Name);
            }
        }

        public void Execute(object sender, string[] args) {
            if (args[0].ToUpper() != Name)
                throw new CommandException(string.Format("Wrong command sent - '{0}'.", args[0].ToUpper()));
            try {
                switch (args[1].ToUpper().Trim()) {
                    case "PATH":
                        if (args.Length > 2) {
                            if (Directory.Exists(args[2]))
                                Gm.ScreenShotDir = args[2];
                            else
                                throw new CommandException(string.Format("Path Not found - '{0}'.", args[2].ToUpper()));
                        }
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