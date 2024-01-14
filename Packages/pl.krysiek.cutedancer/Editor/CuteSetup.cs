#if VRC_SDK_VRCSDK3
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace VRF
{
    public class CuteSetup : EditorWindow
    {
        static DanceTemplateService danceTemplateService = new DanceTemplateService();

        [MenuItem("Tools/CuteDancer/CuteDancer Setup", false, 1)]
        public static void OpenSetupWindow()
        {
            CuteSetup window = EditorWindow.GetWindow<CuteSetup>();
            window.minSize = new Vector2(500, 600);
            window.titleContent.text = "CuteDancer Setup";
            window.Show();
        }

        [MenuItem("Tools/CuteDancer/Create Dance Template", false, 20)]
        public static void GenerateTemplate()
        {
            danceTemplateService.CreateTemplate(SettingsService.Instance.customDancesDirectory);
            EditorUtility.DisplayDialog("Create Dance Template", $"Dance template created in {SettingsService.Instance.customDancesDirectory}\n\nYou can modify it to your like.", "OK");
        }

        [MenuItem("Assets/CuteTools/Cleanup animator", true)]
        public static bool CleanupAnimatorValidator()
        {
            return Selection.activeObject.GetType().Equals(typeof(AnimatorController));
        }

        [MenuItem("Assets/CuteTools/Cleanup animator", false, 311)]
        public static void CleanupAnimator()
        {
            AnimatorControllerUtil.RemoveOrphans(Selection.activeObject as AnimatorController);
        }

        [MenuItem("Assets/CuteTools/Reserialize assets", false, 311)]
        public static void ReserializeAssets()
        {
            AssetDatabase.ForceReserializeAssets();
        }

        private MainViewEditor mainView;

        public void OnEnable()
        {
            mainView = new MainViewEditor();
            rootVisualElement.Add(mainView);
        }

        public void OnFocus()
        {
            SettingsService.Instance.Load();
        }

        public void OnLostFocus()
        {
            SettingsService.Instance.Save();
        }
    }
}
#endif