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
            FORM, EMPTY, ADDED, MISSING, DIFFERENCE, UNKNOWN
        }

        static string ACTION_CTRL = "Assets/CuteDancer/Ctrl_Action_Example.controller";
        static string FX_CTRL = "Assets/CuteDancer/Ctrl_FX_Example.controller";

        Status validStat = Status.FORM;
        AvatarDescriptor avatar;
        AnimatorController actionCtrl;
        AnimatorController fxCtrl;

        public void RenderForm()
        {
            validStat = Validate();

            GUIStyle labelStyle = new GUIStyle(EditorStyles.largeLabel);
            labelStyle.wordWrap = true;

            GUILayout.Label("Select Action and FX controllers used by your avatar.", EditorStyles.largeLabel);
            actionCtrl = EditorGUILayout.ObjectField("Action", actionCtrl, typeof(AnimatorController), false, GUILayout.ExpandWidth(true)) as AnimatorController;
            fxCtrl = EditorGUILayout.ObjectField("FX", fxCtrl, typeof(AnimatorController), false, GUILayout.ExpandWidth(true)) as AnimatorController;

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (validStat == Status.DIFFERENCE)
            {
                CuteButtons.RenderButton("Update animator layers", CuteIcons.ADD, HandleUpdate);
            }
            else
            {
                CuteButtons.RenderButton("Add animator layers", CuteIcons.ADD, HandleAdd,
                                !(validStat == Status.EMPTY || validStat == Status.MISSING));
            }

            CuteButtons.RenderButton("Remove", CuteIcons.REMOVE, HandleRemove,
                !(validStat == Status.ADDED || validStat == Status.UNKNOWN), GUILayout.Width(150));

            GUILayout.EndHorizontal();
        }

        public void RenderStatus()
        {
            switch (validStat)
            {
                case Status.FORM:
                    CuteInfoBox.RenderInfoBox(CuteIcons.INFO, "Please select animator controllers.");
                    break;
                case Status.EMPTY:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "Layers are not added.");
                    break;
                case Status.MISSING:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "Layers are not added (missing controllers will be created).");
                    break;
                case Status.ADDED:
                    CuteInfoBox.RenderInfoBox(CuteIcons.OK, "Layers are added.");
                    break;
                case Status.DIFFERENCE:
                    CuteInfoBox.RenderInfoBox(CuteIcons.WARN, "Layers are out of date. Press update button to fix it.");
                    break;
                case Status.UNKNOWN:
                    CuteInfoBox.RenderInfoBox(CuteIcons.ERROR, "Layers are in mixed state. You can still try to remove any existing layers using Remove button and re-add them.");
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
            avatar = null;
            actionCtrl = null;
            fxCtrl = null;
        }

        void HandleAdd()
        {
            if (!actionCtrl && !CreateController(AvatarDescriptor.AnimLayerType.Action, $"{avatar.name}-Action"))
            {
                return;
            }

            if (!fxCtrl && !CreateController(AvatarDescriptor.AnimLayerType.FX, $"{avatar.name}-FX"))
            {
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
            if (validStat == Status.UNKNOWN)
            {
                if (!EditorUtility.DisplayDialog("CuteScript", "The script will try to remove CuteDance layers from your animators if any exists.\n\nThey are matched by name though, so it may not help.", "Let's try", "Cancel"))
                {
                    return;
                }
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

        void HandleUpdate()
        {
            // yolo
            HandleRemove();
            HandleAdd();
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
            if (!actionCtrl || !fxCtrl)
            {
                return Status.MISSING;
            }

            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath(ACTION_CTRL, typeof(AnimatorController)) as AnimatorController;
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath(FX_CTRL, typeof(AnimatorController)) as AnimatorController;

            bool actionHasLayers = CheckLayersExists(actionCtrl, srcActionCtrl, out bool actionDiffs);
            bool fxHasLayers = CheckLayersExists(fxCtrl, srcFxCtrl, out bool fxDiffs);

            if (actionHasLayers && fxHasLayers)
            {
                if (actionDiffs || fxDiffs)
                {
                    return Status.DIFFERENCE;
                }
                return Status.ADDED;
            }
            if (!actionHasLayers && !fxHasLayers)
            {
                return Status.EMPTY;
            }
            return Status.UNKNOWN;

        }

        bool CheckLayersExists(AnimatorController controller, AnimatorController refCtrl, out bool diffs)
        {
            diffs = false;
            for (int i = 0; i < refCtrl.layers.Length; i++)
            {
                string layerName = refCtrl.layers[i].name;
                AnimatorControllerLayer layer = Array.Find(controller.layers, l => l.name == layerName);
                if (layer == null)
                {
                    return false;
                }
                if (!CompareLayers(layer, refCtrl.layers[i]))
                {
                    diffs = true;
                }
            }
            return true;
        }

        bool CompareLayers(AnimatorControllerLayer layer, AnimatorControllerLayer refLayer)
        {
            if (refLayer.stateMachine.states.Length != layer.stateMachine.states.Length)
            {
                Debug.Log($"States count is different [layerName={layer.name}, dest={layer.stateMachine.states.Length}, ref={refLayer.stateMachine.states.Length}]");
                return false;
            }
            for (int i = 0; i < refLayer.stateMachine.states.Length; i++)
            {
                var state = layer.stateMachine.states[i].state;
                var refState = refLayer.stateMachine.states[i].state;
                if (refLayer.stateMachine.states.Length != layer.stateMachine.states.Length)
                {
                    Debug.Log($"Transitions count is different [layerName={layer.name}, stateName={state.name}, dest={state.transitions.Length}, ref={refState.transitions.Length}]");
                    return false;
                }
            }

            return true;
        }

        bool CreateController(AnimLayerType type, string name)
        {
            var ok = EditorUtility.DisplayDialog("CuteScript", $"It seems your avatar does not have {(AnimLayerType)type} animator. Empty one will be created and assigned to your avatar.\n\nNew animator will be saved under path:\nAssets/{name}.controller", "Create it!", "Cancel");
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