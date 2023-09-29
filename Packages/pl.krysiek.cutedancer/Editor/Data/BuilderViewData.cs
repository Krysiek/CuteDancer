using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    public class BuilderViewData : ScriptableObject
    {
        [NonSerialized]
        public Dictionary<string, List<DanceViewData>> dances;

        public string parameterName;

        public int parameterStartValue;

        public string outputDirectory;
    }
}