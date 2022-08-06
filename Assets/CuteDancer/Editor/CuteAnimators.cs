using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using AvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using VRCAnimatorLayerControl = VRC.SDK3.Avatars.Components.VRCAnimatorLayerControl;
using CustomAnimLayer = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.CustomAnimLayer;
using AnimLayerType = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor.AnimLayerType;

namespace VRF
{
    class CuteAnimators
    {
        static string DEF_ACTION_CTRL = "Assets/VRCSDK/Examples3/Animation/Controllers/vrc_AvatarV3ActionLayer.controller";

        public static AnimatorController CreateDefaultAnimator(AnimLayerType type, String path)
        {

            AnimatorController emptyCtrl = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path);

            if (type == AnimLayerType.Action)
            {
                if (AssetDatabase.CopyAsset(DEF_ACTION_CTRL, path))
                {
                    emptyCtrl = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
                }
                else
                {
                    Debug.LogError("Default Action animator not found in SDK. AFK and default animations may be broken!");
                }
            }

            EditorUtility.SetDirty(emptyCtrl);

            return emptyCtrl;

        }

        public static bool IsAnimatorEmpty(AnimLayerType type, AnimatorController animator)
        {
            if (animator.layers.Length == 1)
            {
                var baseLayer = animator.layers[0];
                if (baseLayer.stateMachine.states.Length == 0 && baseLayer.stateMachine.stateMachines.Length == 0)
                {
                    return true;
                }

                if (type == AnimLayerType.Action)
                {
                    // check if base layer equals to VRCSDK default
                    AnimatorController defActionCtrl = AssetDatabase.LoadAssetAtPath(DEF_ACTION_CTRL, typeof(AnimatorController)) as AnimatorController;
                    var defBaseLayer = defActionCtrl.layers[0];
                    if (CompareLayers(baseLayer, defBaseLayer))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static AnimatorController GetAnimatorByType(AvatarDescriptor avatar, AnimLayerType type)
        {
            return Array.Find(avatar.baseAnimationLayers, layer => layer.type == type).animatorController as AnimatorController;
        }

        public static CustomAnimLayer CreateCustomAnimLayer(AnimLayerType type, AnimatorController animator = null)
        {
            CustomAnimLayer animLayer = new CustomAnimLayer();
            animLayer.isEnabled = !!animator;
            animLayer.type = type;
            animLayer.animatorController = animator;
            animLayer.mask = null;
            animLayer.isDefault = !animator;

            return animLayer;
        }

        public static bool CompareLayers(AnimatorControllerLayer layer, AnimatorControllerLayer refLayer)
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

                if (state.transitions.Length != refState.transitions.Length)
                {
                    Debug.Log($"Transitions count is different [layerName={layer.name}, stateName={state.name}, dest={state.transitions.Length}, ref={refState.transitions.Length}]");
                    return false;
                }

                if (state.behaviours.Length != refState.behaviours.Length) {
                    Debug.Log($"Behaviours count is different [layerName={layer.name}, stateName={state.name}, dest={state.behaviours.Length}, ref={refState.behaviours.Length}]");
                    return false;
                }
            }

            return true;
        }

        public static bool IsAnimatorUsingWD(AnimatorController ctrl)
        {
            return Array.Exists(ctrl.layers, l => Array.Exists(l.stateMachine.states, s => s.state.writeDefaultValues == true));
        }

        public static void UpdateVrcAnimatorLayerControlAfterClone(AnimatorController ctrl, bool enableLayerWeight)
        {
            int cdIndex = Array.FindIndex(ctrl.layers, l => l.name.StartsWith("CuteDancer"));
            foreach (var state in ctrl.layers[cdIndex].stateMachine.states)
            {
                foreach (StateMachineBehaviour behaviour in state.state.behaviours)
                {
                    var vrcLayerControl = behaviour as VRCAnimatorLayerControl;
                    if (vrcLayerControl?.layer != null)
                    {
                        Debug.Log($"Update VRCAnimatorLayerControl behaviour activity to enabled");
                        var rawVrcLayerControl = new SerializedObject(vrcLayerControl);
                        rawVrcLayerControl.FindProperty("m_Enabled").intValue = enableLayerWeight ? 1 : 0;
                        rawVrcLayerControl.ApplyModifiedProperties();
                        Debug.Log($"Update VRCAnimatorLayerControl layer number [from={vrcLayerControl.layer}, to={cdIndex}]");
                        vrcLayerControl.layer = cdIndex;
                        EditorUtility.SetDirty(vrcLayerControl);
                    }
                }
            }
        }
    }

}