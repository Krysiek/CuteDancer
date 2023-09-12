using System;
using UnityEditor.Animations;
using UnityEngine;

namespace VRF
{
    [Serializable]
    public class DanceViewData : ScriptableObject
    {
        public string _name;

        public string displayName;

        public string author;

        public string collection;

        public Texture2D icon;

        public AnimatorController animator;

        public AudioClip audio;

        public bool selected = false;

        public bool audioEnabled = true;

        public int order = 255;
    }
}