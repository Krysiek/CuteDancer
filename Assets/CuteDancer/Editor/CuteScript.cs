#if VRC_SDK_VRCSDK3
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.UIElements;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using UnityEditor.UIElements;

namespace VRF
{
    public class CuteScript : EditorWindow
    {
        DancesLoader dancesLoader = new DancesLoader();
        CuteBuilder cuteBuilder = new CuteBuilder();

        [MenuItem("Tools/CuteDancer Setup")]
        static void Init()
        {
            CuteScript window = EditorWindow.GetWindow<CuteScript>();
            window.minSize = new Vector2(500, 400);
            window.titleContent.text = "CuteDancer Script";
            window.Show();
        }

        MainViewEditor mainView = new MainViewEditor();
        DancesBrowserView dancesBrowserView = new DancesBrowserView();

        VisualElement dancesBrowserContainer;

        public void OnEnable()
        {
            VisualElement root = rootVisualElement;
            var mainViewEl = mainView.Create();
            root.Add(mainViewEl);


            mainView.RegisterButtonClick(MainViewEditor.Buttons.RefreshBtn, e => RenderDancesList());
            mainView.RegisterButtonClick(MainViewEditor.Buttons.BuildBtn, e => MakeBuild());
            mainView.RegisterButtonClick(MainViewEditor.Buttons.RebuildBtn, e => RemakeBuild());
            mainView.RegisterButtonClick(MainViewEditor.Buttons.AvatarApplyBtn, e => Debug.Log("Avatar apply"));
            mainView.RegisterButtonClick(MainViewEditor.Buttons.AvatarRemoveBtn, e => Debug.Log("Avatar remove"));
            mainView.RegisterButtonClick(MainViewEditor.Buttons.AvatarUpdateBtn, e => Debug.Log("Avatar update"));

            dancesBrowserContainer = mainViewEl.Q("DancesList");
            RenderDancesList();

            mainView.Validate();
        }

        void OnGUI()
        {
            mainView.Validate();
            Debug.Log(JsonUtility.ToJson(mainView.data));
        }

        private void RenderDancesList()
        {
            dancesLoader.LoadDances();
            dancesBrowserContainer.Clear();
            var dancesBrowserEl = dancesBrowserView.Create(dancesLoader.collections);
            dancesBrowserContainer.Add(dancesBrowserEl);
        }

        private void MakeBuild()
        {
            cuteBuilder.MakeBuild(mainView.data);
        }

        private void RemakeBuild()
        {
            cuteBuilder.ClearBuild(mainView.data);
            cuteBuilder.MakeBuild(mainView.data);
        }
    }
}

#endif