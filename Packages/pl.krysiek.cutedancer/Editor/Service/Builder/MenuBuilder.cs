using System;
using System.IO;
using UnityEditor;
using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;

namespace VRF
{
    public class MenuBuilder : BuilderInterface
    {
        private static Logger log = new Logger("MenuBuilder");

        public void Build(SettingsBuilderData settings)
        {
            string sourcePath = Path.Combine(CuteResources.CUTEDANCER_RUNTIME, "TemplateVRCMenu.asset");
            string outputPath = Path.Combine(settings.outputDirectory, "CuteDancer-VRCMenu.asset");

            if (!AssetDatabase.CopyAsset(sourcePath, outputPath))
            {
                throw new Exception("Error copying template: VRCMenu");
            }

            ExpressionsMenu expressionsMenu = AssetDatabase.LoadAssetAtPath<ExpressionsMenu>(outputPath);

            int paramValue = settings.parameterStartValue;
            int submenuCount = 1;

            foreach (DanceBuilderData dance in settings.dances)
            {
                if (expressionsMenu.controls.Count >= 8)
                {
                    var entryToBeMoved = expressionsMenu.controls[7];

                    string oldOutputPath = outputPath;
                    outputPath = Path.Combine(settings.outputDirectory, "CuteDancer-VRCMenu-more" + submenuCount + ".asset");
                    submenuCount++;

                    AssetDatabase.CopyAsset(sourcePath, outputPath);
                    ExpressionsMenu oldExpressionsMenu = expressionsMenu;
                    expressionsMenu = AssetDatabase.LoadAssetAtPath<ExpressionsMenu>(outputPath);

                    expressionsMenu.controls[0] = entryToBeMoved;

                    oldExpressionsMenu.controls[7] = new ExpressionsMenu.Control()
                    {
                        name = "More...",
                        type = ExpressionsMenu.Control.ControlType.SubMenu,
                        subMenu = expressionsMenu,
                    };

                    log.LogInfo("Save file [name = " + oldOutputPath + "]");
                    EditorUtility.SetDirty(oldExpressionsMenu);
                }

                var menuEntry = new ExpressionsMenu.Control
                {
                    name = dance.displayName,
                    icon = dance.icon,
                    type = ExpressionsMenu.Control.ControlType.Toggle,
                    parameter = new ExpressionsMenu.Control.Parameter()
                    {
                        name = settings.parameterName
                    },
                    value = paramValue++
                };

                expressionsMenu.controls.Add(menuEntry);
            }

            log.LogInfo("Save file [name = " + outputPath + "]");
            EditorUtility.SetDirty(expressionsMenu);
            AssetDatabase.SaveAssets();
        }
    }
}
