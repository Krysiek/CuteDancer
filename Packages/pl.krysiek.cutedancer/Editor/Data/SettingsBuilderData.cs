using System.Collections.Generic;

namespace VRF
{
    public class SettingsBuilderData
    {
        public List<DanceBuilderData> dances;

        public string parameterName;

        public int parameterStartValue;

        public string outputDirectory;

        public static implicit operator SettingsBuilderData(MainViewData data)
        {
            SettingsBuilderData builderSettingsData = new SettingsBuilderData
            {
                outputDirectory = data.outputDirectory,
                parameterName = data.parameterName,
                parameterStartValue = data.parameterStartValue,
                dances = new List<DanceBuilderData>()
            };

            foreach (KeyValuePair<string, List<DanceViewData>> entry in data.dances)
            {
                foreach (DanceViewData danceViewData in entry.Value)
                {
                    if (danceViewData.selected)
                    {
                        builderSettingsData.dances.Add(danceViewData);
                    }
                }

            }

            return builderSettingsData;
        }
    }
}