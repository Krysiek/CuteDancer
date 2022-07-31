using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using CustomAnimLayer = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.CustomAnimLayer;
using AnimLayerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType;

namespace VRF
{
    public class CuteLayers : CuteGroup
    {
        enum Status
        {
            FORM, MISSING_ACTION, MISSING_FX, EMPTY, ADDED, UNKNOWN
        }

        static string ACTION_CTRL = "Assets/CuteDancer/Ctrl_Action_Example.controller";
        static string FX_CTRL = "Assets/CuteDancer/Ctrl_FX_Example.controller";

        AvatarDescriptor avatar;
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
                case Status.MISSING_ACTION:
                case Status.MISSING_FX:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "Layers are not added (missing controllers will be created).");
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
            avatar = avatarDescriptor;
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
                case Status.MISSING_ACTION:
                    if (!CreateController(AvatarDescriptor.AnimLayerType.Action, $"{avatar.name}-Action"))
                    {
                        return;
                    }
                    goto case Status.MISSING_FX;
                case Status.MISSING_FX:
                    if (!CreateController(AvatarDescriptor.AnimLayerType.FX, $"{avatar.name}-FX"))
                    {
                        return;
                    }
                    break;
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
            if (!avatar)
            {
                return Status.FORM;
            }
            if (!actionCtrl)
            {
                return Status.MISSING_ACTION;
            }
            if (!fxCtrl)
            {
                return Status.MISSING_FX;
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

        bool CreateController(AnimLayerType type, string name)
        {
            var ok = EditorUtility.DisplayDialog("CuteScript", $"It seems your avatar does not have {(AnimLayerType)type} animator. Empty one will be created and assigned to your avatar.\n\nNew animator will be saved under path:\nAssets/{name}.controller", "OK", "Cancel");
            if (!ok)
            {
                EditorUtility.DisplayDialog("CuteScript", "Operation aborted. Layers are NOT added!", "OK");
                return false;
            }

            var emptyCtrl = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"Assets/{name}.controller");
            CustomAnimLayer animLayer = new CustomAnimLayer();
            animLayer.isEnabled = true;
            animLayer.type = type;
            animLayer.animatorController = emptyCtrl;
            animLayer.mask = null;
            animLayer.isDefault = false;

            int index = Array.FindIndex(avatar.baseAnimationLayers, layer => layer.type == type);

            avatar.baseAnimationLayers[index] = animLayer;

            EditorUtility.SetDirty(avatar);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            SetAvatar(avatar); // refresh local vars

            return true;
        }

    }
}