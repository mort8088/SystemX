// -----------------------------------------------------------------------
// <copyright file="FPSCommand.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using SystemX.CommandProcesser;
using SystemX.CommandProcesser.Commands;

namespace SystemX.Common {
    public class FPSCommand : I_Command {
        public GameStatemanager Gm { get; set; }

        public string Name {
            get {
                return "FPS";
            }
        }

        public string Help {
            get {
                return string.Format("{1} FONT %FontName% - Sets the Font to use for the FPS display.{0}{1} %State%(On/Off/True/False/1/0) - Turns the FPS counter on or off.{0}{1} - Will show the current status of the FPS counter.", Environment.NewLine, Name);
            }
        }

        public void Execute(object sender, string[] args) {
            if (args[0].ToUpper() != Name)
                throw new CommandException(string.Format("Wrong command sent - '{0}'.", args[0].ToUpper()));
            try {
                if (args.Length == 1) {
                    CommandManager.Write(string.Format("FPS FONT={0} DISPLAY={1}", Gm.FpsFont, Gm.ShowFPS));
                    return;
                }

                switch (args[1].ToUpper().Trim()) {
                    case "FONT":
                        if (args.Length == 3) {
                            try {
                                if (Gm.Font[args[2]] != null)
                                    Gm.FpsFont = args[2];
                            }
                            catch {
                                throw new CommandException(string.Format("Unknown font sent - '{0}'.", args[2]));
                            }
                        }
                        break;
                    case "ON":
                    case "TRUE":
                    case "1":
                        Gm.ShowFPS = true;
                        break;
                    case "OFF":
                    case "FALSE":
                    case "0":
                        Gm.ShowFPS = false;
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