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
        enum Status
        {
            FORM, EMPTY, ADDED, MISSING, DIFFERENCE, UNKNOWN
        }

        // TODO read from build configuration
        static string ACTION_CTRL = Path.Combine("Assets", "CuteDancer", "Build", "CuteDancer-Action.controller");
        static string FX_CTRL = Path.Combine("Assets", "CuteDancer", "Build", "CuteDancer-FX.controller");

        Status validStat = Status.FORM;
        AvatarDescriptor avatar;
        AnimatorController actionCtrl;
        AnimatorController fxCtrl;

        bool actionWD = false;
        bool fxWD = false;

        public void RenderForm()
        {
            validStat = Validate();

            GUIStyle labelStyle = new GUIStyle(EditorStyles.largeLabel);
            labelStyle.wordWrap = true;

            GUILayout.Label("Select Action and FX controllers used by your avatar.", EditorStyles.largeLabel);
            var newActionCtrl = EditorGUILayout.ObjectField("Action", actionCtrl, typeof(AnimatorController), false, GUILayout.ExpandWidth(true)) as AnimatorController;
            actionWD = EditorGUILayout.Toggle(new GUIContent("Write Defaults", "VRChat default is OFF, but most avatars has this setting set to ON."), actionWD);
            var newFxCtrl = EditorGUILayout.ObjectField("FX", fxCtrl, typeof(AnimatorController), false, GUILayout.ExpandWidth(true)) as AnimatorController;
            fxWD = EditorGUILayout.Toggle(new GUIContent("Write Defaults", "VRChat default is OFF, but most avatars has this setting set to ON."), fxWD);

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            if (validStat == Status.DIFFERENCE)
            {
                CuteButtons.RenderButton("Update animator layers", CuteIcons.ADD, HandleUpdate);
            }
            else
            {
                CuteButtons.RenderButton("Add animator layers", CuteIcons.ADD, () => HandleAdd(),
                                !(validStat == Status.EMPTY || validStat == Status.MISSING));
            }

            CuteButtons.RenderButton("Remove", CuteIcons.REMOVE, () => HandleRemove(),
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

            if (actionCtrl)
            {
                actionWD = CuteAnimators.IsAnimatorUsingWD(actionCtrl);
            }
            if (fxCtrl)
            {
                fxWD = CuteAnimators.IsAnimatorUsingWD(fxCtrl);
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
            if (!actionCtrl && !CreateController(AvatarDescriptor.AnimLayerType.Action, $"{avatar.name}-Action", silent))
            {
                return;
            }

            if (!fxCtrl && !CreateController(AvatarDescriptor.AnimLayerType.FX, $"{avatar.name}-FX", silent))
            {
                return;
            }

            DoBackup();

            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath(ACTION_CTRL, typeof(AnimatorController)) as AnimatorController;
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath(FX_CTRL, typeof(AnimatorController)) as AnimatorController;

            Array.ForEach(srcActionCtrl.layers, l => Array.ForEach(l.stateMachine.states, s => s.state.writeDefaultValues = actionWD));
            Array.ForEach(srcFxCtrl.layers, l => Array.ForEach(l.stateMachine.states, s => s.state.writeDefaultValues = fxWD));

            Debug.Log("Merging controllers [source=" + srcActionCtrl.name + ", desitnation=" + actionCtrl.name + "]");
            VRF.VRLabs.AV3Manager.AnimatorCloner.MergeControllers(actionCtrl, srcActionCtrl);
            Debug.Log("Merging controllers [source=" + srcFxCtrl.name + ", desitnation=" + fxCtrl.name + "]");
            VRF.VRLabs.AV3Manager.AnimatorCloner.MergeControllers(fxCtrl, srcFxCtrl);

            Array.ForEach(srcActionCtrl.layers, l => Array.ForEach(l.stateMachine.states, s => s.state.writeDefaultValues = true));
            Array.ForEach(srcFxCtrl.layers, l => Array.ForEach(l.stateMachine.states, s => s.state.writeDefaultValues = true));

            AssetDatabase.SaveAssets();

            CuteAnimators.UpdateVrcAnimatorLayerControlAfterClone(actionCtrl, !actionWD);
        }

        public void HandleRemove()
        {
            HandleRemove(false);
        }

        void HandleRemove(bool silent = false)
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

        // TODO temporary validation, true - instaled, false - not
        public bool TempValidate()
        {
            var status = Validate();
            return status == Status.ADDED || status == Status.DIFFERENCE || status == Status.UNKNOWN;
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