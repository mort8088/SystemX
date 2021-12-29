// -----------------------------------------------------------------------
// <copyright file="SpriteSheetCommand.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using SystemX.CommandProcesser.Commands;
using SystemX.Common;

namespace SystemX.SpriteSheets {
    public class SpriteSheetCommand : I_Command {
        public GameStatemanager Gm { get; set; }

        public string Name {
            get {
                return "SPRSHEET";
            }
        }

        public string Help {
            get {
                return string.Format("SPRSHEET LOAD %SheetName%, %Texture2DFileName%, %DictionaryFileName%{0}SPRSHEET DISPOSE %SheetName%", Environment.NewLine);
            }
        }

        public void Execute(object sender, string[] args) {
            if (args[0].ToUpper() != Name)
                throw new CommandException(string.Format("Wrong command sent - '{0}'.", args[0].ToUpper()));
            try {
                switch (args[1].ToUpper().Trim()) {
                    case "LOAD":
                        if (args.Length == 5)
                            Gm.SpriteSheets.LoadSheet(args[2].Trim(), args[3].Trim(), args[4].Trim());
                        else
                            throw new CommandException(string.Format("incorrect parameters sent :- {0}", Help));
                        break;
                    case "DISPOSE":
                        if (args.Length == 3)
                            Gm.SpriteSheets.Sheets.Remove(args[2].Trim());
                        else
                            throw new CommandException(string.Format("incorrect parameters sent :- {0}", Help));
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