#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Dynamics.Contact.Components;

namespace VRF
{
    public class ContactsPrefabBuilder
    {

        public void BuildContactsPrefab(SettingsBuilderData settings)
        {
            string sourcePath = Path.Combine(CuteResources.CUTEDANCER_RUNTIME, "TemplateContacts.prefab");
            string outputPath = Path.Combine(settings.outputDirectory, "CuteDancer-Contacts.prefab");

            if (!AssetDatabase.CopyAsset(sourcePath, outputPath))
            {
                throw new Exception("Error copying template: Contacts Prefab");
            }

            GameObject prefab = PrefabUtility.LoadPrefabContents(outputPath);

            Transform receiversGroup = prefab.transform.Find("Receivers");
            Transform templateReceiver = receiversGroup.Find("{DANCE}Receiver");
            Transform templateSender = prefab.transform.Find("{DANCE}Sender");

            VRCContactReceiver multiReceiver = prefab.transform.Find("CuteDancerMultiReceiver").GetComponent<VRCContactReceiver>();

            int multiReceiversCount = 1;

            foreach (DanceBuilderData dance in settings.dances)
            {
                if (multiReceiver.collisionTags.Count >= 16)
                {
                    multiReceiver = UnityEngine.Object.Instantiate(multiReceiver, prefab.transform);
                    multiReceiver.collisionTags.Clear();
                    multiReceiver.name = multiReceiver.name.Replace("{Clone}", "_" + multiReceiversCount);
                    multiReceiversCount++;
                }

                multiReceiver.collisionTags.Add(dance._name);

                VRCContactReceiver receiver = UnityEngine.Object.Instantiate(templateReceiver, receiversGroup).GetComponent<VRCContactReceiver>();
                receiver.collisionTags[0] = dance._name;
                receiver.parameter = dance._name;
                receiver.name = templateReceiver.name.Replace("{DANCE}", dance._name);

                VRCContactSender sender = UnityEngine.Object.Instantiate(templateSender, prefab.transform).GetComponent<VRCContactSender>();
                sender.collisionTags[0] = dance._name;
                sender.name = templateReceiver.name.Replace("{DANCE}", dance._name);
            }

            templateReceiver.parent = prefab.transform; // need this before nulling, otherwise obejct spawns in scene
            templateReceiver.parent = null;
            templateSender.parent = null;

            Debug.Log("Save file [name = " + outputPath + "]");
            PrefabUtility.SaveAsPrefabAsset(prefab, outputPath);
            PrefabUtility.UnloadPrefabContents(prefab);
        }
    }
}
#endif