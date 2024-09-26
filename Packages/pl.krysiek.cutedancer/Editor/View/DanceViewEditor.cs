#if VRC_SDK_VRCSDK3
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace VRF
{
    public class DanceViewEditor : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<DanceViewEditor, UxmlTraits> { }

        private DanceViewData _danceViewData;
        public DanceViewData DanceViewData
        {
            get => _danceViewData;
            set
            {
                _danceViewData = value;
                DrawGui();
            }
        }

        public DanceViewEditor()
        {
            VisualTreeAsset danceView = CuteResources.LoadView("DanceView");
            danceView.CloneTree(this);
            this.Q<Toggle>("AudioToggle").RegisterCallback<ChangeEvent<bool>>(ev => DrawMusicBtn());
        }

        private void DrawGui()
        {
            this.Bind(new SerializedObject(_danceViewData));

            if (_danceViewData.icon != null)
            {
                this.Q<Image>("Icon").image = _danceViewData.icon;
            }
            else
            {
                this.Q<Image>("Icon").style.width = 12;
            }
            this.Q<Button>("DanceItemBtn").clickable = new Clickable((ev) => ToggleSelection());

            Button musicBtn = this.Q<Button>("MusicBtn");
            DrawMusicBtn();
            if (_danceViewData.audio == null)
            {
                musicBtn.style.display = DisplayStyle.None;
            }
            else
            {
                musicBtn.clickable = new Clickable((ev) =>
                {
                    _danceViewData.audioEnabled = !_danceViewData.audioEnabled;
                    DrawMusicBtn();
                    SettingsService.Instance.SaveFromDanceViewData(_danceViewData);
                });
            }
        }

        private void DrawMusicBtn()
        {
            Button musicBtn = this.Q<Button>("MusicBtn");

            if (_danceViewData.audioEnabled)
            {
                musicBtn.AddToClassList("music-on");
                musicBtn.RemoveFromClassList("music-off");
            }
            else
            {
                musicBtn.AddToClassList("music-off");
                musicBtn.RemoveFromClassList("music-on");
            }
        }

        private void ToggleSelection()
        {
            _danceViewData.selected = !_danceViewData.selected;
            SettingsService.Instance.SaveFromDanceViewData(_danceViewData);
        }
    }
}

#endif