#if VRC_SDK_VRCSDK3
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

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
        BuildInfoBuidler buildInfoBuidler = new BuildInfoBuidler();

        public void Build(SettingsBuilderData settings)
        {
            float startBuildTime = Time.realtimeSinceStartup;

            if (settings.dances.Count == 0)
            {
                EditorUtility.DisplayDialog("CuteDancer Builder", "Please select at least one dance to proceed.", "OK");
                return;
            }

            Directory.CreateDirectory(settings.outputDirectory);

            parameterBuilder.Build(settings);
            menuBuilder.Build(settings);
            musicPrefabBuilder.Build(settings);
            contactsPrefabBuilder.Build(settings);
            animFxOffBuilder.Build(settings);
            animFxOnBuilder.Build(settings);
            actionControllerBuilder.Build(settings);
            fxControllerBuilder.Build(settings);
            buildInfoBuidler.Build(settings);

            AssetDatabase.Refresh();

            SettingsService.Instance.SaveFromSettingsBuilderData(settings);

            Debug.Log($"Builder Service: CuteDancer build finished in {Time.realtimeSinceStartup - startBuildTime:0.00} seconds");
        }

        public void Rebuild(SettingsBuilderData settings)
        {
            BuildInfoData oldBuildInfo = buildInfoBuidler.GetBuildInfoData(settings.outputDirectory);
            List<BuildInfoData.FilePathGuid> oldFileInfos = oldBuildInfo?.filePathUuids;

            if (oldBuildInfo)
            {
                foreach (var fileInfo in oldBuildInfo.filePathUuids)
                {
                    Debug.Log($"Delete asset [{fileInfo.path}]");
                    AssetDatabase.DeleteAsset(fileInfo.path);
                }
            }

            Build(settings);
            
            if (oldFileInfos != null)
            {
                Debug.Log("Restoring GUIDs of previous build.");
                buildInfoBuidler.RestoreGuids(settings.outputDirectory, oldFileInfos);
            }



            // // TODO add validation to remove build files only
            // if (settings.outputDirectory != "Assets\\CuteDancer\\Build")
            // {
            //     if (!EditorUtility.DisplayDialog("Alpha version warning", "Changing build path is not recommended yet.\n\n"
            //         + "All content from the directory will be earsed before build without any validation:\n" + settings.outputDirectory + "\n\nARE YOU SURE?", "Yes", "NO!!!!!!!!!!!!!!!!"))
            //     {
            //         return;
            //     }
            // }

            // FileInfo[] files = new DirectoryInfo(settings.outputDirectory).GetFiles();
            // foreach (FileInfo file in files)
            // {
            //     if (!file.Name.StartsWith("."))
            //     {
            //         Debug.Log("Delete file [name = " + file.Name + "]");
            //         file.Delete();
            //     }
            // }

            // DirectoryInfo[] dirs = new DirectoryInfo(settings.outputDirectory).GetDirectories();
            // foreach (DirectoryInfo dir in dirs)
            // {
            //     Debug.Log("Delete directory [name = " + dir.Name + "]");
            //     dir.Delete(true);
            // }

            // AssetDatabase.Refresh();
            // Build(settings);

        }

    }
}

#endif