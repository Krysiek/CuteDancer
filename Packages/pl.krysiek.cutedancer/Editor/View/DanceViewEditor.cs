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
    public class DanceViewEditor
    {
        public VisualElement Create(DanceData data)
        {
            VisualTreeAsset danceView = CuteResources.LoadView("DanceView");
            VisualElement danceViewEl = danceView.CloneTree();

            danceViewEl.Bind(new SerializedObject(data));

            return danceViewEl;

        }

    }
}

#endif