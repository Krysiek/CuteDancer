using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetCleanup : MonoBehaviour
{
    public static bool RemoveOrphans(string assetPath)
    {
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
                "Orphans removal",
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
