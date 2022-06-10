using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    public class CutePrefab : CuteGroup
    {
        string avatarName = "Avatar";
        GameObject avatar;

        static string MUSIC_PREFAB = "Assets/CuteDancer/CuteDancerMusic.prefab";
        static string CONTACT_PREFAB = "Assets/CuteDancer/CuteDancerContact.prefab";

        enum Status
        {
            FORM, EMPTY, ADDED, MISMATCH
        }

        public void RenderForm()
        {

            GUIStyle labelStyle = new GUIStyle(EditorStyles.largeLabel);
            labelStyle.wordWrap = true;
            GUILayout.Label("Please drag & drop prefabs to root of your avatar:\n" +
                            "- CuteDancerMusic\n" +
                            "- CuteDancerContact",
                        labelStyle);
            labelStyle.fontStyle = FontStyle.Italic;
        }

        public void RenderStatus()
        {
            switch (ValidateMusic())
            {
                case Status.FORM:
                    CuteInfoBox.RenderInfoBox(CuteIcons.INFO, "Avatar not selected.");
                    break;
                case Status.EMPTY:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "Music prefab is not added.");
                    break;
                case Status.ADDED:
                    CuteInfoBox.RenderInfoBox(CuteIcons.OK, "Music prefab is added.");
                    break;
                case Status.MISMATCH:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "Music prefab found on the avatar, but the path is different.\nMake sure animations are updated. See README, section 'Updating animations', for more details.");
                    break;
            }

            switch (ValidateContact())
            {
                case Status.FORM:
                    CuteInfoBox.RenderInfoBox(CuteIcons.INFO, "Avatar not selected.");
                    break;
                case Status.EMPTY:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "CuteDancerContact prefab is not added.");
                    break;
                case Status.ADDED:
                    CuteInfoBox.RenderInfoBox(CuteIcons.OK, "CuteDancerContact prefab is added.");
                    break;
                case Status.MISMATCH:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "CuteDancerContact prefab found on the avatar, but the path is different.\nMake sure animations are updated. See README, section 'Updating animations', for more details.");
                    break;
            }
        }

        public void SetAvatar(AvatarDescriptor avatarDescriptor)
        {
            avatar = avatarDescriptor.gameObject;
            avatarName = avatarDescriptor.name;
        }

        public void ClearForm()
        {
            avatar = null;
            avatarName = "Avatar";
        }

        Status ValidateMusic()
        {
            if (!avatar)
            {
                return Status.FORM;
            }

            string musicGuid = AssetDatabase.AssetPathToGUID(MUSIC_PREFAB);

            Transform musicInstance = avatar.transform.Find("CuteDancerMusic");

            if (musicInstance)
            {
                return Status.ADDED;
            }

            // RecursiveFindChild
            // AssetDatabase.LoadAssetAtPath(CUTE_MENU, typeof(ExpressionsMenu)) as ExpressionsMenu;
            // PrefabUtility.GetPrefabAssetType

            return Status.EMPTY;
        }

        Status ValidateContact()
        {
            if (!avatar)
            {
                return Status.FORM;
            }

            string contactGuid = AssetDatabase.AssetPathToGUID(CONTACT_PREFAB);

            Transform contactInstance = avatar.transform.Find("CuteDancerContact");

            if (contactInstance)
            {
                return Status.ADDED;
            }


            // RecursiveFindChild(avatar.transform, contactGuid);

            return Status.EMPTY;
        }

        Transform RecursiveFindChild(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {

                // var ptype = PrefabUtility.GetCorrespondingObjectFromOriginalSource(child.gameObject);
                // Debug.Log(child.name +"---"+ ptype);

                if (child.name == childName)
                {
                    return child;
                }
                else
                {
                    Transform found = RecursiveFindChild(child, childName);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }
    }
}