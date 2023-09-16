using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;
using System.IO;

namespace VRF
{
    public class CuteSubmenu : AvatarApplierInterface
    {
        static string CUTE_MENU = Path.Combine("Assets", "CuteDancer", "Build", "CuteDancer-VRCMenu.asset"); // TODO read from build configuration
        static string DANCE_ICON = Path.Combine("Packages", "pl.krysiek.cutedancer", "Runtime", "Icons", "CuteDancer.png");

        enum Status
        {
            FORM, EMPTY, ADDED, FULL, MISSING
        }

        Status validStat = Status.FORM;
        AvatarDescriptor avatar;
        ExpressionsMenu expressionMenu;

        public void RenderForm()
        {
            validStat = Validate();

            GUIStyle labelStyle = new GUIStyle(EditorStyles.largeLabel);
            labelStyle.wordWrap = true;

            GUILayout.Label("Select expression menu used by your avatar", labelStyle);
            expressionMenu = EditorGUILayout.ObjectField("Expressions Menu", expressionMenu, typeof(ExpressionsMenu), false, GUILayout.ExpandWidth(true)) as ExpressionsMenu;

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            CuteButtons.RenderButton("Add expression submenu", CuteIcons.ADD, HandleAdd,
                validStat == Status.FORM || validStat == Status.ADDED || validStat == Status.FULL);
            CuteButtons.RenderButton("Remove", CuteIcons.REMOVE, HandleRemove, validStat != Status.ADDED, GUILayout.Width(150));

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
                case Status.MISSING:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "CuteDancer submenu is not added (missing expression menu asset will be created).");
                    break;
                case Status.FULL:
                    CuteInfoBox.RenderInfoBox(CuteIcons.ERROR, "No slots available in selected expression menu.\nPlease select another menu or remove unused control from the menu.");
                    break;
            }
        }

        public void SetAvatar(AvatarDescriptor avatarDescriptor)
        {
            avatar = avatarDescriptor;
            expressionMenu = avatarDescriptor.expressionsMenu;
        }

        public void ClearForm()
        {
            avatar = null;
            expressionMenu = null;
        }

        Status Validate()
        {
            if (avatar == null)
            {
                return Status.FORM;
            }
            if (expressionMenu == null)
            {
                return Status.MISSING;
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

        public void HandleAdd()
        {
            if (!expressionMenu && !CreateExpressionMenu())
            {
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

        public void HandleRemove()
        {
            DoBackup();

            Texture2D cuteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DANCE_ICON);
            int ix = expressionMenu.controls.FindIndex(menuEntry => menuEntry.icon == cuteIcon);

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

        bool CreateExpressionMenu()
        {
            var path = $"Assets/{avatar.name}-ExpressionMenu.asset";
            var ok = EditorUtility.DisplayDialog("CuteScript", $"It seems your avatar does not have expression menu. Empty one will be created and assigned to your avatar.\n\nNew asset will be saved under path:\n{path}", "Create it!", "Cancel");
            if (!ok)
            {
                EditorUtility.DisplayDialog("CuteScript", "Operation aborted. Expresion Menu is NOT added!", "OK");
                return false;
            }

            var emptyMenu = ScriptableObject.CreateInstance(typeof(ExpressionsMenu)) as ExpressionsMenu;
            emptyMenu.controls = new List<ExpressionsMenu.Control>();

            AssetDatabase.CreateAsset(emptyMenu, path);
            expressionMenu = AssetDatabase.LoadAssetAtPath<ExpressionsMenu>(path);

            avatar.expressionsMenu = expressionMenu;
            avatar.customExpressions = true;
            EditorUtility.SetDirty(expressionMenu);
            EditorUtility.SetDirty(avatar);

            return true;
        }
    }
}