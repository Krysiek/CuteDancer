#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace VRF
{
    public class AnimFxOnBuilder : BuilderInterface
    {
        private static Logger log = new Logger("AnimFxOnBuilder");

        public void Build(SettingsBuilderData settings)
        {
            if (!Directory.Exists(Path.Combine(settings.outputDirectory, "FX")))
            {
                AssetDatabase.CreateFolder(settings.outputDirectory, "FX");
            }

            foreach (DanceBuilderData dance in settings.dances)
            {
                string sourcePath = Path.Combine(CuteResources.CUTEDANCER_RUNTIME, "TemplateDanceFX_ON.anim");
                string outputPath = Path.Combine(settings.outputDirectory, "FX", dance._name + "_FX_ON.anim");

                if (!AssetDatabase.CopyAsset(sourcePath, outputPath))
                {
                    throw new Exception("Error copying template: Dance FX ON animation");
                }

                AnimationClip animation = AssetDatabase.LoadAssetAtPath<AnimationClip>(outputPath);

                EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(animation);

                EditorCurveBinding senderBinding = Array.Find(bindings, (binding) => binding.path == "CuteDancer/Senders/{DANCE}Sender");
                RenameBinding(animation, senderBinding, dance._name);

                log.LogInfo("Save file [name = " + outputPath + "]");
                EditorUtility.SetDirty(animation);
            }

            AssetDatabase.SaveAssets();
        }

        private void RenameBinding(AnimationClip animation, EditorCurveBinding binding, string danceName)
        {
            var curve = AnimationUtility.GetEditorCurve(animation, binding);
            AnimationUtility.SetEditorCurve(animation, binding, null);
            binding.path = binding.path.Replace("{DANCE}", danceName);
            AnimationUtility.SetEditorCurve(animation, binding, curve);
        }
    }
}
#endif