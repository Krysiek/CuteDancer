using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AssetCleanup : MonoBehaviour
{
    public static bool RemoveOrphans(string assetPath)
    {
        if (AssetDatabase.GetMainAssetTypeAtPath(assetPath) != typeof(AnimatorController)) {
            // AnimatorController structure is flat. There are bunch of elements referencing each other by fileID.
            // Main element contains reference to itself, so it won't be removed.

            // Other assets usually have one top element without any internal reference to it.
            // In this case this code can completely remove content of the asset, so better break here.
            return false;
        }

        string rawData = File.ReadAllText(assetPath);
        string[] rawLines = File.ReadAllLines(assetPath);

        Dictionary<string, string> orphans = new Dictionary<string, string>();

        for (int i = 0; i < rawLines.Length; i++)
        {
            string line = rawLines[i];
            if (line.StartsWith("---"))
            {
                string id = line.Split('&')[1];
                if (!rawData.Contains("fileID: " + id))
                {
                    orphans.Add(id, rawLines[i + 1]);
                }
            }
        }

        if (orphans.Count > 0)
        {

            var groupedByType = orphans.GroupBy(p => p.Value);

            string info = "";

            foreach (var item in groupedByType)
            {
                info += $"{item.Key} {item.Count()} elements\n";
            }

            bool ok = EditorUtility.DisplayDialog(
                "Asset cleanup",
                "Orphans found in the file:\n\n" + info,
                "Remove", "Cancel");

            if (ok)
            {
                var ids = orphans.Select(p => p.Key);

                string newData = "";
                bool copy = true;

                foreach (var line in rawLines)
                {
                    if (line.StartsWith("---"))
                    {
                        string id = line.Split('&')[1];
                        copy = !ids.Contains(id);
                    }
                    if (copy)
                    {
                        newData += line + "\n";
                    }
                }

                File.WriteAllText(assetPath, newData);
                AssetDatabase.Refresh();

                return true;
            }

        }

        return false;
    }
}
