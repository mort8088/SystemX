// -----------------------------------------------------------------------
// <copyright file="I_Config.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

namespace SystemX.Config {
    public interface I_Config {
        SettingsGroup this[string index] { get; set; }
        Dictionary<string, SettingsGroup> SettingGroups { get; }
        SettingsGroup AddSettingsGroup(string groupName);
        void DeleteSettingsGroup(string groupName);
        void Load(Stream stream);
        void Load(string file);
        void Save(Stream stream);
        void Save(string filename);
    }
}