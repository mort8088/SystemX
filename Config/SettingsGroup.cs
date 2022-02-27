// -----------------------------------------------------------------------
// <copyright file="SettingsGroup.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace SystemX.Config {
    /// <summary>
    ///     A group of settings from a configuration file.
    /// </summary>
    public class SettingsGroup {
        internal SettingsGroup(string name) {
            Name = name;
            Settings = new Dictionary<string, Setting>();
        }

        internal SettingsGroup(string name, IEnumerable<Setting> settings) {
            if (settings == null)
                throw new ArgumentNullException("settings");

            Name = name;
            Settings = new Dictionary<string, Setting>();

            foreach (Setting setting in settings)
                Settings.Add(setting.Name, setting);
        }

        /// <summary>
        ///     Gets the name of the group.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the settings found in the group.
        /// </summary>
        public Dictionary<string, Setting> Settings { get; private set; }

        /// <summary>
        ///     Indexer Property for the Settings collection
        /// </summary>
        /// <param name="index">name of the setting object</param>
        /// <returns>The requested Setting object</returns>
        public Setting this[string index] {
            get {
                return Settings[index];
            }
            set {
                Settings[index] = value;
            }
        }

        /// <summary>
        ///     Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        public void AddSetting(string name, int value) {
            if (Settings.ContainsKey(name)) Settings[name].SetValue(value);
            else {
                Setting setting = new Setting(name);
                setting.SetValue(value);
                Settings.Add(name, setting);
            }
        }

        /// <summary>
        ///     Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        public void AddSetting(string name, float value) {
            if (Settings.ContainsKey(name)) Settings[name].SetValue(value);
            else {
                Setting setting = new Setting(name);
                setting.SetValue(value);
                Settings.Add(name, setting);
            }
        }

        /// <summary>
        ///     Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        public void AddSetting(string name, bool value) {
            if (Settings.ContainsKey(name)) Settings[name].SetValue(value);
            else {
                Setting setting = new Setting(name);
                setting.SetValue(value);
                Settings.Add(name, setting);
            }
        }

        /// <summary>
        ///     Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="value">The value of the setting.</param>
        public void AddSetting(string name, string value) {
            if (Settings.ContainsKey(name)) Settings[name].SetValue(value);
            else {
                Setting setting = new Setting(name);
                setting.SetValue(value);
                Settings.Add(name, setting);
            }
        }

        /// <summary>
        ///     Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="values">The values of the setting.</param>
        public void AddSetting(string name, params int[] values) {
            if (Settings.ContainsKey(name)) Settings[name].SetValue(values);
            else {
                Setting setting = new Setting(name);
                setting.SetValue(values);
                Settings.Add(name, setting);
            }
        }

        /// <summary>
        ///     Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="values">The values of the setting.</param>
        public void AddSetting(string name, params float[] values) {
            if (Settings.ContainsKey(name)) Settings[name].SetValue(values);
            else {
                Setting setting = new Setting(name);
                setting.SetValue(values);
                Settings.Add(name, setting);
            }
        }

        /// <summary>
        ///     Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="values">The values of the setting.</param>
        public void AddSetting(string name, params bool[] values) {
            if (Settings.ContainsKey(name)) Settings[name].SetValue(values);
            else {
                Setting setting = new Setting(name);
                setting.SetValue(values);
                Settings.Add(name, setting);
            }
        }

        /// <summary>
        ///     Adds a setting to the group.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <param name="values">The values of the setting.</param>
        public void AddSetting(string name, params string[] values) {
            if (Settings.ContainsKey(name)) Settings[name].SetValue(values);
            else {
                Setting setting = new Setting(name);
                setting.SetValue(values);
                Settings.Add(name, setting);
            }
        }

        /// <summary>
        ///     Deletes a setting from the group.
        /// </summary>
        /// <param name="name">The name of the setting to delete.</param>
        public void DeleteSetting(string name) {
            Settings.Remove(name);
        }
    }
}