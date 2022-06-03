using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace VRF
{
    public class CuteLayers
    {
        static string ACTION_CTRL = "Assets/CuteDancer/Ctrl_Action_Example.controller";
        static string FX_CTRL = "Assets/CuteDancer/Ctrl_FX_Example.controller";

        AnimatorController actionCtrl;
        AnimatorController fxCtrl;

        public void RenderGUI(GUIStyle labelStyle)
        {
            GUILayout.Label("Select Action and FX controllers used by your avatar.", labelStyle);
            actionCtrl = EditorGUILayout.ObjectField("Action", actionCtrl, typeof(AnimatorController), false, GUILayout.ExpandWidth(true)) as AnimatorController;
            fxCtrl = EditorGUILayout.ObjectField("FX", fxCtrl, typeof(AnimatorController), false, GUILayout.ExpandWidth(true)) as AnimatorController;

            GUILayout.Space(10);

            GUIStyle buttonStyle = new GUIStyle(EditorStyles.miniButton);
            buttonStyle.fixedHeight = 30;
            bool addAction = GUILayout.Button("Add layers to my avatar", buttonStyle);
            bool removeAction = GUILayout.Button("Remove layers from my avatar", buttonStyle);

            if (addAction)
            {
                this.HandleAdd();
            }
            if (removeAction)
            {
                this.HandleRemove();
            }
        }

        void HandleAdd()
        {
            if (!ValidateForm())
            {
                return;
            }

            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath(ACTION_CTRL, typeof(AnimatorController)) as AnimatorController;
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath(FX_CTRL, typeof(AnimatorController)) as AnimatorController;

            if (CheckLayersExists(actionCtrl, srcActionCtrl) || CheckLayersExists(fxCtrl, srcFxCtrl))
            {
                EditorUtility.DisplayDialog("Error", "Layers already added.", "OK");
                return;
            }

            VRF.VRLabs.AV3Manager.AnimatorCloner.MergeControllers(actionCtrl, srcActionCtrl);
            VRF.VRLabs.AV3Manager.AnimatorCloner.MergeControllers(fxCtrl, srcFxCtrl);
        }
        void HandleRemove()
        {
            if (!ValidateForm())
            {
                return;
            }

            AnimatorController srcActionCtrl = AssetDatabase.LoadAssetAtPath(ACTION_CTRL, typeof(AnimatorController)) as AnimatorController;
            AnimatorController srcFxCtrl = AssetDatabase.LoadAssetAtPath(FX_CTRL, typeof(AnimatorController)) as AnimatorController;

            if (!(CheckLayersExists(actionCtrl, srcActionCtrl) || CheckLayersExists(fxCtrl, srcFxCtrl)))
            {
                EditorUtility.DisplayDialog("Error", "Layers already removed.", "OK");
                return;
            }

            for (int i = 0; i < srcActionCtrl.layers.Length; i++)
            {
                this.RemoveLayer(actionCtrl, srcActionCtrl.layers[i].name);
            }

            for (int i = 0; i < srcFxCtrl.layers.Length; i++)
            {
                this.RemoveLayer(fxCtrl, srcFxCtrl.layers[i].name);
            }
        }
        bool ValidateForm()
        {
            if (!actionCtrl || !fxCtrl)
            {
                EditorUtility.DisplayDialog("Validation Error", "Action and FX controllers are required.", "OK");
                return false;
            }
            return true;
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

        void RemoveLayer(AnimatorController controller, string name)
        {
            int removeIx = Array.FindIndex(controller.layers, l => l.name == name);
            Debug.Log("Removing layer [name=" + name + " index=" + removeIx + "] from controller [name=" + controller.name + "]");
            controller.RemoveLayer(removeIx);
        }
    }
}