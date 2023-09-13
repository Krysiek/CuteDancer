using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Core.Tokens;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

public class AvatarApplyService
{
    private AvatarDescriptor _avatar;
    public AvatarDescriptor avatar
    {
        get => _avatar;
        set
        {
            _avatar = value;
            // TODO SetAvatar of all sub-appliers
        }
    }

    void BuildAvatar()
    {
        // TODO
    }

}
