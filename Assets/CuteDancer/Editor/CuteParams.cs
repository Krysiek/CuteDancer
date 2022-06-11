using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    public class CuteParams : CuteGroup
    {
        static string PARAMS_REF = "Assets/CuteDancer/VRCParams_Example.asset";

        enum Status
        {
            FORM, EMPTY, ADDED
        }

        ExpressionParameters expressionParams;

        public void RenderForm()
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.largeLabel);
            labelStyle.wordWrap = true;

            GUILayout.Label("Select expression parameters used by your avatar", labelStyle);
            expressionParams = EditorGUILayout.ObjectField("Expression Parameters", expressionParams, typeof(ExpressionParameters), false, GUILayout.ExpandWidth(true)) as ExpressionParameters;

            GUILayout.Space(10);

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);
            buttonStyle.fixedHeight = 30;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Add expression parameters", CuteIcons.ADD), buttonStyle))
            {
                HandleAdd();
            }
            if (GUILayout.Button(new GUIContent("Remove", CuteIcons.REMOVE), buttonStyle, GUILayout.Width(150)))
            {
                HandleRemove();
            }
            GUILayout.EndHorizontal();
        }

        public void RenderStatus()
        {
            switch (Validate())
            {
                case Status.FORM:
                    CuteInfoBox.RenderInfoBox(CuteIcons.INFO, "Please select expression parameters asset.");
                    break;
                case Status.EMPTY:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "Expression parameters are not added.");
                    break;
                case Status.ADDED:
                    CuteInfoBox.RenderInfoBox(CuteIcons.OK, "Expression parameters are added.");
                    break;
            }
        }

        public void SetAvatar(AvatarDescriptor avatar)
        {
            expressionParams = avatar.expressionParameters;
        }

        public void ClearForm()
        {
            expressionParams = null;
        }

        void HandleAdd()
        {
            switch (Validate())
            {
                case Status.ADDED:
                case Status.FORM:
                    EditorUtility.DisplayDialog("CuteScript", "Option disabled.", "OK");
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

        void HandleRemove()
        {
            switch (Validate())
            {
                case Status.EMPTY:
                case Status.FORM:
                    EditorUtility.DisplayDialog("CuteScript", "Option disabled.", "OK");
                    return;
            }

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

        Status Validate()
        {
            if (expressionParams == null)
            {
                return Status.FORM;
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
                return Status.EMPTY;
            }
            return Status.ADDED;
        }
    }
}