#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace VRF
{
    public class FxControllerBuilder
    {
        public void BuildFxController(SettingsBuilderData settings)
        {
            string sourcePath = Path.Combine(CuteResources.CUTEDANCER_RUNTIME, "TemplateFX.controller");
            string outputPath = Path.Combine(settings.outputDirectory, "CuteDancer-FX.controller");

            if (!AssetDatabase.CopyAsset(sourcePath, outputPath))
            {
                throw new Exception("Error copying template: FX Controller");
            }

            AnimatorController animator = AssetDatabase.LoadAssetAtPath<AnimatorController>(outputPath);

            animator.parameters = ParseTemplateParameters(settings, animator.parameters);

            AnimatorControllerLayer[] animatorLayers = animator.layers;
            AnimatorControllerLayer layer = animatorLayers[0];

            AnimatorStateMachine rootStateMachine = layer.stateMachine;

            ChildAnimatorState templateState = Array.Find(rootStateMachine.states, state => state.state.name.Contains("{DANCE}"));
            AnimatorState beforeState = Array.Find(rootStateMachine.states, state => state.state.name == "Sending dance").state;
            AnimatorStateTransition templateInTransition = Array.Find(beforeState.transitions, t => t.destinationState == templateState.state);

            float nodeX = templateState.position.x;
            float nodeY = templateState.position.y - 60;

            int paramValue = settings.parameterStartValue;
            int paramValueMin = paramValue;
            int paramValueMax = paramValue + settings.dances.Count;

            List<AnimatorStateTransition> transitionsToUpdate = GetTransitionsWithParamVariable(rootStateMachine);

            foreach (AnimatorStateTransition transition in transitionsToUpdate)
            {
                AnimatorCondition[] conditions = Array.FindAll(transition.conditions, c => c.parameter == "{PARAM}" && (c.mode == AnimatorConditionMode.Greater || c.mode == AnimatorConditionMode.Less));

                foreach (AnimatorCondition condition in conditions)
                {
                    transition.RemoveCondition(condition);
                    transition.AddCondition(
                        condition.mode,
                        condition.mode == AnimatorConditionMode.Greater ? paramValueMin : paramValueMax,
                        settings.parameterName
                    );
                }
            }

            foreach (DanceBuilderData dance in settings.dances)
            {
                AnimatorState senderState = rootStateMachine.AddState(dance._name, new Vector3(nodeX, nodeY));
                EditorUtility.CopySerialized(templateState.state, senderState);
                senderState.name = senderState.name.Replace("{DANCE}", dance._name);

                // unfortunately CopySerialized copies a reference of transitions, need to remake them
                senderState.transitions = Array.Empty<AnimatorStateTransition>();
                foreach (AnimatorStateTransition templateTransition in templateState.state.transitions)
                {
                    AnimatorStateTransition copiedOutTransition = new AnimatorStateTransition();
                    EditorUtility.CopySerialized(templateTransition, copiedOutTransition);
                    copiedOutTransition.conditions = Array.Empty<AnimatorCondition>();

                    foreach (AnimatorCondition condition in templateTransition.conditions)
                    {
                        copiedOutTransition.AddCondition(
                            condition.mode,
                            paramValue,
                            settings.parameterName
                        );
                    }

                    senderState.AddTransition(copiedOutTransition);
                }

                string fxAnimPath = Path.Combine(settings.outputDirectory, "FX", $"{dance._name}_FX_ON.anim");
                senderState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(fxAnimPath);


                AnimatorStateTransition copiedInTransition = new AnimatorStateTransition();
                EditorUtility.CopySerialized(templateInTransition, copiedInTransition);
                copiedInTransition.conditions = Array.Empty<AnimatorCondition>();
                copiedInTransition.destinationState = senderState;

                foreach (AnimatorCondition condition in templateInTransition.conditions)
                {
                    if (condition.parameter == "{PARAM}")
                    {
                        copiedInTransition.AddCondition(
                            condition.mode,
                            paramValue,
                            settings.parameterName
                        );
                    }
                    else
                    {
                        copiedInTransition.AddCondition(
                            condition.mode,
                            condition.threshold,
                            condition.parameter
                        );
                    }
                }

                beforeState.AddTransition(copiedInTransition);

                nodeY += 50;
                paramValue++;
            }

            rootStateMachine.RemoveState(templateState.state);
            animator.layers = animatorLayers;

            Debug.Log($"Save file [name = {outputPath}]");
            EditorUtility.SetDirty(animator);
            AssetDatabase.SaveAssets();
        }

        private List<AnimatorStateTransition> GetTransitionsWithParamVariable(AnimatorStateMachine rootStateMachine)
        {
            List<AnimatorStateTransition> transitionsToUpdate = new List<AnimatorStateTransition>();

            foreach (var state in rootStateMachine.states)
            {
                foreach (var transition in state.state.transitions)
                {
                    foreach (var condition in transition.conditions)
                    {
                        if (condition.parameter == "{PARAM}" && (condition.mode == AnimatorConditionMode.Greater || condition.mode == AnimatorConditionMode.Less))
                        {
                            transitionsToUpdate.Add(transition);
                        }
                    }
                }
            }
            return transitionsToUpdate;
        }

        private void ExtractFollowingBlocks(List<AnimatorState> blocks, AnimatorState current)
        {
            blocks.Add(current);
            if (current.transitions.Length > 0 && !blocks.Contains(current.transitions[0].destinationState))
            {
                if (current.transitions[0].destinationState)
                {
                    ExtractFollowingBlocks(blocks, current.transitions[0].destinationState);
                }
            }
        }

        private AnimatorControllerParameter[] ParseTemplateParameters(SettingsBuilderData settings, AnimatorControllerParameter[] controllerParameters)
        {
            List<AnimatorControllerParameter> animatorParameters = new List<AnimatorControllerParameter>(controllerParameters);

            foreach (AnimatorControllerParameter param in animatorParameters)
            {
                param.name = param.name.Replace("{PARAM}", settings.parameterName);
            }

            return animatorParameters.ToArray();
        }
    }
}
#endif