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
        MusicPrefabBuilder musicPrefabBuilder = new MusicPrefabBuilder();
        ContactsPrefabBuilder contactsPrefabBuilder = new ContactsPrefabBuilder();
        AnimFxOffBuilder animFxOffBuilder = new AnimFxOffBuilder();
        AnimFxOnBuilder animFxOnBuilder = new AnimFxOnBuilder();
        ActionControllerBuilder actionControllerBuilder = new ActionControllerBuilder();
        FxControllerBuilder fxControllerBuilder = new FxControllerBuilder();

        public void Build(SettingsBuilderData settings)
        {
            float startBuildTime = Time.realtimeSinceStartup;

            if (settings.dances.Count == 0)
            {
                EditorUtility.DisplayDialog("CuteDancer Builder", "Please select at least one dance to proceed.", "OK");
                return;
            }

            Directory.CreateDirectory(settings.outputDirectory);

            parameterBuilder.BuildParameters(settings);
            menuBuilder.BuildMenu(settings);
            musicPrefabBuilder.BuildMusicPrefab(settings);
            contactsPrefabBuilder.BuildContactsPrefab(settings);
            animFxOffBuilder.BuildAnimFxOff(settings);
            animFxOnBuilder.BuildAnimFxOn(settings);
            actionControllerBuilder.BuildActionController(settings);
            fxControllerBuilder.BuildFxController(settings);

            AssetDatabase.Refresh();
            
            SettingsService.Instance.SaveFromSettingsBuilderData(settings);

            Debug.Log($"Builder Service: CuteDancer build finished in {Time.realtimeSinceStartup - startBuildTime:0.00} seconds");
        }

        public void Rebuild(SettingsBuilderData settings)
        {
            // TODO add validation to remove build files only
            if (settings.outputDirectory != "Assets\\CuteDancer\\Build")
            {
                if (!EditorUtility.DisplayDialog("Alpha version warning", "Changing build path is not recommended yet.\n\n"
                    + "All content from the directory will be earsed before build without any validation:\n" + settings.outputDirectory + "\n\nARE YOU SURE?", "Yes", "NO!!!!!!!!!!!!!!!!"))
                {
                    return;
                }
            }

            FileInfo[] files = new DirectoryInfo(settings.outputDirectory).GetFiles();
            foreach (FileInfo file in files)
            {
                if (!file.Name.StartsWith("."))
                {
                    Debug.Log("Delete file [name = " + file.Name + "]");
                    file.Delete();
                }
            }

            DirectoryInfo[] dirs = new DirectoryInfo(settings.outputDirectory).GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                Debug.Log("Delete directory [name = " + dir.Name + "]");
                dir.Delete(true);
            }

            AssetDatabase.Refresh();
            Build(settings);
        }

    }
}

#endif