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
            RefreshBtn,
            BuildBtn,
            RebuildBtn,
            BrowseBtn
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
            builderViewData.outputDirectory = SettingsService.Instance.outputDirectory;

            CuteResources.LoadView("BuilderView").CloneTree(this);
            this.Bind(new SerializedObject(builderViewData));

            this.Q("DancesList").Add(dancesBrowserView.GetEl());

            RegisterButtonClick(Buttons.SelectAllBtn, e => ToggleSelectedDances());
            RegisterButtonClick(Buttons.RefreshBtn, e => LoadDances());
            RegisterButtonClick(Buttons.BrowseBtn, e => BrowseOutputDirectory());
            RegisterButtonClick(Buttons.BuildBtn, e => builderService.Build(builderViewData));
            RegisterButtonClick(Buttons.RebuildBtn, e => builderService.Rebuild(builderViewData));

            LoadDances();

            this.RegisterCallback<ChangeEvent<bool>>((changeEvent) => Validate());
            this.RegisterCallback<ChangeEvent<string>>((changeEvent) => Validate());

            Validate();
        }

        private void LoadDances()
        {
            builderViewData.dances = dancesLoaderService.LoadDances();
            dancesBrowserView.Collections = builderViewData.dances;
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

        private void BrowseOutputDirectory()
        {
            string path = EditorUtility.OpenFolderPanel("Browse output directory", Path.GetDirectoryName(builderViewData.outputDirectory), "");
            if (path.Contains("Assets"))
            {
                log.LogDebug($"Selected directory {Application.dataPath}");
                builderViewData.outputDirectory = path.Substring(path.IndexOf("Assets"));
            }
            else if (path != "")
            {
                log.LogError("Selected directory must be within Assets directory!");
            }
        }

        public void Validate()
        {
            ShowButton(Buttons.BuildBtn, !Directory.Exists(builderViewData.outputDirectory));
            ShowButton(Buttons.RebuildBtn, Directory.Exists(builderViewData.outputDirectory));
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