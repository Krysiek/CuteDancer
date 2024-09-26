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
        private static Logger log = new Logger("CuteLayers");

        static string ACTION_CTRL_PATH = Path.Combine("CuteDancer-Action.controller");
        static string FX_CTRL_PATH = Path.Combine("CuteDancer-FX.controller");

        private AnimatorController actionCtrl;
        private AnimatorController fxCtrl;

        private string buildPath;
        public string BuildPath
        {
            set => buildPath = value;
        }

        private string ActionCtrlPath => Path.Combine(buildPath, ACTION_CTRL_PATH);
        private string FxCtrlPath => Path.Combine(buildPath, FX_CTRL_PATH);

        private AvatarDescriptor avatar;
        public AvatarDescriptor Avatar
        {
            set
            {
                if (!value)
                {
                    avatar = null;
                    actionCtrl = null;
                    fxCtrl = null;
                    ActionWD = false;
                    FxWD = false;
                    return;
                }

                avatar = value;
                actionCtrl = Array.Find(value.baseAnimationLayers, layer => layer.type == AnimLayerType.Action).animatorController as AnimatorController;
                fxCtrl = Array.Find(value.baseAnimationLayers, layer => layer.type == AnimLayerType.FX).animatorController as AnimatorController;

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
        }

        public bool ActionWD { get; set; }
        public bool FxWD { get; set; }

        public CuteLayers()
        {
            ActionWD = false;
            FxWD = false;
        }

        public void HandleAdd(bool silent = false)
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

            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(ActionCtrlPath);
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(FxCtrlPath);

            Array.ForEach(srcActionCtrl.layers, l => Array.ForEach(l.stateMachine.states, s => s.state.writeDefaultValues = ActionWD));
            Array.ForEach(srcFxCtrl.layers, l => Array.ForEach(l.stateMachine.states, s => s.state.writeDefaultValues = FxWD));

            log.LogDebug("Merging controllers [source=" + srcActionCtrl.name + ", desitnation=" + actionCtrl.name + "]");
            VRLabs.AV3Manager.AnimatorCloner.MergeControllers(actionCtrl, srcActionCtrl);
            log.LogDebug("Merging controllers [source=" + srcFxCtrl.name + ", desitnation=" + fxCtrl.name + "]");
            VRLabs.AV3Manager.AnimatorCloner.MergeControllers(fxCtrl, srcFxCtrl);

            EditorUtility.ClearDirty(srcActionCtrl);
            EditorUtility.ClearDirty(srcFxCtrl);

            AssetDatabase.SaveAssets();

            CuteAnimators.UpdateVrcAnimatorLayerControlAfterClone(actionCtrl, !ActionWD);
        }

        public void HandleRemove(bool silent = false)
        {
            DoBackup();

            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath(ActionCtrlPath, typeof(AnimatorController)) as AnimatorController;
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath(FxCtrlPath, typeof(AnimatorController)) as AnimatorController;

            if (actionCtrl)
            {
                for (int i = 0; i < srcActionCtrl.layers.Length; i++)
                {
                    this.RemoveLayer(actionCtrl, srcActionCtrl.layers[i].name);
                }
            }

            if (fxCtrl)
            {
                for (int i = 0; i < srcFxCtrl.layers.Length; i++)
                {
                    this.RemoveLayer(fxCtrl, srcFxCtrl.layers[i].name);
                }
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
                    Avatar = avatar;
                }
            }
        }

        void RemoveLayer(AnimatorController controller, string name)
        {
            int removeIx = Array.FindIndex(controller.layers, l => l.name == name);
            if (removeIx >= 0)
            {
                log.LogDebug("Removing layer [name=" + name + ", index=" + removeIx + "] from controller [name=" + controller.name + "]");
                controller.RemoveLayer(removeIx);
            }
            else
            {
                log.LogDebug("Layer [name=" + name + "] not found in controller [name=" + controller.name + "]");
            }
        }

        void DoBackup()
        {
            CuteBackup.CreateBackup(AssetDatabase.GetAssetPath(actionCtrl), avatar.name);
            CuteBackup.CreateBackup(AssetDatabase.GetAssetPath(fxCtrl), avatar.name);
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
            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(ActionCtrlPath);
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(FxCtrlPath);

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
            Avatar = avatar; // refresh local vars

            return true;
        }

    }
}