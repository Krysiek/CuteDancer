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
    public class DancesLoader
    {
        static string DANCES_DIR = Path.Combine("Packages", "pl.krysiek.cutedancer", "Resources", "Dances"); // TODO add custom path from assets

        public bool loaded = false;

        public Dictionary<string, List<DanceData>> collections = new Dictionary<string, List<DanceData>>();

        public string[] originalsWhitelist = new string[] { "SARDefaultDance", "BadgerDance", "ShoulderShakeDance", "ZufoloImpazzitoDance", "DistractionDance" };

        public void LoadDances()
        {
            // TODO preserve selected dances
            collections.Clear();

            string[] dancesPath = Directory.GetDirectories(DANCES_DIR);

            foreach (string dancePath in dancesPath)
            {
                try
                {
                    DanceData danceData = ScriptableObject.CreateInstance<DanceData>();

                    danceData.code = Path.GetFileName(dancePath);
                    danceData.displayCode = danceData.code;

                    string infoPath = Path.Combine(dancePath, "info.json");

                    if (File.Exists(infoPath))
                    {
                        string infoDataStr = new StreamReader(infoPath).ReadToEnd();
                        DanceJsonData parsed = JsonUtility.FromJson<DanceJsonData>(infoDataStr);
                        danceData.displayCode = parsed.displayCode;
                        danceData.author = "by " + parsed.author;
                        danceData.collection = parsed.collection;
                        danceData.order = parsed.order;
                        if (danceData.collection == "Originals")
                        {
                            if (!originalsWhitelist.Contains(danceData.code))
                            {
                                danceData.collection = "Not originals";
                            }
                        }
                    }
                    else
                    {
                        danceData.collection = "Other";
                    }

                    if (!collections.ContainsKey(danceData.collection))
                    {
                        collections.Add(danceData.collection, new List<DanceData>());
                    }

                    List<DanceData> dances = collections[danceData.collection];

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


            foreach (KeyValuePair<string, List<DanceData>> collection in collections)
            {
                collection.Value.Sort((o1, o2) => o1.order - o2.order);
            }

            loaded = true;
        }
    }
}

#endif