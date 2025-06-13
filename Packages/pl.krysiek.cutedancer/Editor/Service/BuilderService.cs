using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace VRF
{
    public class BuilderService
    {
        private static Logger log = new Logger("BuilderService");

        ParameterBuilder parameterBuilder = new();
        MenuBuilder menuBuilder = new();
        PrefabBuilder contactsPrefabBuilder = new();
        AnimFxOffBuilder animFxOffBuilder = new();
        AnimFxOnBuilder animFxOnBuilder = new();
        ActionControllerBuilder actionControllerBuilder = new();
        FxControllerBuilder fxControllerBuilder = new();
        BuildInfoBuilder buildInfoBuilder = new();
        FuryComponentBuilder furyComponentBuilder = new();

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
            contactsPrefabBuilder.Build(settings);
            animFxOffBuilder.Build(settings);
            animFxOnBuilder.Build(settings);
            actionControllerBuilder.Build(settings);
            fxControllerBuilder.Build(settings);
            furyComponentBuilder.Build(settings);
            buildInfoBuilder.Build(settings);

            AssetDatabase.Refresh();

            SettingsService.Instance.SaveFromSettingsBuilderData(settings);

            log.LogInfo($"Build finished in {Time.realtimeSinceStartup - startBuildTime:0.00} seconds");
        }

        public void Rebuild(SettingsBuilderData settings)
        {
            BuildInfoData oldBuildInfo = buildInfoBuilder.GetBuildInfoData(settings.outputDirectory);
            List<BuildInfoData.FilePathGuid> oldFileInfos = oldBuildInfo?.restoreGuids;

            if (oldBuildInfo)
            {
                string[] metasPaths = Directory.GetFiles(settings.outputDirectory, "*.meta", SearchOption.TopDirectoryOnly);
                string[] assetsPaths = Array.ConvertAll(metasPaths, meta => Path.ChangeExtension(meta, null));
                foreach (string assetPath in assetsPaths)
                {
                    log.LogDebug($"Delete asset [{assetPath}]");
                    AssetDatabase.DeleteAsset(assetPath);
                }
            }

            Build(settings);

            if (oldFileInfos != null)
            {
                log.LogDebug("Restoring GUIDs of previous build.");
                buildInfoBuilder.RestoreGuids(settings.outputDirectory, oldFileInfos);
            }
        }
    }
}
