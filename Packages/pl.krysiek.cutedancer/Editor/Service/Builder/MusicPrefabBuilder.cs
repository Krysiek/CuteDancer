#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace VRF
{
    public class MusicPrefabBuilder : BuilderInterface
    {
        private static Logger log = new Logger("MusicPrefabBuilder");

        public void Build(SettingsBuilderData settings)
        {
            string sourcePath = Path.Combine(CuteResources.CUTEDANCER_RUNTIME, "TemplateMusic.prefab");
            string outputPath = Path.Combine(settings.outputDirectory, "CuteDancer-Music.prefab");

            if (!AssetDatabase.CopyAsset(sourcePath, outputPath))
            {
                throw new Exception("Error copying template: Music Prefab");
            }

            GameObject prefab = PrefabUtility.LoadPrefabContents(outputPath);

            Transform template = prefab.transform.GetChild(0);

            foreach (DanceBuilderData dance in settings.dances)
            {
                if (dance.audio != null)
                {
                    Transform danceAudioOb = UnityEngine.Object.Instantiate(template, prefab.transform);
                    AudioSource danceAudio = danceAudioOb.GetComponent<AudioSource>();
                    danceAudio.clip = dance.audio;
                    danceAudioOb.name = template.name.Replace("{DANCE}", dance._name);
                }
            }

            template.parent = null;

            log.LogInfo("Save file [name = " + outputPath + "]");
            PrefabUtility.SaveAsPrefabAsset(prefab, outputPath);
            PrefabUtility.UnloadPrefabContents(prefab);
        }
    }
}
#endif