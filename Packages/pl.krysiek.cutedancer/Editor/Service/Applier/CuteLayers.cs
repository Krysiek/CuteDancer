using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using VRCAnimatorLayerControl = VRC.SDK3.Avatars.Components.VRCAnimatorLayerControl;
using CustomAnimLayer = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.CustomAnimLayer;
using AnimLayerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType;
using System.IO;

namespace VRF
{
    public class CuteLayers : AvatarApplierInterface
    {
        // TODO read from build configuration
        static string ACTION_CTRL = Path.Combine("Assets", "CuteDancer", "Build", "CuteDancer-Action.controller");
        static string FX_CTRL = Path.Combine("Assets", "CuteDancer", "Build", "CuteDancer-FX.controller");

        AvatarDescriptor avatar;
        AnimatorController actionCtrl;
        AnimatorController fxCtrl;

        public bool ActionWD { get; set; }
        public bool FxWD { get; set; }

        public CuteLayers()
        {
            ActionWD = false;
            FxWD = false;
        }

        public void SetAvatar(AvatarDescriptor avatarDescriptor)
        {
            avatar = avatarDescriptor;
            actionCtrl = Array.Find(avatarDescriptor.baseAnimationLayers, layer => layer.type == AnimLayerType.Action).animatorController as AnimatorController;
            fxCtrl = Array.Find(avatarDescriptor.baseAnimationLayers, layer => layer.type == AnimLayerType.FX).animatorController as AnimatorController;

            // TODO handle this on UI when checkbox will be available
            if (actionCtrl)
            {
                ActionWD = CuteAnimators.IsAnimatorUsingWD(actionCtrl);
            }
            if (fxCtrl)
            {
                FxWD = CuteAnimators.IsAnimatorUsingWD(fxCtrl);
            }
        }

        public void ClearForm()
        {
            avatar = null;
            actionCtrl = null;
            fxCtrl = null;
        }

        public void HandleAdd()
        {
            HandleAdd(false); // TODO differentiate for updates
        }

        void HandleAdd(bool silent = false)
        {
            if (!actionCtrl && !CreateController(AnimLayerType.Action, $"{avatar.name}-Action", silent))
            {
                return;
            }

            if (!fxCtrl && !CreateController(AnimLayerType.FX, $"{avatar.name}-FX", silent))
            {
                return;
            }

            DoBackup();

            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(ACTION_CTRL);
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(FX_CTRL);

            Array.ForEach(srcActionCtrl.layers, l => Array.ForEach(l.stateMachine.states, s => s.state.writeDefaultValues = ActionWD));
            Array.ForEach(srcFxCtrl.layers, l => Array.ForEach(l.stateMachine.states, s => s.state.writeDefaultValues = FxWD));

            Debug.Log("Merging controllers [source=" + srcActionCtrl.name + ", desitnation=" + actionCtrl.name + "]");
            VRLabs.AV3Manager.AnimatorCloner.MergeControllers(actionCtrl, srcActionCtrl);
            Debug.Log("Merging controllers [source=" + srcFxCtrl.name + ", desitnation=" + fxCtrl.name + "]");
            VRLabs.AV3Manager.AnimatorCloner.MergeControllers(fxCtrl, srcFxCtrl);

            EditorUtility.ClearDirty(srcActionCtrl);
            EditorUtility.ClearDirty(srcFxCtrl);

            AssetDatabase.SaveAssets();

            CuteAnimators.UpdateVrcAnimatorLayerControlAfterClone(actionCtrl, !ActionWD);
        }

        public void HandleRemove()
        {
            HandleRemove(false);
        }

        void HandleRemove(bool silent = false)
        {
            if (GetStatus() == ApplyStatus.BLOCKED)
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

            bool actionCtrlEmpty = CuteAnimators.IsAnimatorEmpty(AnimLayerType.Action, actionCtrl);
            bool fxCtrlEmpty = CuteAnimators.IsAnimatorEmpty(AnimLayerType.FX, fxCtrl);

            if (actionCtrlEmpty || fxCtrlEmpty)
            {
                if (silent || EditorUtility.DisplayDialog("CuteScript", "Some animators remain empty after deleting layers. Do you want to disconnect them from the avatar and restore VRChat default animators?", "Sure", "No"))
                {
                    if (actionCtrlEmpty)
                    {
                        int actionIndex = Array.FindIndex(avatar.baseAnimationLayers, layer => layer.type == AnimLayerType.Action);
                        avatar.baseAnimationLayers[actionIndex] = CuteAnimators.CreateCustomAnimLayer(AnimLayerType.Action);
                    }
                    if (fxCtrlEmpty)
                    {
                        int fxIndex = Array.FindIndex(avatar.baseAnimationLayers, layer => layer.type == AnimLayerType.FX);
                        avatar.baseAnimationLayers[fxIndex] = CuteAnimators.CreateCustomAnimLayer(AnimLayerType.FX);
                    }
                    EditorUtility.SetDirty(avatar);
                    SetAvatar(avatar);
                }
            }
        }

        void HandleUpdate()
        {
            // yolo
            HandleRemove(true);
            HandleAdd(true);
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

        public ApplyStatus GetStatus()
        {
            if (!avatar)
            {
                return ApplyStatus.EMPTY;
            }
            if (!actionCtrl || !fxCtrl)
            {
                return ApplyStatus.ADD;
            }
            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(ACTION_CTRL);
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(FX_CTRL);

            bool actionHasLayers = CheckLayersExists(actionCtrl, srcActionCtrl, out bool actionDiffs);
            bool fxHasLayers = CheckLayersExists(fxCtrl, srcFxCtrl, out bool fxDiffs);

            if (actionHasLayers && fxHasLayers)
            {
                if (actionDiffs || fxDiffs)
                {
                    return ApplyStatus.UPDATE;
                }
                return ApplyStatus.REMOVE;
            }
            if (!actionHasLayers && !fxHasLayers)
            {
                return ApplyStatus.ADD;
            }
            return ApplyStatus.BLOCKED;
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
                if (!CuteAnimators.CompareLayers(layer, refCtrl.layers[i]))
                {
                    diffs = true;
                }
            }
            return true;
        }

        bool CreateController(AnimLayerType type, string name, bool silent = false)
        {
            if (!silent)
            {
                var ok = EditorUtility.DisplayDialog("CuteScript", $"It seems your avatar does not have {(AnimLayerType)type} animator. Empty one will be created and assigned to your avatar.\n\nNew animator will be saved under path:\nAssets/{name}.controller", "Create it!", "Cancel");
                if (!ok)
                {
                    EditorUtility.DisplayDialog("CuteScript", "Operation aborted. Layers are NOT added!", "OK");
                    return false;
                }
            }

            var emptyCtrl = CuteAnimators.CreateDefaultAnimator(type, $"Assets/{name}.controller");
            CustomAnimLayer animLayer = CuteAnimators.CreateCustomAnimLayer(type, emptyCtrl);

            int index = Array.FindIndex(avatar.baseAnimationLayers, layer => layer.type == type);

            avatar.baseAnimationLayers[index] = animLayer;
            avatar.customizeAnimationLayers = true;

            EditorUtility.SetDirty(avatar);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            SetAvatar(avatar); // refresh local vars

            return true;
        }

    }
}