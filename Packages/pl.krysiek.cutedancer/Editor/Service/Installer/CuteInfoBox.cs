using UnityEngine;
using UnityEditor;

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