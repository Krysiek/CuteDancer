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
        private static Logger log = new Logger("CuteSubmenu");

        static string CUTE_MENU_FILENAME = Path.Combine("CuteDancer-VRCMenu.asset");
        static string DANCE_ICON = Path.Combine("Packages", "pl.krysiek.cutedancer", "Runtime", "Icons", "CuteDancer.png");

        private ExpressionsMenu expressionMenu;

        private string buildPath;
        public string BuildPath
        {
            get => Path.Combine(buildPath, CUTE_MENU_FILENAME);
            set => buildPath = value;
        }

        private AvatarDescriptor avatar;
        public AvatarDescriptor Avatar
        {
            set
            {
                avatar = value;
                expressionMenu = value?.expressionsMenu;
            }
        }

        public ApplyStatus GetStatus()
        {
            if (avatar == null)
            {
                return ApplyStatus.EMPTY;
            }
            if (expressionMenu == null)
            {
                return ApplyStatus.ADD;
            }
            ExpressionsMenu cuteMenu = AssetDatabase.LoadAssetAtPath<ExpressionsMenu>(BuildPath);
            if (expressionMenu.controls.Exists(menuEntry => menuEntry.subMenu == cuteMenu))
            {
                return ApplyStatus.REMOVE;
            }
            Texture2D cuteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DANCE_ICON);
            if (expressionMenu.controls.Exists(menuEntry => menuEntry.name == "CuteDancer") || expressionMenu.controls.Exists(menuEntry => menuEntry.icon == cuteIcon))
            {
                return ApplyStatus.UPDATE;
            }
            if (expressionMenu.controls.ToArray().Length >= 8)
            {
                return ApplyStatus.BLOCKED;
            }
            return ApplyStatus.ADD;
        }

        public void HandleAdd(bool silent = false)
        {
            if (!expressionMenu && !CreateExpressionMenu())
            {
                return;
            }

            DoBackup();

            ExpressionsMenu cuteMenu = AssetDatabase.LoadAssetAtPath(BuildPath, typeof(ExpressionsMenu)) as ExpressionsMenu;

            var menuEntry = new ExpressionsMenu.Control
            {
                name = "CuteDancer",
                icon = AssetDatabase.LoadAssetAtPath(DANCE_ICON, typeof(Texture2D)) as Texture2D,
                type = ExpressionsMenu.Control.ControlType.SubMenu,
                subMenu = cuteMenu
            };

            log.LogDebug("Adding expression menu control to menu [name=" + expressionMenu.name + "]");
            expressionMenu.controls.Add(menuEntry);
            EditorUtility.SetDirty(expressionMenu);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void HandleRemove(bool silent = false)
        {
            DoBackup();

            Texture2D cuteIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(DANCE_ICON);
            int ix = expressionMenu.controls.FindIndex(menuEntry => menuEntry.icon == cuteIcon);
            if (ix == -1)
            {
                log.LogDebug("Expression menu not found by icon, searching by name.");
                ix = expressionMenu.controls.FindIndex(menuEntry => menuEntry.name == "CuteDancer" && menuEntry.subMenu == null);
            }

            if (ix > -1)
            {
                log.LogDebug("Removing expression menu control from menu [name=" + expressionMenu.name + ", index=" + ix + "]");
                expressionMenu.controls.RemoveAt(ix);
                EditorUtility.SetDirty(expressionMenu);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        void DoBackup()
        {
            CuteBackup.CreateBackup(AssetDatabase.GetAssetPath(expressionMenu), avatar.name);
        }

        bool CreateExpressionMenu()
        {
            string path = $"Assets/{avatar.name}-ExpressionMenu.asset";
            bool ok = EditorUtility.DisplayDialog("CuteScript", $"It seems your avatar does not have expression menu. Empty one will be created and assigned to your avatar.\n\nNew asset will be saved under path:\n{path}", "Create it!", "Cancel");
            if (!ok)
            {
                EditorUtility.DisplayDialog("CuteScript", "Operation aborted. Expresion Menu is NOT added!", "OK");
                return false;
            }

            ExpressionsMenu emptyMenu = ScriptableObject.CreateInstance(typeof(ExpressionsMenu)) as ExpressionsMenu;
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