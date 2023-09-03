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
    public class ParameterBuilder
    {

        public void MakeBuild()
        {

            // var path = $"Assets/{avatar.name}-ExpressionParams.asset";
            // var ok = EditorUtility.DisplayDialog("CuteScript", $"It seems your avatar does not have expression parameters. Empty one will be created and assigned to your avatar.\n\nNew asset will be saved under path:\n{path}", "Create it!", "Cancel");
            // if (!ok)
            // {
            //     EditorUtility.DisplayDialog("CuteScript", "Operation aborted. Expresion Params are NOT added!", "OK");
            //     return false;
            // }

            // var emptyParams = new ExpressionParameters();
            // emptyParams.parameters = new ExpressionParameters.Parameter[0];

            // AssetDatabase.CreateAsset(emptyParams, path);
            // expressionParams = AssetDatabase.LoadAssetAtPath<ExpressionParameters>(path);

            // avatar.expressionParameters = expressionParams;
            // avatar.customExpressions = true;
            // EditorUtility.SetDirty(expressionParams);
            // EditorUtility.SetDirty(avatar);



        }

    }
}

#endif