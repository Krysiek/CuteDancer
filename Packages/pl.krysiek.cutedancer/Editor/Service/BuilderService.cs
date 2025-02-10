#if VRC_SDK_VRCSDK3
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VRF
{
    public class BuilderService
    {
        private static Logger log = new Logger("BuilderService");

        ParameterBuilder parameterBuilder = new ParameterBuilder();
        MenuBuilder menuBuilder = new MenuBuilder();
        ContactsPrefabBuilder contactsPrefabBuilder = new ContactsPrefabBuilder();
        AnimFxOffBuilder animFxOffBuilder = new AnimFxOffBuilder();
        AnimFxOnBuilder animFxOnBuilder = new AnimFxOnBuilder();
        ActionControllerBuilder actionControllerBuilder = new ActionControllerBuilder();
        FxControllerBuilder fxControllerBuilder = new FxControllerBuilder();
        BuildInfoBuilder buildInfoBuilder = new BuildInfoBuilder();

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
            buildInfoBuilder.Build(settings);

            AssetDatabase.Refresh();

            SettingsService.Instance.SaveFromSettingsBuilderData(settings);

            log.LogInfo($"Build finished in {Time.realtimeSinceStartup - startBuildTime:0.00} seconds");
        }

        public void Rebuild(SettingsBuilderData settings)
        {
            BuildInfoData oldBuildInfo = buildInfoBuilder.GetBuildInfoData(settings.outputDirectory);
            List<BuildInfoData.FilePathGuid> oldFileInfos = oldBuildInfo?.filePathUuids;

            if (oldBuildInfo)
            {
                foreach (var fileInfo in oldBuildInfo.filePathUuids)
                {
                    log.LogDebug($"Delete asset [{fileInfo.path}]");
                    AssetDatabase.DeleteAsset(fileInfo.path);
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

#endif