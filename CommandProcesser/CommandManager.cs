// -----------------------------------------------------------------------
// <copyright file="CommandManager.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using SystemX.CommandProcesser.Commands;
using SystemX.Common;
using SystemX.Helpers;
using SystemX.Logger;
using mort8088.XML;

namespace SystemX.CommandProcesser {
    public static class CommandManager {
        private static I_LoggerService _logger;
        private static char[] _commandSeperators;

        /// <summary>
        ///     Loaded Game Commands - Basic functionality
        /// </summary>
        public static Dictionary<string, I_Command> CommandList = new Dictionary<string, I_Command>();

        /// <summary>
        ///     Loaded Game Methods - Scripts of commands
        /// </summary>
        public static Dictionary<string, string> MethodList = new Dictionary<string, string>();

        //public static Queue<MessageRequest> MessageQueue = new Queue<MessageRequest>();

        public static List<string> Outputbuffer { get; set; }

        public static void Init(GameStatemanager gm) {
            _logger = gm.LogFile;

            _commandSeperators = new char[2];
            _commandSeperators[0] = ',';
            _commandSeperators[1] = ' ';

            // Set-up Output buffer
            Outputbuffer = new List<string>();

            // Check the current Current Domain for more DLLs that might have I_Command objects to add
            foreach (I_Command plugin in AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => (from asmType in asm.GetTypes() where asmType.GetInterface("I_Command") != null select (I_Command)Activator.CreateInstance(asmType)))) {
                plugin.Gm = gm;
                CommandList.Add(plugin.Name.ToUpper(), plugin);
            }

            // Check for script files
            foreach (string filename in Directory.GetFiles(Path.Combine(gm.Content.RootDirectory, @"Data\Script"), "*.sxs")) {
                try {
                    XmlDocument scriptFile = new XmlDocument();
                    scriptFile.Load(filename);
                    if (scriptFile.DocumentElement != null &&
                        scriptFile.DocumentElement.Name != "SystemX") continue;

                    XmlNodeList methods = scriptFile.SelectNodes("//scripting/method");
                    if (methods != null) {
                        foreach (XmlElement method in methods) {
                            string strName = XmlHelper.ReadAttrib(method, "name", "ERROR");
                            if (strName.Equals("ERROR") ||
                                MethodList.ContainsKey(strName)) {
                                _logger.WriteLine("Scripting Method in {0} missing name attrib", filename);
                                continue;
                            }

                            string strCommands = XmlHelper.ReadTextNode(method, "commands", "ERROR");
                            if (strCommands.Equals("ERROR") ||
                                string.IsNullOrEmpty(strCommands.Trim())) {
                                _logger.WriteLine("Scripting Method {0} in {1} missing commands node", strName, filename);
                                continue;
                            }

                            MethodList.Add(strName.Trim().ToUpper(), strCommands.Trim());
                        }
                    }
                }
                catch {
                    _logger.WriteLine("There was an error reading the scripting file {0}", filename);
                }
            }
        }

        public static void Process(object sender, string command) {
            try {
                // Process should action the requested command immediately rather than buffer things until the next update.
                string[] tmpParams = command.Split(_commandSeperators, StringSplitOptions.RemoveEmptyEntries);

                if (tmpParams.Length == 0)
                    return;

                if (CommandList.Keys.Contains(tmpParams[0].ToUpper())) CommandList[tmpParams[0].ToUpper()].Execute(sender, tmpParams);
                else if (MethodList.Keys.Contains(tmpParams[0].ToUpper())) {
                    string[] commandCalls = StringHelper.SplitOnNewLine(MethodList[tmpParams[0].ToUpper()]);

                    foreach (string commandCall in commandCalls) Process(sender, commandCall.Trim());
                } else Write(string.Format("Unknown Command \"{0}\"", tmpParams[0].ToUpper()));
            }
            catch (CommandException ce) {
                Write(ce.InnerException == null
                          ? string.Format("{0}", ce.Message)
                          : string.Format("{0}", ce.InnerException.Message));
            }
        }

        public static void Write(string msg) {
            string[] lines = StringHelper.SplitOnNewLine(msg);

            foreach (string item in lines) Outputbuffer.Add(item);
        }
    }
}