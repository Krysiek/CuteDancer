using System.IO;
using UnityEngine;
using UnityEditor;

namespace VRF
{
    class CuteBackup
    {
        public static void CreateBackup(string path)
        {
            string date = System.DateTime.Now.ToString("yyyyMMdd");

            string ext = Path.GetExtension(path);
            string backupPath = Path.ChangeExtension(path, null) + "_backup_" + date + ext;

            if (!File.Exists(backupPath))
            {
                FileUtil.CopyFileOrDirectory(path, backupPath);
                Debug.Log("Daily backup created: " + backupPath);
            }
            else
            {
                Debug.Log("Skip create backup (already created): " + backupPath);
            }
        }
    }

}