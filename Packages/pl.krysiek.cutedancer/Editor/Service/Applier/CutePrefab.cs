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

        public void SetAvatar(AvatarDescriptor avatarDescriptor)
        {
            avatar = avatarDescriptor.gameObject;
        }

        public void ClearForm()
        {
            avatar = null;
        }

        public ApplyStatus GetStatus()
        {
            if (!avatar)
            {
                return ApplyStatus.EMPTY;
            }

            Transform musicInstance = avatar.transform.Find("CuteDancer-Music");
            Transform contactInstance = avatar.transform.Find("CuteDancer-Contacts");

            if (musicInstance && contactInstance)
            {
                if (IsPrefabModified(musicInstance.gameObject) || IsPrefabModified(contactInstance.gameObject))
                {
                    return ApplyStatus.UPDATE;
                }

                return ApplyStatus.REMOVE;
            }

            if (!musicInstance && !contactInstance)
            {
                return ApplyStatus.ADD;
            }

            return ApplyStatus.UPDATE;
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