#if VRC_SDK_VRCSDK3
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace VRF
{
    public class OwOScript : EditorWindow
    {
        CuteLayers cuteLayers = new CuteLayers();

        [MenuItem("Tools/CuteDancer Setup")]
        static void Init()
        {
            OwOScript window = (OwOScript)EditorWindow.GetWindow(typeof(OwOScript));
            window.titleContent.text = "CuteDancer Script";
            window.Show();
        }

        void OnGUI()
        {
            GUIStyle titleStyle = new GUIStyle();
            titleStyle.fontSize = 20;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = EditorStyles.boldLabel.normal.textColor;
            titleStyle.margin.top = 15;
            titleStyle.margin.bottom = 15;

            GUIStyle labelStyle = new GUIStyle(EditorStyles.largeLabel);
            labelStyle.wordWrap = true;
            labelStyle.margin.top = 20;

            GUILayout.Label("CuteDancer Setup", titleStyle);

            cuteLayers.RenderGUI(labelStyle);
           

            // GUILayout.Label("Select your main avatar object from scene.", labelStyle);
            // avatar = EditorGUILayout.ObjectField("Avatar", avatar, typeof(GameObject), true) as GameObject;

            // GUILayout.Label("Select expressions parameters used by your avatar", labelStyle);
            // todo
        }

        
    }
}

#endif