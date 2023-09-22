#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace VRF
{
    public class AnimFxOnBuilder : BuilderInterface
    {

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

                EditorCurveBinding senderBinding = Array.Find(bindings, (binding) => binding.path == "CuteDancer-Contacts/{DANCE}Sender");
                RenameBinding(animation, senderBinding, dance._name);

                EditorCurveBinding musicBinding = Array.Find(bindings, (binding) => binding.path == "CuteDancer-Music/{DANCE}Music");
                if (dance.audio != null)
                {
                    RenameBinding(animation, musicBinding, dance._name);
                }
                else
                {
                    AnimationUtility.SetEditorCurve(animation, musicBinding, null);
                }

                Debug.Log("Save file [name = " + outputPath + "]");
                EditorUtility.SetDirty(animation);
            }

            AssetDatabase.SaveAssets();
        }

        private void RenameBinding(AnimationClip animation, EditorCurveBinding musicBinding, string danceName)
        {
            var curve = AnimationUtility.GetEditorCurve(animation, musicBinding);
            AnimationUtility.SetEditorCurve(animation, musicBinding, null);
            musicBinding.path = musicBinding.path.Replace("{DANCE}", danceName);
            AnimationUtility.SetEditorCurve(animation, musicBinding, curve);
        }
    }
}
#endif