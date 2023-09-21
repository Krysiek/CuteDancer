#if VRC_SDK_VRCSDK3
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace VRF
{
    public class DancesLoaderService
    {
        static string DANCES_DIR = Path.Combine("Packages", "pl.krysiek.cutedancer", "Runtime", "Dances");   
        static string[] ORIGINALS_WHITELIST = new string[] { "SARDefaultDance", "BadgerDance", "ShoulderShakeDance", "ZufoloImpazzitoDance", "DistractionDance" };

        SettingsService settings = SettingsService.Instance;

        public Dictionary<string, List<DanceViewData>> LoadDances()
        {
            string[] dancesPaths = Directory.GetDirectories(DANCES_DIR);
            // TODO 1st add looking for dances in Assets/CuteDancer/Dances (customizable path)
            // TODO 2nd add looking for extra dances in other packages with "cutedancer" in name

            Dictionary<string, List<DanceViewData>> collections = new Dictionary<string, List<DanceViewData>>();

            foreach (string dancePath in dancesPaths)
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
                    danceData.animator = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorPath);

                    string iconPath = Directory.GetFiles(dancePath, "*.png").FirstOrDefault();
                    danceData.icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

                    string audioPath = Directory.GetFiles(dancePath, "*.ogg").FirstOrDefault();
                    danceData.audio = AssetDatabase.LoadAssetAtPath<AudioClip>(audioPath);

                    danceData.selected = settings.selectedDances.Contains(danceData._name);
                    danceData.audioEnabled = !settings.musicDisabledDances.Contains(danceData._name);

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