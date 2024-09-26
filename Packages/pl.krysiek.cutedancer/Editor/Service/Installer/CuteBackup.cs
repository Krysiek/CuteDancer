using System.IO;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace VRF
{
    class CuteBackup
    {
        private static Logger log = new Logger("CuteBackup");

        public static void CreateBackup(string path, string avatarName)
        {
            string date = System.DateTime.Now.ToString("yyyyMMdd_HH");

            string filename = Path.GetFileName(path);
            string ext = Path.GetExtension(filename);
            string avatarNameSafe = Regex.Replace(avatarName, "\\W", "");
            string newFilename = $"{avatarNameSafe}_{Path.ChangeExtension(filename, null)}_backup_{date}{ext}";

            string backupPath = Path.Join(SettingsService.Instance.BackupDirectory, newFilename);

            if (!Directory.Exists(SettingsService.Instance.BackupDirectory))
            {
                Directory.CreateDirectory(SettingsService.Instance.BackupDirectory);
            }

            if (!File.Exists(backupPath))
            {
                if (AssetDatabase.CopyAsset(path, backupPath))
                {
                    log.LogInfo("Backup created: " + backupPath);
                }
                else
                {
                    log.LogWarn("Failed to create backup: " + backupPath);
                }
            }
            else
            {
                log.LogDebug("Skip create backup (already created): " + backupPath);
            }
        }
    }

}