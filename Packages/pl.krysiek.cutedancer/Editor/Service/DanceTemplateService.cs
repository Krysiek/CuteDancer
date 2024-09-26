using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DanceTemplateService
{
    static string DANCE_NAME = "MySuperDance";
    static string SRC_PATH = Path.Combine(CuteResources.CUTEDANCER_RUNTIME, "TemplateDance", DANCE_NAME);

    public void CreateTemplate(string destinationPath)
    {
        Directory.CreateDirectory(destinationPath);
        string destDancePath = Path.Combine(destinationPath, DANCE_NAME);
        AssetDatabase.CopyAsset(SRC_PATH, destDancePath);

        string orgAnimGuid = AssetDatabase.AssetPathToGUID(Path.Combine(SRC_PATH, "MySuperDance.anim"));
        string copiedAnimGuid = AssetDatabase.AssetPathToGUID(Path.Combine(destDancePath, "MySuperDance.anim"));

        string controllerToUpdate = Path.Combine(destDancePath, "MySuperDance.controller");
        string controllerData = File.ReadAllText(controllerToUpdate);

        controllerData = controllerData.Replace(orgAnimGuid, copiedAnimGuid);

        File.WriteAllText(controllerToUpdate, controllerData);

        AssetDatabase.Refresh();

        Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(destDancePath);
    }
}
