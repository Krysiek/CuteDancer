#if VRC_SDK_VRCSDK3
using UnityEngine;

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace VRF
{
    public class DanceViewEditor
    {
        private readonly VisualElement danceViewEl;

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
            danceViewEl = danceView.CloneTree();
        }

        public VisualElement GetEl()
        {
            return danceViewEl;
        }

        private void DrawGui()
        {
            danceViewEl.Bind(new SerializedObject(_danceViewData));

            if (_danceViewData.icon != null)
            {
                danceViewEl.Q<Image>("Icon").image = _danceViewData.icon;
            } else {
                danceViewEl.Q<Image>("Icon").style.width = 12;
            }
            danceViewEl.Q<Button>("DanceItemBtn").clickable = new Clickable((ev) => _danceViewData.selected = !_danceViewData.selected);

            Button musicBtn = danceViewEl.Q<Button>("MusicBtn");
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
                });
            }
        }

        private void DrawMusicBtn()
        {
            Button musicBtn = danceViewEl.Q<Button>("MusicBtn");

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
            Debug.Log(JsonUtility.ToJson(_danceViewData));
        }
    }
}

#endif