using UnityEngine;
using UnityEditor;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using System.IO;

namespace VRF
{
    public class CutePrefab : AvatarApplierInterface
    {
        private static Logger log = new Logger("CutePrefab");

        static string PREFAB_FILENAME = Path.Combine("CuteDancer.prefab");

        enum Status
        {
            FORM, EMPTY, ADDED, ADDED_PARTIAL, DIFFERENCE
        }
        
        private string buildPath;
        public string BuildPath
        {
            set => buildPath = value;
        }

        private string PrefabPath => Path.Combine(buildPath, PREFAB_FILENAME);

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

            Transform prefabInstance = avatar.transform.Find("CuteDancer");

            if (prefabInstance)
            {
                if (IsPrefabModified(prefabInstance.gameObject))
                {
                    return ApplyStatus.UPDATE;
                }

                return ApplyStatus.REMOVE;
            }

            if (!prefabInstance)
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
                log.LogDebug($"Prefab {instance.name} is modified.");
                return true;
            }
            return false;
        }

        public void HandleAdd(bool silent = false)
        {
            Transform existingInstance = avatar.transform.Find("CuteDancer");
            if (!existingInstance)
            {
                var prefab = AssetDatabase.LoadAssetAtPath(PrefabPath, typeof(GameObject));
                var prefabInstance = PrefabUtility.InstantiatePrefab(prefab, avatar.transform);
                EditorUtility.SetDirty(prefabInstance);
            }
        }

        public void HandleRemove(bool silent = false)
        {
            RemoveByName("CuteDancer");
            // legacy names:
            RemoveByName("CuteDancerMusic");
            RemoveByName("CuteDancerContacts");
            RemoveByName("CuteDancer-Music");
            RemoveByName("CuteDancer-Contacts");

            EditorUtility.SetDirty(avatar);
        }

        private void RemoveByName(string name) {
            Transform prefab = avatar.transform.Find(name);
            if (prefab)
            {
                UnityEngine.Object.DestroyImmediate(prefab.gameObject);
            }
        }
    }
}