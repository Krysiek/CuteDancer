using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    public class InstallerViewData : ScriptableObject
    {
        public BuildInfoData build;

        public AvatarDescriptor avatar;

        public GameObject avatarGameObject;
        public VRCExpressionParameters avatarExpressionParameters;
        public VRCExpressionsMenu avatarExpressionsMenu;
        public AnimatorController avatarActionController;
        public AnimatorController avatarFxController;
    }
}