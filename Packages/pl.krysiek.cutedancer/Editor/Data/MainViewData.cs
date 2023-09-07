using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    public class MainViewData : ScriptableObject
    {
        [NonSerialized]
        public Dictionary<string, List<DanceViewData>> dances;

        public string parameterName = "VRCEmote";

        public int parameterStartValue = 128;

        public string outputDirectory = "Assets\\CuteDancer\\Build";

        public AvatarDescriptor avatar;
    }
}