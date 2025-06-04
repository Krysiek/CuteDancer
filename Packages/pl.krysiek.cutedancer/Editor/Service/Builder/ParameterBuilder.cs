using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;

namespace VRF
{
    public class ParameterBuilder : BuilderInterface
    {
        private static Logger log = new Logger("ParameterBuilder");

        public void Build(SettingsBuilderData settings)
        {
            string sourcePath = Path.Combine(CuteResources.CUTEDANCER_RUNTIME, "TemplateVRCParams.asset");
            string outputPath = Path.Combine(settings.outputDirectory, "CuteDancer-VRCParams.asset");

            if (!AssetDatabase.CopyAsset(sourcePath, outputPath))
            {
                throw new Exception("Error copying template: VRCParams");
            }

            ExpressionParameters expressionParameters = AssetDatabase.LoadAssetAtPath<ExpressionParameters>(outputPath);

            foreach (ExpressionParameters.Parameter parameter in expressionParameters.parameters)
            {
                if (parameter.name.Contains("{PARAM}"))
                {
                    parameter.name = parameter.name.Replace("{PARAM}", settings.parameterName);
                }
            }

            log.LogInfo("Save file [name = " + outputPath + "]");
            EditorUtility.SetDirty(expressionParameters);
            AssetDatabase.SaveAssets();
        }

    }
}
