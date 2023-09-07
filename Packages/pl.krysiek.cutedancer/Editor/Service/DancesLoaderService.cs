#if VRC_SDK_VRCSDK3
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    public class DancesLoaderService
    {
        static string DANCES_DIR = Path.Combine("Packages", "pl.krysiek.cutedancer", "Runtime", "Dances");
        // TODO add custom path from assets
        static string[] ORIGINALS_WHITELIST = new string[] { "SARDefaultDance", "BadgerDance", "ShoulderShakeDance", "ZufoloImpazzitoDance", "DistractionDance" };

        public Dictionary<string, List<DanceViewData>> LoadDances()
        {
            string[] dancesPath = Directory.GetDirectories(DANCES_DIR);
            // TODO add looking for extra dances in other packages with "cutedancer" in name

            Dictionary<string, List<DanceViewData>> collections = new Dictionary<string, List<DanceViewData>>();

            foreach (string dancePath in dancesPath)
            {
                try
                {
                    DanceViewData danceData = ScriptableObject.CreateInstance<DanceViewData>();

                    string infoPath = Directory.GetFiles(dancePath, "*.json")[0];

                    if (!File.Exists(infoPath))
                    {
                        Debug.LogWarning("Skipped dance without json info: " + dancePath);
                        continue;
                    }

                    StreamReader streamReader = new StreamReader(infoPath);
                    string infoDataStr = streamReader.ReadToEnd();
                    streamReader.Close();
                    DanceJsonData infoData = JsonUtility.FromJson<DanceJsonData>(infoDataStr);
                    danceData._name = infoData.name;

                    if (danceData._name == null)
                    {
                        Debug.LogWarning("Skipped dance without name: " + dancePath);
                        continue;
                    }

                    danceData.displayName = infoData.displayName != null ? infoData.displayName : infoData.name;
                    danceData.author = infoData.author != null ? "by " + infoData.author : "";
                    danceData.collection = infoData.collection != null ? infoData.collection : "Other";
                    danceData.order = infoData.order;
                    if (danceData.collection == "Originals")
                    {
                        if (!ORIGINALS_WHITELIST.Contains(danceData._name))
                        {
                            danceData.collection = "Not originals";
                        }
                    }

                    if (!collections.ContainsKey(danceData.collection))
                    {
                        collections.Add(danceData.collection, new List<DanceViewData>());
                    }

                    List<DanceViewData> dances = collections[danceData.collection];

                    string animatorPath = Directory.GetFiles(dancePath, "*.controller")[0];
                    danceData.animator = AssetDatabase.LoadAssetAtPath<Animator>(animatorPath);

                    string iconPath = Directory.GetFiles(dancePath, "*.png").FirstOrDefault();
                    danceData.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

                    string audioPath = Directory.GetFiles(dancePath, "*.ogg").FirstOrDefault();
                    danceData.audio = AssetDatabase.LoadAssetAtPath<AudioClip>(audioPath);

                    dances.Add(danceData);
                }
                catch (Exception err)
                {
                    Debug.LogWarning("Incorrect structure, dance skipped: " + dancePath + ", error: " + err.ToString());
                }
            }

            foreach (KeyValuePair<string, List<DanceViewData>> collection in collections)
            {
                collection.Value.Sort((o1, o2) => o1.order - o2.order);
            }

            return collections;
        }
    }
}

#endif