using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using System.IO;

namespace VRF
{
    public class CutePrefab : AvatarApplierInterface
    {
        // TODO read from build configuration
        static string MUSIC_PREFAB = Path.Combine("Assets", "CuteDancer", "Build", "CuteDancer-Music.prefab");
        static string CONTACT_PREFAB = Path.Combine("Assets", "CuteDancer", "Build", "CuteDancer-Contacts.prefab");

        enum Status
        {
            FORM, EMPTY, ADDED, ADDED_PARTIAL, DIFFERENCE
        }

        GameObject avatar;
        Status validStat = Status.FORM;

        public void RenderForm()
        {
            validStat = Validate();

            GUIStyle labelStyle = new GUIStyle(EditorStyles.largeLabel);
            labelStyle.wordWrap = true;
            GUILayout.Label("Add music and contact prefabs using the button below or drag & drop them to the root of your avatar",
                        labelStyle);
            labelStyle.fontStyle = FontStyle.Italic;

            GUILayout.BeginHorizontal();

            if (validStat == Status.ADDED_PARTIAL || validStat == Status.DIFFERENCE)
            {
                CuteButtons.RenderButton("Update prefabs", CuteIcons.ADD, HandleUpdate);
            }
            else
            {
                CuteButtons.RenderButton("Add prefabs", CuteIcons.ADD, HandleAdd, validStat == Status.FORM || validStat == Status.ADDED);
            }
            CuteButtons.RenderButton("Remove", CuteIcons.REMOVE, HandleRemove, validStat == Status.FORM || validStat == Status.EMPTY, GUILayout.Width(150));

            GUILayout.EndHorizontal();
        }

        public void RenderStatus()
        {
            switch (validStat)
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
        }

        public void ClearForm()
        {
            avatar = null;
        }

        Status Validate()
        {
            if (!avatar)
            {
                return Status.FORM;
            }

            Transform musicInstance = avatar.transform.Find("CuteDancer-Music");
            Transform contactInstance = avatar.transform.Find("CuteDancer-Contacts");

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

        public void HandleAdd()
        {
            Transform musicInstance = avatar.transform.Find("CuteDancer-Music");
            if (!musicInstance)
            {
                var musicPrefab = AssetDatabase.LoadAssetAtPath(MUSIC_PREFAB, typeof(GameObject));
                var musicPrefabInstance = PrefabUtility.InstantiatePrefab(musicPrefab, avatar.transform);
                EditorUtility.SetDirty(musicPrefabInstance);
            }

            Transform contactInstance = avatar.transform.Find("CuteDancer-Contacts");
            if (!contactInstance)
            {
                var contactPrefab = AssetDatabase.LoadAssetAtPath(CONTACT_PREFAB, typeof(GameObject));
                var contactPrefabInstance = PrefabUtility.InstantiatePrefab(contactPrefab, avatar.transform);
                EditorUtility.SetDirty(contactPrefabInstance);
            }
        }

        public void HandleRemove()
        {
            Transform musicInstance = avatar.transform.Find("CuteDancer-Music");
            Transform contactInstance = avatar.transform.Find("CuteDancer-Contacts");

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