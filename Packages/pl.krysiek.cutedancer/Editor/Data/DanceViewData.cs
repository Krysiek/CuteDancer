using System;
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

        public Animator animator;

        public AudioClip audio;

        public bool selected = false;

        public bool audioEnabled = true;

        public int order = 255;
    }
}