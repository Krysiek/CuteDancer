#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using VRC.SDK3.Avatars.ScriptableObjects;
using UnityEditor.Animations;
using System.Linq;


namespace VRF
{
    public class MainViewEditor
    {
        public enum Buttons
        {
            SelectAllBtn,
            RefreshBtn,
            BuildBtn,
            RebuildBtn,
            AvatarApplyBtn,
            AvatarRemoveBtn,
            AvatarUpdateBtn
        }

        private readonly DancesLoaderService dancesLoaderService = new DancesLoaderService();
        private readonly BuilderService builderService = new BuilderService();
        private readonly AvatarApplyService avatarApplyService = new AvatarApplyService();
        private readonly SettingsService settingsService = SettingsService.Instance;

        private readonly DancesListViewEditor dancesBrowserView = new DancesListViewEditor();

        private readonly MainViewData mainViewData;
        private readonly VisualElement mainViewEl;

        public MainViewEditor()
        {
            mainViewData = ScriptableObject.CreateInstance<MainViewData>();
            mainViewData.parameterName = settingsService.parameterName;
            mainViewData.parameterStartValue = settingsService.parameterStartValue;
            mainViewData.outputDirectory = settingsService.outputDirectory;

            mainViewEl = CuteResources.LoadView("MainView").CloneTree();
            mainViewEl.Bind(new SerializedObject(mainViewData));

            mainViewEl.Q("DancesList").Add(dancesBrowserView.GetEl());

            RegisterButtonClick(Buttons.SelectAllBtn, e => ToggleSelectedDances());
            RegisterButtonClick(Buttons.RefreshBtn, e => LoadDances());
            RegisterButtonClick(Buttons.BuildBtn, e => builderService.Build(mainViewData));
            RegisterButtonClick(Buttons.RebuildBtn, e => builderService.Rebuild(mainViewData));

            LoadDances();

            // fix what cannot be done in UXML (at least in Unity 2019)
            mainViewEl.Q<ObjectField>("Avatar").objectType = typeof(AvatarDescriptor);
            mainViewEl.Q<ObjectField>("AvatarGameObject").objectType = typeof(GameObject);
            mainViewEl.Q<ObjectField>("AvatarExpressionParameters").objectType = typeof(VRCExpressionParameters);
            mainViewEl.Q<ObjectField>("AvatarExpressionsMenu").objectType = typeof(VRCExpressionsMenu);
            mainViewEl.Q<ObjectField>("AvatarActionController").objectType = typeof(AnimatorController);
            mainViewEl.Q<ObjectField>("AvatarFxController").objectType = typeof(AnimatorController);

            mainViewEl.Q<ObjectField>("Avatar").RegisterValueChangedCallback(HandleAvatarSelect);

            RegisterButtonClick(Buttons.AvatarApplyBtn, e => avatarApplyService.AddToAvatar());
            RegisterButtonClick(Buttons.AvatarRemoveBtn, e => avatarApplyService.RemoveFromAvatar());
            RegisterButtonClick(Buttons.AvatarUpdateBtn, e => Debug.Log("not implemented"));
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

        private void ToggleSelectedDances()
        {
            bool allSelected = mainViewData.dances.All(dances => dances.Value.All(dance => dance.selected));
            bool noneSelected = mainViewData.dances.All(dances => dances.Value.All(dance => !dance.selected));
            foreach (var danceCollection in mainViewData.dances.Values)
            {
                foreach (var dance in danceCollection)
                {
                    dance.selected = noneSelected || !allSelected;
                }
            }
            settingsService.SaveFromSelectedDances(mainViewData.dances);
        }

        public void Validate()
        {
            ShowButton(Buttons.BuildBtn, !Directory.Exists(mainViewData.outputDirectory));
            ShowButton(Buttons.RebuildBtn, Directory.Exists(mainViewData.outputDirectory));

            // TODO do complex validation for the avatar
            if (avatarApplyService.Validate())
            {
                ShowButton(Buttons.AvatarApplyBtn, false, mainViewData.avatar);
                ShowButton(Buttons.AvatarRemoveBtn, true, mainViewData.avatar);
            }
            else
            {
                ShowButton(Buttons.AvatarApplyBtn, true, mainViewData.avatar);
                ShowButton(Buttons.AvatarRemoveBtn, false, mainViewData.avatar);
            }
            ShowButton(Buttons.AvatarUpdateBtn, false);
        }

        private void RegisterButtonClick(Buttons btn, Action<EventBase> action)
        {
            mainViewEl.Q<Button>(btn.ToString()).clickable = new Clickable(action);
        }

        private void ShowButton(Buttons btn, bool show, bool enabled = true)
        {
            Button button = mainViewEl.Q<Button>(btn.ToString());
            button.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            button.SetEnabled(enabled);
        }

        private void HandleAvatarSelect(ChangeEvent<UnityEngine.Object> evt)
        {
            AvatarDescriptor avatar = (AvatarDescriptor)evt.newValue;
            avatarApplyService.avatar = avatar;
            if (avatar)
            {

                mainViewEl.Q<ObjectField>("AvatarGameObject").value = avatar.gameObject;
                mainViewEl.Q<ObjectField>("AvatarExpressionParameters").value = avatar.expressionParameters;
                mainViewEl.Q<ObjectField>("AvatarExpressionsMenu").value = avatar.expressionsMenu;
                mainViewEl.Q<ObjectField>("AvatarActionController").value = Array.Find(avatar.baseAnimationLayers, layer => layer.type == AvatarDescriptor.AnimLayerType.Action).animatorController as AnimatorController;
                mainViewEl.Q<ObjectField>("AvatarFxController").value = Array.Find(avatar.baseAnimationLayers, layer => layer.type == AvatarDescriptor.AnimLayerType.FX).animatorController as AnimatorController;
            }
            else
            {
                mainViewEl.Q<ObjectField>("AvatarGameObject").value = null;
                mainViewEl.Q<ObjectField>("AvatarExpressionParameters").value = null;
                mainViewEl.Q<ObjectField>("AvatarExpressionsMenu").value = null;
                mainViewEl.Q<ObjectField>("AvatarActionController").value = null;
                mainViewEl.Q<ObjectField>("AvatarFxController").value = null;
            }
        }

    }
}

#endif