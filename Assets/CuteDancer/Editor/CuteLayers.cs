using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

namespace VRF
{
    public class CuteLayers : CuteGroup
    {
        enum Status
        {
            FORM, EMPTY, ADDED, UNKNOWN
        }

        static string ACTION_CTRL = "Assets/CuteDancer/Ctrl_Action_Example.controller";
        static string FX_CTRL = "Assets/CuteDancer/Ctrl_FX_Example.controller";

        AnimatorController actionCtrl;
        AnimatorController fxCtrl;

        public void RenderForm()
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.largeLabel);
            labelStyle.wordWrap = true;

            GUILayout.Label("Select Action and FX controllers used by your avatar.", EditorStyles.largeLabel);
            actionCtrl = EditorGUILayout.ObjectField("Action", actionCtrl, typeof(AnimatorController), false, GUILayout.ExpandWidth(true)) as AnimatorController;
            fxCtrl = EditorGUILayout.ObjectField("FX", fxCtrl, typeof(AnimatorController), false, GUILayout.ExpandWidth(true)) as AnimatorController;

            GUILayout.Space(10);

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);
            buttonStyle.fixedHeight = 30;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Add animator layers", CuteIcons.ADD), buttonStyle))
            {
                HandleAdd();
            };
            if (GUILayout.Button(new GUIContent("Remove", CuteIcons.REMOVE), buttonStyle, GUILayout.Width(150)))
            {
                HandleRemove();
            };
            GUILayout.EndHorizontal();
        }

        public void RenderStatus()
        {
            switch (Validate())
            {
                case Status.FORM:
                    CuteInfoBox.RenderInfoBox(CuteIcons.INFO, "Please select animator controllers.");
                    break;
                case Status.EMPTY:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "Layers are not added.");
                    break;
                case Status.ADDED:
                    CuteInfoBox.RenderInfoBox(CuteIcons.OK, "Layers are added.");
                    break;
                case Status.UNKNOWN:
                    CuteInfoBox.RenderInfoBox(CuteIcons.ERROR, "Layers are in mixed state. Please edit them manually.");
                    break;
            }
        }

        public void SetAvatar(AvatarDescriptor avatarDescriptor)
        {
            actionCtrl = Array.Find(avatarDescriptor.baseAnimationLayers, layer => layer.type == AvatarDescriptor.AnimLayerType.Action).animatorController as AnimatorController;
            fxCtrl = Array.Find(avatarDescriptor.baseAnimationLayers, layer => layer.type == AvatarDescriptor.AnimLayerType.FX).animatorController as AnimatorController;
        }

        public void ClearForm()
        {
            actionCtrl = null;
            fxCtrl = null;
        }

        void HandleAdd()
        {
            switch (Validate())
            {
                case Status.ADDED:
                case Status.FORM:
                case Status.UNKNOWN:
                    EditorUtility.DisplayDialog("CuteScript", "Option disabled.", "OK");
                    return;
            }

            DoBackup();

            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath(ACTION_CTRL, typeof(AnimatorController)) as AnimatorController;
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath(FX_CTRL, typeof(AnimatorController)) as AnimatorController;

            Debug.Log("Merging controllers [source=" + srcActionCtrl.name + ", desitnation=" + actionCtrl.name + "]");
            VRF.VRLabs.AV3Manager.AnimatorCloner.MergeControllers(actionCtrl, srcActionCtrl);
            Debug.Log("Merging controllers [source=" + srcFxCtrl.name + ", desitnation=" + fxCtrl.name + "]");
            VRF.VRLabs.AV3Manager.AnimatorCloner.MergeControllers(fxCtrl, srcFxCtrl);
        }

        void HandleRemove()
        {
            switch (Validate())
            {
                case Status.EMPTY:
                case Status.FORM:
                case Status.UNKNOWN:
                    EditorUtility.DisplayDialog("CuteScript", "Option disabled.", "OK");
                    return;
            }

            DoBackup();

            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath(ACTION_CTRL, typeof(AnimatorController)) as AnimatorController;
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath(FX_CTRL, typeof(AnimatorController)) as AnimatorController;

            for (int i = 0; i < srcActionCtrl.layers.Length; i++)
            {
                this.RemoveLayer(actionCtrl, srcActionCtrl.layers[i].name);
            }

            for (int i = 0; i < srcFxCtrl.layers.Length; i++)
            {
                this.RemoveLayer(fxCtrl, srcFxCtrl.layers[i].name);
            }
        }

        void RemoveLayer(AnimatorController controller, string name)
        {
            int removeIx = Array.FindIndex(controller.layers, l => l.name == name);
            if (removeIx >= 0)
            {
                Debug.Log("Removing layer [name=" + name + ", index=" + removeIx + "] from controller [name=" + controller.name + "]");
                controller.RemoveLayer(removeIx);
            }
            else
            {
                Debug.Log("Layer [name=" + name + "] not found in controller [name=" + controller.name + "]");
            }
        }

        void DoBackup()
        {
            CuteBackup.CreateBackup(AssetDatabase.GetAssetPath(actionCtrl));
            CuteBackup.CreateBackup(AssetDatabase.GetAssetPath(fxCtrl));
        }

        Status Validate()
        {
            if (!actionCtrl || !fxCtrl)
            {
                return Status.FORM;
            }

            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath(ACTION_CTRL, typeof(AnimatorController)) as AnimatorController;
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath(FX_CTRL, typeof(AnimatorController)) as AnimatorController;

            bool actionValid = CheckLayersExists(actionCtrl, srcActionCtrl);
            bool fxValid = CheckLayersExists(fxCtrl, srcFxCtrl);

            if (actionValid && fxValid)
            {
                return Status.ADDED;
            }
            if (!actionValid && !fxValid)
            {
                return Status.EMPTY;
            }
            return Status.UNKNOWN;

        }

        bool CheckLayersExists(AnimatorController controller, AnimatorController refCtrl)
        {
            for (int i = 0; i < refCtrl.layers.Length; i++)
            {
                string layerName = refCtrl.layers[i].name;
                if (!Array.Exists(controller.layers, l => l.name == layerName))
                {
                    return false;
                }
            }
            return true;
        }

    }
}