// -----------------------------------------------------------------------
// <copyright file="ConfigINI.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SystemX.Config {
    /// <summary>
    ///     Represents a configuration file loaded from an .ini file.
    /// </summary>
    public class ConfigIni : I_Config {
        /// <summary>
        ///     Creates a blank configuration file.
        /// </summary>
        public ConfigIni() {
            SettingGroups = new Dictionary<string, SettingsGroup>();
        }

        /// <summary>
        ///     Loads a configuration file.
        /// </summary>
        /// <param name="file">The filename where the configuration file can be found.</param>
        public ConfigIni(string file) {
            Load(file);
        }

        /// <summary>
        ///     Loads a configuration file.
        /// </summary>
        /// <param name="stream">The stream from which to load the configuration file.</param>
        public ConfigIni(Stream stream) {
            Load(stream);
        }

        /// <summary>
        ///     Gets the groups found in the configuration file.
        /// </summary>
        public Dictionary<string, SettingsGroup> SettingGroups { get; private set; }

        /// <summary>
        ///     Indexer Property for the SettingGroups collection contains basic validation on get
        /// </summary>
        /// <param name="index">string index value</param>
        /// <returns>SettingsGroup</returns>
        public SettingsGroup this[string index] {
            get {
                return SettingGroups.ContainsKey(index) ? SettingGroups[index] : null;
            }
            set {
                SettingGroups[index] = value;
            }
        }

        /// <summary>
        ///     Adds a new settings group to the configuration file.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>The newly created SettingsGroup.</returns>
        public SettingsGroup AddSettingsGroup(string groupName) {
            SettingsGroup group;
            if (SettingGroups.ContainsKey(groupName)) {
                group = SettingGroups[groupName];

                //throw new Exception("Group already exists with name '" + groupName + "'");
            } else {
                group = new SettingsGroup(groupName);
                SettingGroups.Add(groupName, group);
            }
            return group;
        }

        /// <summary>
        ///     Deletes a settings group from the configuration file.
        /// </summary>
        /// <param name="groupName">The name of the group to delete.</param>
        public void DeleteSettingsGroup(string groupName) {
            SettingGroups.Remove(groupName);
        }

        /// <summary>
        ///     Loads the configuration from a file.
        /// </summary>
        /// <param name="file">The file from which to load the configuration.</param>
        public void Load(string file) {
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) Load(stream);
        }

        /// <summary>
        ///     Loads the configuration from a stream.
        /// </summary>
        /// <param name="stream">The stream from which to read the configuration.</param>
        public void Load(Stream stream) {
            // track line numbers for exceptions
            int lineNumber = 0;

            // groups found
            List<SettingsGroup> groups = new List<SettingsGroup>();

            // current group information
            string currentGroupName = null;
            List<Setting> settings = null;

            using (StreamReader reader = new StreamReader(stream)) {
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();
                    lineNumber++;

                    // strip out comments
                    if (line != null &&
                        line.Contains("#")) {
                        if (line.IndexOf("#", StringComparison.Ordinal) == 0)
                            continue;

                        line = line.Substring(0, line.IndexOf("#", StringComparison.Ordinal));
                    }

                    // trim off any extra whitespace
                    if (line != null) line = line.Trim();

                    // try to match a group name
                    if (line == null) continue;
                    Match match = Regex.Match(line, "\\[[a-zA-Z\\d\\s]+\\]");

                    // found group name
                    if (match.Success) {
                        // if we have a current group we're on, we save it
                        if (settings != null &&
                            !string.IsNullOrEmpty(currentGroupName))
                            groups.Add(new SettingsGroup(currentGroupName, settings));

                        // make sure the name exists
                        if (match.Value.Length == 2)
                            throw new Exception(string.Format("Group must have name (line {0})", lineNumber));

                        // set our current group information
                        currentGroupName = match.Value.Substring(1, match.Length - 2);
                        settings = new List<Setting>();
                    } else if (line.Contains("=")) {
                        /*
                             * no group name, check for setting with equals sign
                             */

                        // split the line
                        string[] parts = line.Split(new[] {
                                                              '='
                                                          }, 2);

                        // if we have any more than 2 parts, we have a problem
                        if (parts.Length != 2)
                            throw new Exception(string.Format("Settings must be in the format 'name = value' (line {0})", lineNumber));

                        // trim off whitespace
                        parts[0] = parts[0].Trim();
                        parts[1] = parts[1].Trim();

                        // figure out if we have an array or not
                        bool isArray = false;
                        bool inString = false;

                        // go through the characters
                        foreach (char c in parts[1]) {
                            // any comma not in a string makes us creating an array
                            if (c == ',' &&
                                !inString)
                                isArray = true;

                            // flip the inString value each time we hit a quote
                            else if (c == '"')
                                inString = !inString;
                        }

                        // if we have an array, we have to trim off whitespace for each item and
                        // do some checking for boolean values.
                        if (isArray) {
                            // split our value array
                            string[] pieces = parts[1].Split(',');

                            // need to build a new string
                            StringBuilder builder = new StringBuilder();

                            for (int i = 0; i < pieces.Length; i++) {
                                // trim off whitespace
                                string s = pieces[i].Trim();

                                // convert to lower case
                                string t = s.ToLower();

                                // check for any of the true values
                                if (t == "on" ||
                                    t == "yes" ||
                                    t == "true")
                                    s = "true";

                                // check for any of the false values
                                else if (t == "off" ||
                                         t == "no" ||
                                         t == "false")
                                    s = "false";

                                // append the value
                                builder.Append(s);

                                // if we are not on the last value, add a comma
                                if (i < pieces.Length - 1)
                                    builder.Append(",");
                            }

                            // save the built string as the value
                            parts[1] = builder.ToString();
                        } else {
                            /*
                                 * if not an array
                                 */

                            // make sure we are not working with a string value
                            if (!parts[1].StartsWith("\"")) {
                                // convert to lower
                                string t = parts[1].ToLower();

                                // check for any of the true values
                                if (t == "on" ||
                                    t == "yes" ||
                                    t == "true")
                                    parts[1] = "true";

                                // check for any of the false values
                                else if (t == "off" ||
                                         t == "no" ||
                                         t == "false")
                                    parts[1] = "false";
                            }
                        }

                        // add the setting to our list making sure, once again, we have stripped
                        // off the whitespace
                        if (settings == null) continue;
                        settings.Add(new Setting(parts[0].Trim(), parts[1].Trim(), isArray));
                    }
                }
            }

            // make sure we save off the last group
            if (settings != null &&
                !string.IsNullOrEmpty(currentGroupName))
                groups.Add(new SettingsGroup(currentGroupName, settings));

            // create our new group dictionary
            SettingGroups = new Dictionary<string, SettingsGroup>();

            // add each group to the dictionary
            foreach (SettingsGroup group in groups) SettingGroups.Add(group.Name, group);
        }

        /// <summary>
        ///     Saves the configuration to a file
        /// </summary>
        /// <param name="filename">The filename for the saved configuration file.</param>
        public void Save(string filename) {
            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write)) Save(stream);
        }

        /// <summary>
        ///     Saves the configuration to a stream.
        /// </summary>
        /// <param name="stream">The stream to which the configuration will be saved.</param>
        public void Save(Stream stream) {
            using (StreamWriter writer = new StreamWriter(stream)) {
                foreach (KeyValuePair<string, SettingsGroup> groupValue in SettingGroups) {
                    writer.WriteLine("[{0}]", groupValue.Key);
                    foreach (KeyValuePair<string, Setting> settingValue in groupValue.Value.Settings) writer.WriteLine("{0} = {1}", settingValue.Key, settingValue.Value.RawValue);
                    writer.WriteLine();
                }
            }
        }
    }
}