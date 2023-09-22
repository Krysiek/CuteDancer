#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace VRF
{
    public class AnimFxOffBuilder : BuilderInterface
    {

        public void Build(SettingsBuilderData settings)
        {
            if (!Directory.Exists(Path.Combine(settings.outputDirectory, "FX")))
            {
                AssetDatabase.CreateFolder(settings.outputDirectory, "FX");
            }

            string sourcePath = Path.Combine(CuteResources.CUTEDANCER_RUNTIME, "TemplateFX_OFF.anim");
            string outputPath = Path.Combine(settings.outputDirectory, "FX", "CuteDancer-FX_OFF.anim");

            if (!AssetDatabase.CopyAsset(sourcePath, outputPath))
            {
                throw new Exception("Error copying template: FX OFF animation");
            }

            AnimationClip animation = AssetDatabase.LoadAssetAtPath<AnimationClip>(outputPath);

            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(animation);

            EditorCurveBinding musicBindingTemplate = Array.Find(bindings, (binding) => binding.path == "CuteDancer-Music/{DANCE}Music");
            EditorCurveBinding senderBindingTemplate = Array.Find(bindings, (binding) => binding.path == "CuteDancer-Contacts/{DANCE}Sender");

            AnimationCurve musicCurveTemplate = AnimationUtility.GetEditorCurve(animation, musicBindingTemplate);
            AnimationCurve senderCurveTemplate = AnimationUtility.GetEditorCurve(animation, senderBindingTemplate);

            foreach (DanceBuilderData dance in settings.dances)
            {

                EditorCurveBinding senderBinding = new EditorCurveBinding
                {
                    path = senderBindingTemplate.path.Replace("{DANCE}", dance._name),
                    propertyName = senderBindingTemplate.propertyName,
                    type = senderBindingTemplate.type
                };
                AnimationUtility.SetEditorCurve(animation, senderBinding, musicCurveTemplate);

                if (dance.audio != null)
                {
                    EditorCurveBinding musicBinding = new EditorCurveBinding
                    {
                        path = musicBindingTemplate.path.Replace("{DANCE}", dance._name),
                        propertyName = musicBindingTemplate.propertyName,
                        type = musicBindingTemplate.type
                    };
                    AnimationUtility.SetEditorCurve(animation, musicBinding, musicCurveTemplate);
                }
            }

            // clear template bindings
            AnimationUtility.SetEditorCurve(animation, musicBindingTemplate, null);
            AnimationUtility.SetEditorCurve(animation, senderBindingTemplate, null);

            Debug.Log("Save file [name = " + outputPath + "]");
            EditorUtility.SetDirty(animation);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif