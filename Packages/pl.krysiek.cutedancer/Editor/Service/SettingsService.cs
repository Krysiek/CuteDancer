using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VRF
{
    public class SettingsService
    {
        private static Logger log = new Logger("SettingsService");

        private static readonly string SETTINGS_DIR = Path.Combine("ProjectSettings", "Packages", "pl.krysiek.cutedancer");
        private static readonly string SETTINGS_FILE = "settings.json";
        private static readonly string SETTINGS_FILE_PATH = Path.Combine(SETTINGS_DIR, SETTINGS_FILE);

        private SettingsService()
        {
            if (!File.Exists(SETTINGS_FILE_PATH))
            {
                EditorApplication.delayCall += LegacyVersionHelper.RunCheck;
                Directory.CreateDirectory(SETTINGS_DIR);
                Save();
            }

            try
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(SETTINGS_FILE_PATH), this);
                Logger.CurrentLevel = (Logger.LogLevel) this.logLevel;
            }
            catch
            {
                // error when cannot read the file so create the new file
                Save();
            }
        }

        private static SettingsService _instance;
        public static SettingsService Instance
        {
            get
            {
                _instance ??= new SettingsService();
                return _instance;
            }
        }

        public string[] selectedDances = DancesLoaderService.ORIGINALS_WHITELIST;
        public string[] musicDisabledDances = Array.Empty<string>();
        public string parameterName = "VRCEmote";
        public int parameterStartValue = 128;
        public string outputDirectory = Path.Combine("Assets", "CuteDancer", "Build");
        public string backupDirectory = Path.Combine("Assets", "CuteDancer", "Backup");
        public string customDancesDirectory = Path.Combine("Assets", "CuteDancer", "Dances");
        public int logLevel = 1;

        public void Save()
        {
            log.LogDebug("Settings saved");
            File.WriteAllText(SETTINGS_FILE_PATH, JsonUtility.ToJson(this, true));
        }

        public void SaveFromSettingsBuilderData(SettingsBuilderData builderData)
        {
            selectedDances = builderData.dances.ConvertAll<string>(dance => dance._name).ToArray();
            musicDisabledDances = builderData.dances.FindAll(dance => dance.audio == null).ConvertAll(dance => dance._name).ToArray();
            parameterName = builderData.parameterName;
            parameterStartValue = builderData.parameterStartValue;
            outputDirectory = builderData.outputDirectory;
        }

        public void SaveFromSelectedDances(Dictionary<string, List<DanceViewData>> dances)
        {
            List<DanceViewData> dancesList = dances.Values
                .SelectMany(dc => dc)
                .ToList();

            selectedDances = dancesList
                .FindAll(dance => dance.selected)
                .ConvertAll<string>(dance => dance._name)
                .ToArray();

            musicDisabledDances = dancesList
                .FindAll(dance => !dance.audioEnabled)
                .ConvertAll<string>(dance => dance._name)
                .ToArray();
        }

        internal void SaveFromDanceViewData(DanceViewData dance)
        {
            List<string> selectedDancesList = new List<string>(selectedDances);
            if (dance.selected)
            {
                selectedDancesList.Add(dance._name);
            }
            else
            {
                selectedDancesList.Remove(dance._name);
            }
            selectedDances = selectedDancesList.Distinct().ToArray();

            List<string> musicDisabledDancesList = new List<string>(musicDisabledDances);
            if (!dance.audioEnabled)
            {
                musicDisabledDancesList.Add(dance._name);
            }
            else
            {
                musicDisabledDancesList.Remove(dance._name);
            }
            musicDisabledDances = musicDisabledDancesList.Distinct().ToArray();
        }
    }
}