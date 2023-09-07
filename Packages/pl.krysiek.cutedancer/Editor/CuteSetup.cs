#if VRC_SDK_VRCSDK3
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace VRF
{
    public class CuteSetup : EditorWindow
    {
        [MenuItem("Tools/CuteDancer Setup")]
        static void Init()
        {
            CuteSetup window = EditorWindow.GetWindow<CuteSetup>();
            window.minSize = new Vector2(500, 600);
            window.titleContent.text = "CuteDancer Setup";
            window.Show();
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