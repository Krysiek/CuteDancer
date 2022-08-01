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
            FORM, EMPTY, ADDED, ADDED_PARTIAL, DIFFERENCE
        }

        Status validationStatus = Status.FORM;

        public void RenderForm()
        {
            validationStatus = Validate();

            GUIStyle labelStyle = new GUIStyle(EditorStyles.largeLabel);
            labelStyle.wordWrap = true;
            GUILayout.Label("Add prefabs using the button below or drag & drop them to the root of your avatar",
                        labelStyle);
            labelStyle.fontStyle = FontStyle.Italic;

            GUILayout.BeginHorizontal();

            if (validationStatus == Status.ADDED_PARTIAL || validationStatus == Status.DIFFERENCE)
            {
                CuteButtons.RenderButton("Update prefabs", CuteIcons.ADD, HandleUpdate);
            }
            else
            {
                CuteButtons.RenderButton("Add prefabs", CuteIcons.ADD, HandleAdd, validationStatus == Status.FORM || validationStatus == Status.ADDED);
            }
            CuteButtons.RenderButton("Remove", CuteIcons.REMOVE, HandleRemove, validationStatus == Status.FORM || validationStatus == Status.EMPTY, GUILayout.Width(150));

            GUILayout.EndHorizontal();
        }

        public void RenderStatus()
        {
            switch (validationStatus)
            {
                case Status.FORM:
                    CuteInfoBox.RenderInfoBox(CuteIcons.INFO, "Avatar not selected.");
                    break;
                case Status.EMPTY:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "Prefabs are not added.");
                    break;
                case Status.ADDED:
                    CuteInfoBox.RenderInfoBox(CuteIcons.OK, "Prefab are added.");
                    break;
                case Status.ADDED_PARTIAL:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "Some of required prefabs are missing.");
                    break;
                case Status.DIFFERENCE:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "Prefabs on the avatar are out of date. Press update button to fix it.");
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

        Status Validate()
        {
            if (!avatar)
            {
                return Status.FORM;
            }

            Transform musicInstance = avatar.transform.Find("CuteDancerMusic");
            Transform contactInstance = avatar.transform.Find("CuteDancerContact");

            if (musicInstance && contactInstance)
            {
                if (IsPrefabModified(musicInstance.gameObject) || IsPrefabModified(contactInstance.gameObject))
                {
                    return Status.DIFFERENCE;
                }

                return Status.ADDED;
            }

            if (!musicInstance && !contactInstance)
            {
                return Status.EMPTY;
            }

            return Status.ADDED_PARTIAL;
        }

        bool IsPrefabModified(GameObject instance)
        {
            var status = PrefabUtility.GetPrefabInstanceStatus(instance);
            var isModified = PrefabUtility.HasPrefabInstanceAnyOverrides(instance, false);
            if (status != PrefabInstanceStatus.Connected || isModified)
            {
                Debug.Log($"Prefab {instance.name} is modified.");
                return true;
            }
            return false;
        }

        void HandleAdd()
        {
            Transform musicInstance = avatar.transform.Find("CuteDancerMusic");
            if (!musicInstance)
            {
                var musicPrefab = AssetDatabase.LoadAssetAtPath(MUSIC_PREFAB, typeof(GameObject));
                var musicPrefabInstance = PrefabUtility.InstantiatePrefab(musicPrefab, avatar.transform);
                EditorUtility.SetDirty(musicPrefabInstance);
            }

            Transform contactInstance = avatar.transform.Find("CuteDancerContact");
            if (!contactInstance)
            {
                var contactPrefab = AssetDatabase.LoadAssetAtPath(CONTACT_PREFAB, typeof(GameObject));
                var contactPrefabInstance = PrefabUtility.InstantiatePrefab(contactPrefab, avatar.transform);
                EditorUtility.SetDirty(contactPrefabInstance);
            }
        }

        void HandleRemove()
        {
            Transform musicInstance = avatar.transform.Find("CuteDancerMusic");
            Transform contactInstance = avatar.transform.Find("CuteDancerContact");

            if (musicInstance)
            {
                UnityEngine.Object.DestroyImmediate(musicInstance.gameObject);
            }
            if (contactInstance)
            {
                UnityEngine.Object.DestroyImmediate(contactInstance.gameObject);
            }

            EditorUtility.SetDirty(avatar);
        }

        void HandleUpdate()
        {
            // yolo
            HandleRemove();
            HandleAdd();
        }
    }
}