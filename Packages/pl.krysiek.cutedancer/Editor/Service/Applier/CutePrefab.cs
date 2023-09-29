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
        static string MUSIC_PREFAB_FILENAME = Path.Combine("CuteDancer-Music.prefab");
        static string CONTACT_PREFAB_FILENAME = Path.Combine("CuteDancer-Contacts.prefab");

        enum Status
        {
            FORM, EMPTY, ADDED, ADDED_PARTIAL, DIFFERENCE
        }
        
        private string buildPath;
        public string BuildPath
        {
            set => buildPath = value;
        }

        private string MusicPrefabPath => Path.Combine(buildPath, MUSIC_PREFAB_FILENAME);
        private string ContactPrefabPath => Path.Combine(buildPath, CONTACT_PREFAB_FILENAME);

        private GameObject avatar;
        public AvatarDescriptor Avatar
        {
            set => avatar = value?.gameObject;
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
                var musicPrefab = AssetDatabase.LoadAssetAtPath(MusicPrefabPath, typeof(GameObject));
                var musicPrefabInstance = PrefabUtility.InstantiatePrefab(musicPrefab, avatar.transform);
                EditorUtility.SetDirty(musicPrefabInstance);
            }

            Transform contactInstance = avatar.transform.Find("CuteDancer-Contacts");
            if (!contactInstance)
            {
                var contactPrefab = AssetDatabase.LoadAssetAtPath(ContactPrefabPath, typeof(GameObject));
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