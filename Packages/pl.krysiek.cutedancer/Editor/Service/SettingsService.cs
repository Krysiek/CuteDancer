using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;
using UnityEngine;

namespace VRF
{
    public class SettingsService
    {
        private static readonly string SETTINGS_DIR = Path.Combine("ProjectSettings", "Packages", "pl.krysiek.cutedancer");
        private static readonly string SETTINGS_FILE = "settings.json";
        private static readonly string SETTINGS_FILE_PATH = Path.Combine(SETTINGS_DIR, SETTINGS_FILE);

        private SettingsService()
        {
            if (!File.Exists(SETTINGS_FILE_PATH))
            {
                Directory.CreateDirectory(SETTINGS_DIR);
                Save();
            }

            try
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(SETTINGS_FILE_PATH), this);
            }
            catch
            {
                Save();
            }
        }

        private static SettingsService _instance;
        public static SettingsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingsService();
                }
                return _instance;
            }
        }

        public string[] selectedDances = new string[] { "SARDefaultDance", "BadgerDance" };
        public string[] musicDisabledDances = Array.Empty<string>();
        public string parameterName = "VRCEmote";
        public int parameterStartValue = 128;
        public string outputDirectory = "Assets\\CuteDancer\\Build";

        public void Save()
        {
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