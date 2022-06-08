using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace VRF
{
    public class CuteInfoBox
    {

        public static void RenderInfoBox(Texture2D icon, string message)
        {
            GUIStyle style = new GUIStyle(EditorStyles.helpBox);
            GUIContent content = new GUIContent(message, icon);
            GUILayout.Box(content, style, GUILayout.Height(42));
        }
    }
}