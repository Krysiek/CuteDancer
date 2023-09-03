#if VRC_SDK_VRCSDK3
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    public class CuteBuilder
    {
        ParameterBuilder parameterBuilder = new ParameterBuilder();
        MenuBuilder menuBuilder = new MenuBuilder();
        AnimatorBuilder animatorBuilder = new AnimatorBuilder();

        public void MakeBuild(SettingsData settings)
        {
            Directory.CreateDirectory(settings.outputDirectory);
            AssetDatabase.Refresh();

            parameterBuilder.MakeBuild();
            menuBuilder.MakeBuild();
            animatorBuilder.MakeBuild();
        }
        
        public void ClearBuild(SettingsData settings)
        {
            AssetDatabase.DeleteAsset(settings.outputDirectory);
        }

    }
}

#endif