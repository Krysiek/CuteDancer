#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Animations;
using UnityEngine.UIElements;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;


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

        public SettingsData data;

        VisualElement mainViewEl;

        public VisualElement Create()
        {
            data = ScriptableObject.CreateInstance<SettingsData>();

            VisualTreeAsset mainView = Resources.Load<VisualTreeAsset>("MainView");
            mainViewEl = mainView.CloneTree();

            var serializedData = new SerializedObject(data);
            mainViewEl.Bind(serializedData);

            mainViewEl.Q<ObjectField>("Avatar").objectType = typeof(AvatarDescriptor);

            return mainViewEl;

        }

        public void Validate()
        {
            // todo :0
            ShowButton(Buttons.BuildBtn, !Directory.Exists(data.outputDirectory));
            ShowButton(Buttons.RebuildBtn, Directory.Exists(data.outputDirectory));
            ShowButton(Buttons.AvatarRemoveBtn, false);
            ShowButton(Buttons.AvatarUpdateBtn, false);
        }

        public void RegisterButtonClick(Buttons btn, Action<EventBase> action)
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