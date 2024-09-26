using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRF;
using YamlDotNet.Core.Tokens;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

public class AvatarApplyService
{
    private static Logger log = new Logger("AvatarApplyService");

    CutePrefab cutePrefab = new CutePrefab();
    CuteParams cuteParams = new CuteParams();
    CuteSubmenu cuteSubmenu = new CuteSubmenu();
    CuteLayers cuteLayers = new CuteLayers();

    public BuildInfoData BuildInfo
    {
        set
        {
            string buildPath = AssetDatabase.GetAssetPath(value);
            if (buildPath != "")
            {
                buildPath = Path.GetDirectoryName(buildPath);
            }
            cutePrefab.BuildPath = buildPath;
            cuteParams.BuildPath = buildPath;
            cuteSubmenu.BuildPath = buildPath;
            cuteLayers.BuildPath = buildPath;
        }
    }

    private AvatarDescriptor avatar;
    public AvatarDescriptor Avatar
    {
        set
        {
            avatar = value;
            cutePrefab.Avatar = value;
            cuteParams.Avatar = value;
            cuteSubmenu.Avatar = value;
            cuteLayers.Avatar = value;
        }
    }

    public void AddToAvatar(bool isUpdate = false)
    {
        cutePrefab.HandleAdd();
        cuteParams.HandleAdd();
        cuteSubmenu.HandleAdd();
        cuteLayers.HandleAdd(isUpdate);
        log.LogInfo("AddToAvatar complete");
    }

    public void RemoveFromAvatar(bool isUpdate = false)
    {
        cutePrefab.HandleRemove();
        cuteParams.HandleRemove();
        cuteSubmenu.HandleRemove();
        cuteLayers.HandleRemove(isUpdate);
        log.LogInfo("RemoveFromAvatar complete");
    }

    public void UpdateAvatar()
    {
        RemoveFromAvatar(true);
        AddToAvatar(true);
        log.LogInfo("RemoveFromAvatar complete");
    }

    // TODO temporary validation, return true if installed, false if not
    public bool ValidateIsAdded()
    {
        return cutePrefab.GetStatus() != ApplyStatus.ADD
            || cuteParams.GetStatus() != ApplyStatus.ADD
            || cuteSubmenu.GetStatus() != ApplyStatus.ADD
            || cuteLayers.GetStatus() != ApplyStatus.ADD;
    }

}
