using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRF;
using YamlDotNet.Core.Tokens;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

public class AvatarApplyService
{
    CutePrefab cutePrefab = new CutePrefab();
    CuteParams cuteParams = new CuteParams();
    CuteSubmenu cuteSubmenu = new CuteSubmenu();
    CuteLayers cuteLayers = new CuteLayers();

    private AvatarDescriptor _avatar;
    public AvatarDescriptor avatar
    {
        get => _avatar;
        set
        {
            _avatar = value;
            if (value)
            {
                cutePrefab.SetAvatar(value);
                cuteParams.SetAvatar(value);
                cuteSubmenu.SetAvatar(value);
                cuteLayers.SetAvatar(value);
            }
            else
            {
                cutePrefab.ClearForm();
                cuteParams.ClearForm();
                cuteSubmenu.ClearForm();
                cuteLayers.ClearForm();
            }
        }
    }

    public void AddToAvatar()
    {
        cutePrefab.HandleAdd();
        cuteParams.HandleAdd();
        cuteSubmenu.HandleAdd();
        cuteLayers.HandleAdd();
    }

    public void RemoveFromAvatar()
    {
        cutePrefab.HandleRemove();
        cuteParams.HandleRemove();
        cuteSubmenu.HandleRemove();
        cuteLayers.HandleRemove();
    }

    // TODO temporary validation, return true if installed, false if not
    public bool Validate()
    {
        return cuteLayers.TempValidate();
    }

}
