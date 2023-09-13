#if VRC_SDK_VRCSDK3
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace VRF
{
    public class CuteSetup : EditorWindow
    {
        [MenuItem("Tools/CuteDancer/CuteDancer Setup", false, 1)]
        static void OpenSetupWindow()
        {
            CuteSetup window = EditorWindow.GetWindow<CuteSetup>();
            window.minSize = new Vector2(500, 600);
            window.titleContent.text = "CuteDancer Setup";
            window.Show();
        }

        
        [MenuItem("Tools/CuteDancer/Create Dance Template", false, 20)]
        static void GenerateTemplate()
        {
            EditorUtility.DisplayDialog("Create Dance Template", "Not implemented yet.", "OK");
            // TODO implement template generation with unique GUID
            // EditorUtility.DisplayDialog("Create Dance Template", "Dance template created in Assets/CuteDancer/Dances/DanceTemplate\n\nYou can modify it to your like.", "OK");
        }

        private MainViewEditor mainView;

        public void OnEnable()
        {
            mainView = new MainViewEditor();
            VisualElement root = rootVisualElement;
            var mainViewEl = mainView.GetViewElement();
            root.Add(mainViewEl);
            mainView.Validate();
        }

        void OnGUI()
        {
            mainView.Validate();
        }
    }
}
#endif