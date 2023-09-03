using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    public class SettingsData : ScriptableObject
    {
        public List<string> selectedDances = new List<string>();
        
        public string parameterName = "VRCEmote";
        
        public int parameterStartValue = 128;
        
        public string outputDirectory = "Assets\\CuteDancerGenerated";
        
        public AvatarDescriptor avatar;
    }
}