using System.IO;
using UnityEngine;
using UnityEditor;

namespace VRF
{

    class CuteButtons
    {

        static GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);

        static CuteButtons()
        {
            buttonStyle.fixedHeight = 60;
        }

        public static void RenderButton(string text, Texture2D icon, System.Action handler, bool disabled = false, params GUILayoutOption[] options)
        {
            if (disabled) {
                GUI.enabled = false;
            }
            
            bool pressed = GUILayout.Button(new GUIContent(text, icon), buttonStyle, options);
            
            if (disabled) {
                GUI.enabled = true;
            }

            if(pressed) {
                handler();
            }
        }
    }

}