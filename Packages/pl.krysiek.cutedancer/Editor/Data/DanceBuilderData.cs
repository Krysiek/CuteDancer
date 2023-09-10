using System;
using UnityEngine;

namespace VRF
{
    public class DanceBuilderData
    {
        public string _name;

        public string displayName;

        public string collection;

        public Texture2D icon;

        public Animator animator;

        public AudioClip audio;

        public static implicit operator DanceBuilderData(DanceViewData data) => new DanceBuilderData()
        {
            _name = data._name,
            displayName = data.displayName,
            collection = data.collection,
            icon = data.icon,
            animator = data.animator,
            audio = data.audio,
        };

    }
}