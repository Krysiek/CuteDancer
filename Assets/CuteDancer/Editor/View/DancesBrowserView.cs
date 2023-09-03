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
    public class DancesBrowserView
    {
        DanceViewEditor danceViewEditor = new DanceViewEditor();

        public VisualElement Create(Dictionary<String, List<DanceData>> collections)
        {

            VisualTreeAsset danceItemTemplate = Resources.Load<VisualTreeAsset>("DanceView");

            VisualElement dancesList = new VisualElement();
            dancesList.style.width = Length.Percent(100);
            dancesList.style.flexDirection = FlexDirection.Row;
            dancesList.style.flexWrap = Wrap.Wrap;

            foreach (KeyValuePair<string, List<DanceData>> collection in collections)
            {
                var label = new Label(collection.Key);
                label.style.width = Length.Percent(100);
                label.style.marginTop = 4;
                label.style.marginBottom = 4;
                label.style.backgroundColor = new StyleColor(new Color(.15f, .15f, .15f));
                dancesList.Add(label);
                foreach (DanceData danceItem in collection.Value)
                {
                    var danceItemEl = danceViewEditor.Create(danceItem);
                    danceItemEl.Q<Image>("Icon").image = danceItem.icon;
                    danceItemEl.Q<Button>("DanceItemBtn").clickable = new Clickable((ev) => danceItem.selected = !danceItem.selected);
                    dancesList.Add(danceItemEl);
                }
            }

            return dancesList;

        }

    }
}

#endif