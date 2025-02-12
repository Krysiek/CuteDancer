#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace VRF
{
    public class AnimFxOffBuilder : BuilderInterface
    {
        private static Logger log = new Logger("AnimFxOffBuilder");

        public void Build(SettingsBuilderData settings)
        {
            if (!Directory.Exists(Path.Combine(settings.outputDirectory, "FX")))
            {
                AssetDatabase.CreateFolder(settings.outputDirectory, "FX");
            }

            string sourcePath = Path.Combine(CuteResources.CUTEDANCER_RUNTIME, "TemplateDanceFX_OFF.anim");
            string outputPath = Path.Combine(settings.outputDirectory, "FX", "CuteDancer-FX_OFF.anim");

            if (!AssetDatabase.CopyAsset(sourcePath, outputPath))
            {
                throw new Exception("Error copying template: FX OFF animation");
            }

            AnimationClip animation = AssetDatabase.LoadAssetAtPath<AnimationClip>(outputPath);

            EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(animation);

            EditorCurveBinding senderBindingTemplate = Array.Find(bindings, (binding) => binding.path == "CuteDancer/Senders/{DANCE}Sender");

            AnimationCurve senderCurveTemplate = AnimationUtility.GetEditorCurve(animation, senderBindingTemplate);

            foreach (DanceBuilderData dance in settings.dances)
            {

                EditorCurveBinding senderBinding = new EditorCurveBinding
                {
                    path = senderBindingTemplate.path.Replace("{DANCE}", dance._name),
                    propertyName = senderBindingTemplate.propertyName,
                    type = senderBindingTemplate.type
                };
                AnimationUtility.SetEditorCurve(animation, senderBinding, senderCurveTemplate);
            }

            // clear template bindings
            AnimationUtility.SetEditorCurve(animation, senderBindingTemplate, null);

            log.LogInfo("Save file [name = " + outputPath + "]");
            EditorUtility.SetDirty(animation);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif