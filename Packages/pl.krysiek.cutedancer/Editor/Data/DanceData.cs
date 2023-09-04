using System;
using UnityEngine;

namespace VRF
{
    [Serializable]
    public class DanceData : ScriptableObject
    {
        public string _name;

        public string displayName;

        public string author;

        public string collection;

        public Texture2D icon;

        public Animator animator;

        public AudioClip audio;

        public bool selected;

        public int order = 255;
    }
}