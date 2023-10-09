using UnityEditor;

namespace VRF
{
    public class LegacyVersionHelper
    {

        static string OLD_MAIN_DIR_GUID = "1d3ab7f8180cb59439d280fa090061fa";

        public static void RunCheck()
        {
            string oldMainDirPath = AssetDatabase.GUIDToAssetPath(OLD_MAIN_DIR_GUID);

            if (AssetDatabase.IsValidFolder(oldMainDirPath))
            {
                bool remove = EditorUtility.DisplayDialog(
                    "CuteDancer: legacy version detected",
                    "The old version of CuteDancer is still present in your project at path:\n\n" +
                    oldMainDirPath + "\n\n" +
                    "Would you like to remove it now?",
                    "Remove it",
                    "Cancel"
                );

                if (remove)
                {
                    AssetDatabase.DeleteAsset(oldMainDirPath);
                }

            }
        }
    }
}
