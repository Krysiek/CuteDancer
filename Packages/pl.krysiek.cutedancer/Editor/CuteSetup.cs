#if VRC_SDK_VRCSDK3
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace VRF
{
    public class CuteSetup : EditorWindow
    {
        static SettingsService settingsService = SettingsService.Instance;
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
            danceTemplateService.CreateTemplate(settingsService.customDancesDirectory);
            EditorUtility.DisplayDialog("Create Dance Template", $"Dance template created in {settingsService.customDancesDirectory}\n\nYou can modify it to your like.", "OK");
        }

        private MainViewEditor mainView;

        public void OnEnable()
        {
            VisualElement root = rootVisualElement;
            mainView = new MainViewEditor();
            root.Add(mainView);
        }

        public void OnLostFocus()
        {
            // TODO bind to data change
            Debug.Log("CuteDancer: settings saved");
            settingsService.Save();
        }
    }
}
#endif