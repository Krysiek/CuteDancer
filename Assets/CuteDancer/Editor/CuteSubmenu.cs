using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;

namespace VRF
{
    public class CuteSubmenu : CuteGroup
    {
        static string CUTE_MENU = "Assets/CuteDancer/VRCMenu_CuteDancer.asset";
        static string DANCE_ICON = "Assets/VRCSDK/Examples3/Expressions Menu/Icons/person_dance.png"; // TODO use custom icon

        enum Status
        {
            FORM, EMPTY, ADDED, FULL
        }

        ExpressionsMenu expressionMenu;

        public void RenderForm()
        {

            GUIStyle labelStyle = new GUIStyle(EditorStyles.largeLabel);
            labelStyle.wordWrap = true;

            GUILayout.Label("Select expression menu used by your avatar", labelStyle);
            expressionMenu = EditorGUILayout.ObjectField("Expressions Menu", expressionMenu, typeof(ExpressionsMenu), false, GUILayout.ExpandWidth(true)) as ExpressionsMenu;

            GUILayout.Space(10);

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);
            buttonStyle.fixedHeight = 30;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Add expression submenu", CuteIcons.ADD), buttonStyle))
            {
                HandleAdd();
            }
            if (GUILayout.Button(new GUIContent("Remove", CuteIcons.REMOVE), buttonStyle, GUILayout.Width(150)))
            {
                HandleRemove();
            }
            GUILayout.EndHorizontal();
        }

        public void RenderStatus()
        {
            switch (Validate())
            {
                case Status.FORM:
                    CuteInfoBox.RenderInfoBox(CuteIcons.INFO, "Please select expression menu asset where CuteDancer submenu will be added.");
                    break;
                case Status.ADDED:
                    CuteInfoBox.RenderInfoBox(CuteIcons.OK, "CuteDancer submenu is added.");
                    break;
                case Status.EMPTY:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "CuteDancer submenu is not added.");
                    break;
                case Status.FULL:
                    CuteInfoBox.RenderInfoBox(CuteIcons.ERROR, "No slots available in selected expression menu.\nPlease select another menu or remove unused control from the menu.");
                    break;
            }
        }

        public void SetAvatar(AvatarDescriptor avatarDescriptor)
        {
            expressionMenu = avatarDescriptor.expressionsMenu;
        }

        public void ClearForm()
        {
            expressionMenu = null;
        }

        Status Validate()
        {
            if (expressionMenu == null)
            {
                return Status.FORM;
            }
            ExpressionsMenu cuteMenu = AssetDatabase.LoadAssetAtPath(CUTE_MENU, typeof(ExpressionsMenu)) as ExpressionsMenu;
            if (expressionMenu.controls.Exists(menuEntry => menuEntry.subMenu == cuteMenu))
            {
                return Status.ADDED;
            }
            if (expressionMenu.controls.ToArray().Length >= 8)
            {
                return Status.FULL;
            }
            return Status.EMPTY;
        }

        void HandleAdd()
        {
            switch (Validate())
            {
                case Status.ADDED:
                case Status.FORM:
                case Status.FULL:
                    EditorUtility.DisplayDialog("CuteScript", "Option disabled.", "OK");
                    return;
            }

            DoBackup();

            ExpressionsMenu cuteMenu = AssetDatabase.LoadAssetAtPath(CUTE_MENU, typeof(ExpressionsMenu)) as ExpressionsMenu;

            var menuEntry = new ExpressionsMenu.Control();

            menuEntry.name = "CuteDancer";
            menuEntry.icon = AssetDatabase.LoadAssetAtPath(DANCE_ICON, typeof(Texture2D)) as Texture2D;
            menuEntry.type = ExpressionsMenu.Control.ControlType.SubMenu;
            menuEntry.subMenu = cuteMenu;

            Debug.Log("Adding expression menu control to menu [name=" + expressionMenu.name + "]");
            expressionMenu.controls.Add(menuEntry);
            EditorUtility.SetDirty(expressionMenu);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void HandleRemove()
        {
            switch (Validate())
            {
                case Status.EMPTY:
                case Status.FORM:
                case Status.FULL:
                    EditorUtility.DisplayDialog("CuteScript", "Option disabled.", "OK");
                    return;
            }

            DoBackup();

            ExpressionsMenu cuteMenu = AssetDatabase.LoadAssetAtPath(CUTE_MENU, typeof(ExpressionsMenu)) as ExpressionsMenu;
            int ix = expressionMenu.controls.FindIndex(menuEntry => menuEntry.subMenu == cuteMenu);

            Debug.Log("Removing expression menu control from menu [name=" + expressionMenu.name + "]");
            expressionMenu.controls.RemoveAt(ix);
            EditorUtility.SetDirty(expressionMenu);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void DoBackup()
        {
            CuteBackup.CreateBackup(AssetDatabase.GetAssetPath(expressionMenu));
        }
    }
}