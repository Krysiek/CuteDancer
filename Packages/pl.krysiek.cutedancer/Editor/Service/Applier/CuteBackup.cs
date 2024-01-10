using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace VRF
{
    class CuteBackup
    {
        public static void CreateBackup(string path, string avatarName)
        {
            string date = System.DateTime.Now.ToString("yyyyMMdd_HH");

            string filename = Path.GetFileName(path);
            string ext = Path.GetExtension(filename);
            string avatarNameSafe = Regex.Replace(avatarName, "\\W", "");
            string newFilename = $"{avatarNameSafe}_{Path.ChangeExtension(filename, null)}_backup_{date}{ext}";

            string backupPath = Path.Join(SettingsService.Instance.backupDirectory, newFilename);

            if (!Directory.Exists(SettingsService.Instance.backupDirectory))
            {
                Directory.CreateDirectory(SettingsService.Instance.backupDirectory);
            }

            if (!File.Exists(backupPath))
            {
                if (AssetDatabase.CopyAsset(path, backupPath))
                {
                    Debug.Log("Backup created: " + backupPath);
                }
                else
                {
                    Debug.LogWarning("Failed to create backup: " + backupPath);
                }
            }
            else
            {
                Debug.Log("Skip create backup (already created): " + backupPath);
            }
        }
    }

}