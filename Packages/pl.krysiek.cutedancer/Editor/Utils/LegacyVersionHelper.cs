using System.IO;
using UnityEditor;
using UnityEngine;

namespace VRF
{
    public class LegacyVersionHelper
    {
        private static Logger log = new Logger("LegacyVersionHelper");

        static string OLD_MAIN_DIR_GUID = "1d3ab7f8180cb59439d280fa090061fa";
        static string OLD_MAIN_DIR_PATH = Path.Join("Assets", "CuteDancer");

        public static void RunCheck()
        {
            log.LogInfo("Run one-time check for old CuteDancer versions in Assets directory");

            string actualOldPath = AssetDatabase.GUIDToAssetPath(OLD_MAIN_DIR_GUID);

            if (AssetDatabase.IsValidFolder(actualOldPath) && !actualOldPath.EndsWith("Legacy"))
            {
                // workaround to force Unity to not use old GUID for any new directory created at the same path
                string tempRemovePath = actualOldPath + "Legacy";
                AssetDatabase.MoveAsset(actualOldPath, tempRemovePath);

                bool remove = EditorUtility.DisplayDialog(
                    "CuteDancer: legacy version detected",
                    "The old version of CuteDancer was found in the project and was renamed to the path:\n\n" +
                    tempRemovePath + "\n\n" +
                    "If you didn't modify or customize it, you can safely remove it. Otherwise, you may want to keep these files for future use.\n\n" +
                    "Would you like to remove it now?",
                    "Remove it",
                    "Cancel"
                );

                if (remove)
                {
                    AssetDatabase.DeleteAsset(tempRemovePath);
                }
            }

            if (!AssetDatabase.IsValidFolder(OLD_MAIN_DIR_PATH))
            {
                // If user manually delete the old directory, Unity will restore the old GUID for newly created one.
                // This workaround forces Unity to assign old GUID to a different directory name
                AssetDatabase.CreateFolder("Assets", "CuteDancer");
                string tempPath = OLD_MAIN_DIR_PATH + "TempToRemove";
                AssetDatabase.MoveAsset(OLD_MAIN_DIR_PATH, tempPath);
                AssetDatabase.DeleteAsset(tempPath);
            }
        }
    }
}
