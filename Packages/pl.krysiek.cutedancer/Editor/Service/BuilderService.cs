#if VRC_SDK_VRCSDK3
using System.IO;
using UnityEngine;
using UnityEditor;

namespace VRF
{
    public class BuilderService
    {
        ParameterBuilder parameterBuilder = new ParameterBuilder();
        MenuBuilder menuBuilder = new MenuBuilder();

        public void Build(SettingsBuilderData settings)
        {
            if (settings.dances.Count == 0)
            {
                EditorUtility.DisplayDialog("CuteDancer Builder", "Please select at least one dance to proceed.", "OK");
                return;
            }

            Directory.CreateDirectory(settings.outputDirectory);

            parameterBuilder.BuildParameters(settings);
            menuBuilder.BuildMenu(settings);
            // TODO more builders

            AssetDatabase.Refresh();
        }

        public void Rebuild(SettingsBuilderData settings)
        {
            FileInfo[] files = new DirectoryInfo(settings.outputDirectory).GetFiles();
            foreach (FileInfo file in files)
            {
                if (!file.Name.StartsWith("."))
                {
                    Debug.Log("Delete file [name = " + file.Name + "]");
                    file.Delete();
                }
            }
            AssetDatabase.Refresh();
            Build(settings);
        }

    }
}

#endif