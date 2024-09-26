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

namespace VRF
{
    public class InstallerViewEditor : VisualElement
    {
        private static Logger log = new Logger("InstallerViewEditor");

        public enum Buttons
        {
            AvatarApplyBtn,
            AvatarRemoveBtn,
            AvatarUpdateBtn
        }

        private readonly AvatarApplyService avatarApplyService = new AvatarApplyService();

        private readonly InstallerViewData viewData;

        private readonly BuildInfoEditor buildInfoEditor;

        public InstallerViewEditor()
        {
            viewData = ScriptableObject.CreateInstance<InstallerViewData>();

            CuteResources.LoadView("InstallerView").CloneTree(this);
            this.Bind(new SerializedObject(viewData));

            // TODO check if can be done in UXML in Unity 2023
            this.Q<ObjectField>("Build").objectType = typeof(BuildInfoData);
            this.Q<ObjectField>("Avatar").objectType = typeof(AvatarDescriptor);
            this.Q<ObjectField>("AvatarGameObject").objectType = typeof(GameObject);
            this.Q<ObjectField>("AvatarExpressionParameters").objectType = typeof(VRCExpressionParameters);
            this.Q<ObjectField>("AvatarExpressionsMenu").objectType = typeof(VRCExpressionsMenu);
            this.Q<ObjectField>("AvatarActionController").objectType = typeof(AnimatorController);
            this.Q<ObjectField>("AvatarFxController").objectType = typeof(AnimatorController);

            this.Q<ObjectField>("Build").RegisterValueChangedCallback(HandleBuildSelect);
            this.Q<ObjectField>("Avatar").RegisterValueChangedCallback(HandleAvatarSelect);

            RegisterButtonClick(Buttons.AvatarApplyBtn, e => { avatarApplyService.AddToAvatar(); Validate(); });
            RegisterButtonClick(Buttons.AvatarRemoveBtn, e => { avatarApplyService.RemoveFromAvatar(); Validate(); });
            RegisterButtonClick(Buttons.AvatarUpdateBtn, e => { avatarApplyService.UpdateAvatar(); Validate(); });

            buildInfoEditor = this.Q<BuildInfoEditor>("BuildInfo");

            this.RegisterCallback<ChangeEvent<UnityEngine.Object>>((changeEvent) => Validate());
            this.RegisterCallback<AttachToPanelEvent>((attachEvent) => SelectDefaultBuild());

            Validate();
        }

        public void Validate()
        {
            if (viewData.avatar && avatarApplyService.ValidateIsAdded())
            {
                ShowButton(Buttons.AvatarApplyBtn, false, viewData.avatar);
                ShowButton(Buttons.AvatarRemoveBtn, true, viewData.avatar);
                ShowButton(Buttons.AvatarUpdateBtn, true, viewData.avatar);
            }
            else
            {
                ShowButton(Buttons.AvatarApplyBtn, true, viewData.avatar);
                ShowButton(Buttons.AvatarRemoveBtn, false, viewData.avatar);
                ShowButton(Buttons.AvatarUpdateBtn, false, viewData.avatar);
            }

            UpdateAvatarComponents();
        }

        private void SelectDefaultBuild()
        {
            string[] lastBuild = AssetDatabase.FindAssets("t:BuildInfoData", new string[] { Path.Combine(SettingsService.Instance.BuildDirectory, SettingsService.Instance.buildName) });
            if (lastBuild.Length != 0)
            {
                viewData.build = AssetDatabase.LoadAssetAtPath<BuildInfoData>(AssetDatabase.GUIDToAssetPath(lastBuild[0])); ;
            }
        }

        private void RegisterButtonClick(Buttons btn, Action<EventBase> action)
        {
            this.Q<Button>(btn.ToString()).clickable = new Clickable(action);
        }

        private void ShowButton(Buttons btn, bool show, bool enabled = true)
        {
            Button button = this.Q<Button>(btn.ToString());
            button.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
            button.SetEnabled(enabled);
        }

        private void HandleBuildSelect(ChangeEvent<UnityEngine.Object> evt)
        {
            BuildInfoData buildInfo = (BuildInfoData)evt.newValue;
            avatarApplyService.BuildInfo = buildInfo;
            buildInfoEditor.BuildInfoData = buildInfo;
        }

        private void HandleAvatarSelect(ChangeEvent<UnityEngine.Object> evt)
        {
            avatarApplyService.Avatar = (AvatarDescriptor)evt.newValue;
            UpdateAvatarComponents();
        }

        private void UpdateAvatarComponents()
        {
            AvatarDescriptor avatar = viewData.avatar;
            if (avatar)
            {
                this.Q<ObjectField>("AvatarGameObject").value = avatar.gameObject;
                this.Q<ObjectField>("AvatarExpressionParameters").value = avatar.expressionParameters;
                this.Q<ObjectField>("AvatarExpressionsMenu").value = avatar.expressionsMenu;
                this.Q<ObjectField>("AvatarActionController").value = Array.Find(avatar.baseAnimationLayers, layer => layer.type == AvatarDescriptor.AnimLayerType.Action).animatorController as AnimatorController;
                this.Q<ObjectField>("AvatarFxController").value = Array.Find(avatar.baseAnimationLayers, layer => layer.type == AvatarDescriptor.AnimLayerType.FX).animatorController as AnimatorController;
            }
            else
            {
                this.Q<ObjectField>("AvatarGameObject").value = null;
                this.Q<ObjectField>("AvatarExpressionParameters").value = null;
                this.Q<ObjectField>("AvatarExpressionsMenu").value = null;
                this.Q<ObjectField>("AvatarActionController").value = null;
                this.Q<ObjectField>("AvatarFxController").value = null;
            }
        }

    }
}

#endif