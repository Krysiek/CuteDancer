using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace VRF
{
    public class CuteIcons
    {
        public static Texture2D OK = Resources.Load("circle-check-solid") as Texture2D;
        public static Texture2D WARN = Resources.Load("triangle-exclamation-solid") as Texture2D;
        public static Texture2D ERROR = Resources.Load("circle-xmark-solid") as Texture2D;
        public static Texture2D INFO = Resources.Load("circle-info-solid") as Texture2D;
        public static Texture2D ADD = Resources.Load("circle-plus-solid") as Texture2D;
        public static Texture2D REMOVE = Resources.Load("circle-minus-solid") as Texture2D;

        public static Texture2D REFRESH = Resources.Load("rotate-solid") as Texture2D;
        public static Texture2D BUILD = Resources.Load("screwdriver-wrench-solid") as Texture2D;
        public static Texture2D AVATAR = Resources.Load("person-solid") as Texture2D;

        public static Texture2D TOP_BANNER = Resources.Load("top-banner") as Texture2D;


    }
}