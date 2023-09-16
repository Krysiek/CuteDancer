#if VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

namespace VRF
{
    public class ActionControllerBuilder
    {
        public void BuildActionController(SettingsBuilderData settings)
        {
            string sourcePath = Path.Combine(CuteResources.CUTEDANCER_RUNTIME, "TemplateAction.controller");
            string outputPath = Path.Combine(settings.outputDirectory, "CuteDancer-Action.controller");

            if (!AssetDatabase.CopyAsset(sourcePath, outputPath))
            {
                throw new Exception("Error copying template: Action Controller");
            }

            AnimatorController animator = AssetDatabase.LoadAssetAtPath<AnimatorController>(outputPath);

            animator.parameters = ParseTemplateParameters(settings, animator.parameters);

            AnimatorControllerLayer[] animatorLayers = animator.layers;
            AnimatorControllerLayer layer = animatorLayers[0];

            AnimatorStateMachine rootStateMachine = layer.stateMachine;

            ChildAnimatorState templateState = Array.Find(rootStateMachine.states, state => state.state.name == "{DANCE}");
            AnimatorState beforeDanceState = Array.Find(rootStateMachine.states, state => state.state.name == "Prepare animation").state;
            AnimatorState afterDanceState = Array.Find(rootStateMachine.states, state => state.state.name == "Stop").state;
            AnimatorStateTransition[] templateInTransitions = beforeDanceState.transitions;
            AnimatorStateTransition[] templateOutTransitions = templateState.state.transitions;

            float nodeX = templateState.position.x;
            float nodeY = templateState.position.y - 35 * settings.dances.Count;

            int paramValue = settings.parameterStartValue;
            int paramValueMin = paramValue - 1;
            int paramValueMax = paramValue + settings.dances.Count;
            
            AnimatorControllerUtil.UpdateMinMaxTransitions(rootStateMachine, settings.parameterName, paramValueMin, paramValueMax);

            foreach (DanceBuilderData dance in settings.dances)
            {
                AnimatorStateMachine danceStateMachine = dance.animator.layers[0].stateMachine;

                List<AnimatorState> sourceNodes = new List<AnimatorState>();
                List<AnimatorState> copiedNodes = new List<AnimatorState>();

                AnimatorState danceEntryState = dance.animator.layers[0].stateMachine.defaultState;
                ExtractFollowingBlocks(sourceNodes, danceEntryState);

                Debug.Log($"ActionAnimatorBuilder: Copying nodes [{dance._name}]");
                for (int i = 0; i < sourceNodes.Count; i++)
                {
                    AnimatorState danceState = rootStateMachine.AddState(dance._name, new Vector3(nodeX, nodeY));
                    EditorUtility.CopySerialized(sourceNodes[i], danceState);

                    // Clear copied transitions
                    // They have to be rebuilt in next loop when all nodes will be moved
                    danceState.transitions = Array.Empty<AnimatorStateTransition>();

                    copiedNodes.Add(danceState);
                    nodeY += 55;
                }

                Debug.Log($"ActionAnimatorBuilder: Building entry transition for the first state [{copiedNodes[0].name}]");
                BuildTransitions(beforeDanceState, copiedNodes[0], templateInTransitions, dance._name, settings.parameterName, paramValue);

                // Build transitions between blocks and exit transitions
                for (int i = 0; i < sourceNodes.Count; i++)
                {
                    AnimatorState sourceState = sourceNodes[i];
                    AnimatorState copiedState = copiedNodes[i];

                    if (sourceState.transitions.Length == 0)
                    {
                        // No transitions = simple animation, transition to exit state
                        Debug.Log($"ActionAnimatorBuilder: No transitions for state [{copiedState.name}] found, build transition to exit node");
                        BuildTransitions(copiedState, afterDanceState, templateOutTransitions, dance._name, settings.parameterName, paramValue);
                    }

                    foreach (var srcTransition in sourceState.transitions)
                    {
                        if (srcTransition.destinationState == null)
                        {
                            // Transition to exit state
                            Debug.Log($"ActionAnimatorBuilder: Transition to exit state [{copiedState.name}] found, build transition to exit node");
                            BuildTransitions(copiedState, afterDanceState, templateOutTransitions, dance._name, settings.parameterName, paramValue);
                        }
                        else if (srcTransition.conditions.Length > 0 && srcTransition.conditions[0].parameter == "{EXIT}")
                        {
                            // Transition to another block with exit conditions
                            AnimatorState destState = copiedNodes[sourceNodes.FindIndex(state => state == srcTransition.destinationState)];
                            Debug.Log($"ActionAnimatorBuilder: Custom EXIT transition from state [{copiedState.name}] found, build transition to state [{destState.name}]");
                            BuildTransition(copiedState, destState, srcTransition, templateOutTransitions[0].conditions, dance._name, settings.parameterName, paramValue);
                        }
                        else
                        {
                            // Build both
                            AnimatorState destState = copiedNodes[sourceNodes.FindIndex(state => state == srcTransition.destinationState)];
                            Debug.Log($"ActionAnimatorBuilder: Transition from state [{copiedState.name}] found, build transition to state [{destState.name}] AND to exit node");
                            BuildTransition(copiedState, destState, srcTransition, dance._name, settings.parameterName, paramValue);
                            BuildTransitions(copiedState, afterDanceState, templateOutTransitions, dance._name, settings.parameterName, paramValue);
                        }
                    }
                }

                nodeY += 25;
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

        private void BuildTransitions(AnimatorState fromState, AnimatorState toState, AnimatorStateTransition[] templateTransitions, string danceName, string paramName, int paramValue)
        {
            foreach (var templateOutTransition in templateTransitions)
            {
                BuildTransition(fromState, toState, templateOutTransition, danceName, paramName, paramValue);
            }
        }

        private void BuildTransition(AnimatorState fromState, AnimatorState toState, AnimatorStateTransition templateTransition, string danceName, string paramName, int paramValue)
        {
            BuildTransition(fromState, toState, templateTransition, templateTransition.conditions, danceName, paramName, paramValue);
        }

        private void BuildTransition(AnimatorState fromState, AnimatorState toState, AnimatorStateTransition templateTransition, AnimatorCondition[] templateConditions, string danceName, string paramName, int paramValue)
        {
            AnimatorStateTransition transition = fromState.AddTransition(toState);
            transition.hasExitTime = templateTransition.hasExitTime;
            transition.exitTime = templateTransition.exitTime;
            transition.hasFixedDuration = templateTransition.hasFixedDuration;
            transition.duration = templateTransition.duration;
            transition.offset = templateTransition.offset;

            foreach (AnimatorCondition templateCondition in templateConditions)
            {
                if (templateCondition.parameter == "{PARAM}")
                {
                    transition.AddCondition(
                        templateCondition.mode,
                        paramValue,
                        templateCondition.parameter.Replace("{PARAM}", paramName)
                    );
                }
                else
                {
                    transition.AddCondition(
                        templateCondition.mode,
                        templateCondition.threshold,
                        templateCondition.parameter.Replace("{DANCE}", danceName)
                    );
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

            AnimatorControllerParameter templateDanceParam = animatorParameters.Find(param => param.name == "{DANCE}");
            foreach (DanceBuilderData dance in settings.dances)
            {
                animatorParameters.Add(new AnimatorControllerParameter()
                {
                    name = dance._name,
                    type = templateDanceParam.type,
                    defaultBool = templateDanceParam.defaultBool
                });
            }
            animatorParameters.Remove(templateDanceParam);
            return animatorParameters.ToArray();
        }
    }
}
#endif