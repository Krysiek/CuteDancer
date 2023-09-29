#if VRC_SDK_VRCSDK3
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace VRF
{
    public class DancesListViewEditor
    {
        private readonly DanceViewEditor danceViewEditor = new DanceViewEditor();

        private readonly VisualElement dancesListEl;

        private Dictionary<string, List<DanceViewData>> _collections;
        public Dictionary<string, List<DanceViewData>> Collections
        {
            get => _collections;
            set
            {
                _collections = value;
                DrawGui();
            }
        }

        public DancesListViewEditor()
        {
            dancesListEl = new VisualElement();
            dancesListEl.style.width = Length.Percent(100);
            dancesListEl.style.flexDirection = FlexDirection.Row;
            dancesListEl.style.flexWrap = Wrap.Wrap;
        }

        public VisualElement GetEl() {
            return dancesListEl;
        }

        private void DrawGui()
        {
            dancesListEl.Clear();

            foreach (KeyValuePair<string, List<DanceViewData>> collection in _collections)
            {
                var label = new Label(collection.Key);
                label.style.width = Length.Percent(100);
                label.style.marginTop = 4;
                label.style.marginBottom = 4;
                label.style.backgroundColor = new StyleColor(new Color(.15f, .15f, .15f));
                dancesListEl.Add(label);
                foreach (DanceViewData danceItem in collection.Value)
                {
                    DanceViewEditor danceViewEditor = new DanceViewEditor();
                    danceViewEditor.DanceViewData = danceItem;
                    dancesListEl.Add(danceViewEditor);
                }
            }
        }

    }
}

#endif