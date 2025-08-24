#if VRCFURY_API_AVAILABLE
using System.IO;
using UnityEngine;
using UnityEditor;
using com.vrcfury.api.Components;
using com.vrcfury.api;
using VRC.SDK3.Avatars.ScriptableObjects;
#endif

namespace VRF
{
    public class FuryComponentBuilder : BuilderInterface
    {
        private static Logger log = new Logger("FuryComponentBuilder");

        public void Build(SettingsBuilderData settings)
        {
#if VRCFURY_API_AVAILABLE
            string prefabPath = Path.Combine(settings.outputDirectory, "CuteDancer.prefab");
            string actionCtrlPath = Path.Combine(settings.outputDirectory, "CuteDancer-Action.controller");
            string fxCtrlPath = Path.Combine(settings.outputDirectory, "CuteDancer-FX.controller");
            string menuPath = Path.Combine(settings.outputDirectory, "CuteDancer-VRCMenu.asset");
            string paramsPath = Path.Combine(settings.outputDirectory, "CuteDancer-VRCParams.asset");

            GameObject prefab = PrefabUtility.LoadPrefabContents(prefabPath);

            FuryFullController component = FuryComponents.CreateFullController(prefab);

            component.AddController(AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(actionCtrlPath), VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.Action);
            component.AddController(AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(fxCtrlPath), VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType.FX);

            component.AddMenu(AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(menuPath), "CuteDancer");
            component.AddParams(AssetDatabase.LoadAssetAtPath<VRCExpressionParameters>(paramsPath));

            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefab);
#endif
        }
    }
}
