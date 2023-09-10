#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using System.Collections.Generic;


namespace VRF
{
    public class MainViewEditor
    {
        public enum Buttons
        {
            RefreshBtn,
            BuildBtn,
            RebuildBtn,
            AvatarApplyBtn,
            AvatarRemoveBtn,
            AvatarUpdateBtn
        }

        private readonly DancesLoaderService dancesLoaderService = new DancesLoaderService();
        private readonly BuilderService builderService = new BuilderService();

        private readonly DancesListViewEditor dancesBrowserView = new DancesListViewEditor();

        private readonly MainViewData mainViewData;
        private readonly VisualElement mainViewEl;

        public MainViewEditor()
        {
            mainViewData = ScriptableObject.CreateInstance<MainViewData>();

            mainViewEl = CuteResources.LoadView("MainView").CloneTree();
            mainViewEl.Bind(new SerializedObject(mainViewData));

            mainViewEl.Q<ObjectField>("Avatar").objectType = typeof(AvatarDescriptor); // fix what cannot be done in UXML (at least in Unity 2019)

            mainViewEl.Q("DancesList").Add(dancesBrowserView.GetEl());

            RegisterButtonClick(Buttons.RefreshBtn, e => LoadDances());
            RegisterButtonClick(Buttons.BuildBtn, e => builderService.Build(mainViewData));
            RegisterButtonClick(Buttons.RebuildBtn, e => builderService.Rebuild(mainViewData));
            RegisterButtonClick(Buttons.AvatarApplyBtn, e => Debug.Log("Avatar apply"));
            RegisterButtonClick(Buttons.AvatarRemoveBtn, e => Debug.Log("Avatar remove"));
            RegisterButtonClick(Buttons.AvatarUpdateBtn, e => Debug.Log("Avatar update"));

            LoadDances();
        }

        public VisualElement GetViewElement()
        {
            return mainViewEl;
        }

        private void LoadDances()
        {
            mainViewData.dances = dancesLoaderService.LoadDances();
            dancesBrowserView.Collections = mainViewData.dances;
        }

        public void Validate()
        {
            ShowButton(Buttons.BuildBtn, !Directory.Exists(mainViewData.outputDirectory));
            ShowButton(Buttons.RebuildBtn, Directory.Exists(mainViewData.outputDirectory));

            // TODO do validation for avatar (below)
            ShowButton(Buttons.AvatarRemoveBtn, false);
            ShowButton(Buttons.AvatarUpdateBtn, false);
        }

        private void RegisterButtonClick(Buttons btn, Action<EventBase> action)
        {
            mainViewEl.Q<Button>(btn.ToString()).clickable = new Clickable(action);
        }

        private void ShowButton(Buttons btn, bool show)
        {
            mainViewEl.Q<Button>(btn.ToString()).style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }

    }
}

#endif