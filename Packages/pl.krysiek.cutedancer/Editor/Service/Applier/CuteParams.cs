using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using System.IO;

namespace VRF
{
    public class CuteParams : AvatarApplierInterface
    {
        // TODO read from build configuration
        static string PARAMS_REF = Path.Combine("Assets", "CuteDancer", "Build", "CuteDancer-VRCParams.asset");

        AvatarDescriptor avatar;
        ExpressionParameters expressionParams;

        public void SetAvatar(AvatarDescriptor avatarDescriptor)
        {
            avatar = avatarDescriptor;
            expressionParams = avatarDescriptor.expressionParameters;
        }

        public void ClearForm()
        {
            avatar = null;
            expressionParams = null;
        }

        public void HandleAdd()
        {
            if (!expressionParams && !CreateExpressionParams())
            {
                return;
            }

            DoBackup();

            ExpressionParameters paramsRef = AssetDatabase.LoadAssetAtPath(PARAMS_REF, typeof(ExpressionParameters)) as ExpressionParameters;

            var paramsRefList = new List<ExpressionParameters.Parameter>(paramsRef.parameters);
            var paramsList = new List<ExpressionParameters.Parameter>(expressionParams.parameters);

            paramsRefList.ForEach(paramRef =>
            {
                if (!paramsList.Exists(param => param.name == paramRef.name))
                {
                    var newParam = new ExpressionParameters.Parameter();
                    newParam.name = paramRef.name;
                    newParam.saved = paramRef.saved;
                    newParam.valueType = paramRef.valueType;
                    newParam.defaultValue = paramRef.defaultValue;
                    Debug.Log("Adding parameter [name=" + paramRef.name + "]");
                    paramsList.Add(newParam);
                }
            });

            Debug.Log("Updating expression parameters asset.");
            expressionParams.parameters = paramsList.ToArray();
            EditorUtility.SetDirty(expressionParams);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public void HandleRemove()
        {
            DoBackup();

            ExpressionParameters paramsRef = AssetDatabase.LoadAssetAtPath(PARAMS_REF, typeof(ExpressionParameters)) as ExpressionParameters;

            var paramsRefList = new List<ExpressionParameters.Parameter>(paramsRef.parameters);
            Debug.Log("Skip removing commonly used parameter [name=VRCEmote]");
            paramsRefList = paramsRefList.FindAll(param => param.name != "VRCEmote");
            var paramsList = new List<ExpressionParameters.Parameter>(expressionParams.parameters);

            paramsRefList.ForEach(paramRef =>
            {
                var ix = paramsList.FindIndex(param => param.name == paramRef.name);
                if (ix >= 0)
                {
                    Debug.Log("Removing parameter [name=" + paramRef.name + ", index=" + ix + "]");
                    paramsList.RemoveAt(ix);
                }
            });

            Debug.Log("Updating expression parameters asset.");
            expressionParams.parameters = paramsList.ToArray();
            EditorUtility.SetDirty(expressionParams);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void DoBackup()
        {
            CuteBackup.CreateBackup(AssetDatabase.GetAssetPath(expressionParams));
        }

        public ApplyStatus GetStatus()
        {
            if (!avatar)
            {
                return ApplyStatus.EMPTY;
            }
            if (!expressionParams)
            {
                return ApplyStatus.ADD;
            }
            ExpressionParameters paramsRef = AssetDatabase.LoadAssetAtPath(PARAMS_REF, typeof(ExpressionParameters)) as ExpressionParameters;

            bool notFound = false;

            Array.ForEach(paramsRef.parameters, param =>
            {
                if (expressionParams.FindParameter(param.name) == null)
                {
                    notFound = true;
                }
            });
            if (notFound)
            {
                return ApplyStatus.ADD;
            }
            return ApplyStatus.REMOVE;
        }

        bool CreateExpressionParams()
        {
            var path = $"Assets/{avatar.name}-ExpressionParams.asset";
            var ok = EditorUtility.DisplayDialog("CuteScript", $"It seems your avatar does not have expression parameters. Empty one will be created and assigned to your avatar.\n\nNew asset will be saved under path:\n{path}", "Create it!", "Cancel");
            if (!ok)
            {
                EditorUtility.DisplayDialog("CuteScript", "Operation aborted. Expresion Params are NOT added!", "OK");
                return false;
            }

            var emptyParams = new ExpressionParameters();
            emptyParams.parameters = new ExpressionParameters.Parameter[0];

            AssetDatabase.CreateAsset(emptyParams, path);
            expressionParams = AssetDatabase.LoadAssetAtPath<ExpressionParameters>(path);

            avatar.expressionParameters = expressionParams;
            avatar.customExpressions = true;
            EditorUtility.SetDirty(expressionParams);
            EditorUtility.SetDirty(avatar);

            return true;
        }
    }
}