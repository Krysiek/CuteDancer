#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace VRF
{
    public class FxControllerBuilder : BuilderInterface
    {
        public void Build(SettingsBuilderData settings)
        {
            string sourcePath = Path.Combine(CuteResources.CUTEDANCER_RUNTIME, "TemplateFX.controller");
            string outputPath = Path.Combine(settings.outputDirectory, "CuteDancer-FX.controller");

            string animFxOffPath = Path.Combine(settings.outputDirectory, "FX", "CuteDancer-FX_OFF.anim");

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

            // replace FX OFF animation
            AnimationClip animFxOff = AssetDatabase.LoadAssetAtPath<AnimationClip>(animFxOffPath);
            beforeState.motion = animFxOff;
            templateState.state.transitions[0].destinationState.motion = animFxOff;

            float nodeX = templateState.position.x;
            float nodeY = templateState.position.y - 60;

            int paramValue = settings.parameterStartValue;
            int paramValueMin = paramValue - 1;
            int paramValueMax = paramValue + settings.dances.Count;

            AnimatorControllerUtil.UpdateMinMaxTransitions(rootStateMachine, settings.parameterName, paramValueMin, paramValueMax);

            foreach (DanceBuilderData dance in settings.dances)
            {
                AnimatorState senderState = rootStateMachine.AddState(dance._name, new Vector3(nodeX, nodeY));
                EditorUtility.CopySerialized(templateState.state, senderState);
                senderState.name = senderState.name.Replace("{DANCE}", dance._name);

                // unfortunately CopySerialized copies a reference of transitions, need to remake them
                senderState.transitions = Array.Empty<AnimatorStateTransition>();
                foreach (AnimatorStateTransition templateOutTransition in templateState.state.transitions)
                {
                    AnimatorStateTransition outTransition = senderState.AddTransition(templateOutTransition.destinationState);
                    outTransition.hasExitTime = templateOutTransition.hasExitTime;
                    outTransition.exitTime = templateOutTransition.exitTime;
                    outTransition.hasFixedDuration = templateOutTransition.hasFixedDuration;
                    outTransition.duration = templateOutTransition.duration;
                    outTransition.offset = templateOutTransition.offset;

                    foreach (AnimatorCondition condition in templateOutTransition.conditions)
                    {
                        outTransition.AddCondition(
                            condition.mode,
                            paramValue,
                            settings.parameterName
                        );
                    }
                }

                string fxAnimPath = Path.Combine(settings.outputDirectory, "FX", $"{dance._name}_FX_ON.anim");
                senderState.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>(fxAnimPath);

                AnimatorStateTransition inTransition = beforeState.AddTransition(senderState);

                inTransition.hasExitTime = templateInTransition.hasExitTime;
                inTransition.exitTime = templateInTransition.exitTime;
                inTransition.hasFixedDuration = templateInTransition.hasFixedDuration;
                inTransition.duration = templateInTransition.duration;
                inTransition.offset = templateInTransition.offset;

                foreach (AnimatorCondition condition in templateInTransition.conditions)
                {
                    if (condition.parameter == "{PARAM}")
                    {
                        inTransition.AddCondition(
                            condition.mode,
                            paramValue,
                            settings.parameterName
                        );
                    }
                    else
                    {
                        inTransition.AddCondition(
                            condition.mode,
                            condition.threshold,
                            condition.parameter
                        );
                    }
                }

                nodeY += 50;
                paramValue++;
            }

            rootStateMachine.RemoveState(templateState.state);
            animator.layers = animatorLayers;

            Debug.Log($"Save file [name = {outputPath}]");
            EditorUtility.SetDirty(animator);
            AssetDatabase.SaveAssets();
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