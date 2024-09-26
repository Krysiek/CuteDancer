using System.Collections.Generic;
using System.IO;

namespace VRF
{
    public class SettingsBuilderData
    {
        public List<DanceBuilderData> dances;

        public string parameterName;

        public int parameterStartValue;

        public string outputDirectory;

        public static implicit operator SettingsBuilderData(BuilderViewData data)
        {
            SettingsBuilderData builderSettingsData = new SettingsBuilderData
            {
                outputDirectory = Path.Combine(SettingsService.Instance.BuildDirectory, data.buildName),
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