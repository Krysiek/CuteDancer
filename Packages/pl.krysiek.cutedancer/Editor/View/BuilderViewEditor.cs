#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Linq;


namespace VRF
{
    public class BuilderViewEditor : VisualElement
    {
        private static Logger log = new Logger("BuilderViewEditor");

        public enum Buttons
        {
            SelectAllBtn,
            ToggleAudioBtn,
            RefreshBtn,
            BuildBtn,
            RebuildBtn
        }

        private readonly DancesLoaderService dancesLoaderService = new DancesLoaderService();
        private readonly BuilderService builderService = new BuilderService();
        private readonly DancesListViewEditor dancesBrowserView = new DancesListViewEditor();
        private readonly BuilderViewData builderViewData;

        public BuilderViewEditor()
        {
            builderViewData = ScriptableObject.CreateInstance<BuilderViewData>();
            builderViewData.parameterName = SettingsService.Instance.parameterName;
            builderViewData.parameterStartValue = SettingsService.Instance.parameterStartValue;
            builderViewData.buildName = SettingsService.Instance.buildName;

            CuteResources.LoadView("BuilderView").CloneTree(this);
            this.Bind(new SerializedObject(builderViewData));

            this.Q("DancesList").Add(dancesBrowserView.GetEl());

            RegisterButtonClick(Buttons.SelectAllBtn, e => ToggleSelectedDances());
            RegisterButtonClick(Buttons.ToggleAudioBtn, e => ToggleAudio());
            RegisterButtonClick(Buttons.RefreshBtn, e => LoadDances());
            RegisterButtonClick(Buttons.BuildBtn, e => Build());
            RegisterButtonClick(Buttons.RebuildBtn, e => Rebuild());

            LoadDances();

            this.RegisterCallback<ChangeEvent<bool>>((changeEvent) => Validate());
            this.RegisterCallback<ChangeEvent<string>>((changeEvent) => Validate());
            this.Q("BuildName").RegisterCallback<ChangeEvent<string>>((changeEvent) => SaveBuildName());

            Validate();
        }

        private void LoadDances()
        {
            builderViewData.dances = dancesLoaderService.LoadDances();
            dancesBrowserView.Collections = builderViewData.dances;
        }

        private void SaveBuildName()
        {
            SettingsService.Instance.buildName = builderViewData.buildName;
        }

        private void Build()
        {
            builderService.Build(builderViewData);
            Validate();
        }
        private void Rebuild()
        {
            builderService.Rebuild(builderViewData);
            Validate();
        }

        private void ToggleSelectedDances()
        {
            bool allSelected = builderViewData.dances.All(dances => dances.Value.All(dance => dance.selected));
            bool noneSelected = builderViewData.dances.All(dances => dances.Value.All(dance => !dance.selected));
            foreach (var danceCollection in builderViewData.dances.Values)
            {
                foreach (var dance in danceCollection)
                {
                    dance.selected = noneSelected || !allSelected;
                }
            }
            SettingsService.Instance.SaveFromSelectedDances(builderViewData.dances);
        }

        private void ToggleAudio()
        {
            bool allAudioEnabled = builderViewData.dances.Where(dances => dances.Value.All(dance => dance.audio)).All(dances => dances.Value.All(dance => dance.audioEnabled));
            log.LogDebug("allAudioEnabled: " + allAudioEnabled);
            foreach (var danceCollection in builderViewData.dances.Values)
            {
                foreach (var dance in danceCollection)
                {
                    if (dance.audio)
                    {
                        if (allAudioEnabled)
                        {
                            dance.audioEnabled = false;
                        }
                        else
                        {
                            dance.audioEnabled = true;
                        }
                    }
                }
            }
            SettingsService.Instance.SaveFromSelectedDances(builderViewData.dances);
        }

        public void Validate()
        {
            string buildPath = Path.Combine(SettingsService.Instance.BuildDirectory, builderViewData.buildName);
            ShowButton(Buttons.BuildBtn, !Directory.Exists(buildPath));
            ShowButton(Buttons.RebuildBtn, Directory.Exists(buildPath));
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

    }
}

#endif