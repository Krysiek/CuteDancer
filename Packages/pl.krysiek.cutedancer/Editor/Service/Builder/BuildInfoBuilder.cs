
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VRF
{
    public class BuildInfoBuilder : BuilderInterface
    {
        static string INFO_FILENAME = "CuteDancer-BuildInfo.asset";

        public void Build(SettingsBuilderData settings)
        {
            BuildInfoData buildInfoData = ScriptableObject.CreateInstance<BuildInfoData>();
            buildInfoData.BuildDate = DateTime.Now;

            AssetDatabase.CreateAsset(buildInfoData, Path.Combine(settings.outputDirectory, INFO_FILENAME));

            string[] metas = Directory.GetFiles(settings.outputDirectory, "*.meta", SearchOption.TopDirectoryOnly);

            foreach (string meta in metas)
            {
                buildInfoData.filePathUuids.Add(new BuildInfoData.FilePathGuid()
                {
                    path = Path.ChangeExtension(meta, null),
                    guid = AssetDatabase.AssetPathToGUID(Path.ChangeExtension(meta, null))
                });

            }

            EditorUtility.SetDirty(buildInfoData);
            AssetDatabase.SaveAssets();
        }

        public BuildInfoData GetBuildInfoData(string outputDirectory)
        {
            return AssetDatabase.LoadAssetAtPath<BuildInfoData>(Path.Combine(outputDirectory, INFO_FILENAME));
        }

        internal void RestoreGuids(string outputDirectory, List<BuildInfoData.FilePathGuid> oldBuildFiles)
        {
            BuildInfoData newBuildInfo = AssetDatabase.LoadAssetAtPath<BuildInfoData>(Path.Combine(outputDirectory, INFO_FILENAME));

            foreach (var fileInfo in newBuildInfo.filePathUuids)
            {
                var oldFileInfo = oldBuildFiles.Find(fi => fi.path == fileInfo.path);

                if (oldFileInfo != null && !oldFileInfo.path.Contains("VRCMenu-more"))
                {
                    string metaPath = fileInfo.path + ".meta";
                    string metaContent = File.ReadAllText(metaPath);
                    metaContent = metaContent.Replace(fileInfo.guid, oldFileInfo.guid);
                    File.WriteAllText(metaPath, metaContent);
                    fileInfo.guid = oldFileInfo.guid;
                }
            }

            EditorUtility.SetDirty(newBuildInfo);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}