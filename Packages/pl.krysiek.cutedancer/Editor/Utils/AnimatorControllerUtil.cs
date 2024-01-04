using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using System.Linq;

namespace VRF
{
    public class AnimatorControllerUtil
    {

        public static void UpdateMinMaxTransitions(AnimatorStateMachine rootStateMachine, string parameterName, int min, int max)
        {
            List<AnimatorStateTransition> transitionsToUpdate = GetTransitionsWithParamVariable(rootStateMachine);

            foreach (AnimatorStateTransition transition in transitionsToUpdate)
            {
                AnimatorCondition[] conditions = Array.FindAll(transition.conditions, c => c.parameter == "{PARAM}" && (c.mode == AnimatorConditionMode.Greater || c.mode == AnimatorConditionMode.Less));

                foreach (AnimatorCondition condition in conditions)
                {
                    transition.RemoveCondition(condition);
                    transition.AddCondition(
                        condition.mode,
                        condition.threshold == 0 ? min : max,
                        parameterName
                    );
                }
            }

            EditorUtility.SetDirty(rootStateMachine);
        }

        private static List<AnimatorStateTransition> GetTransitionsWithParamVariable(AnimatorStateMachine rootStateMachine)
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

        public static void RemoveOrphans(AnimatorController animator)
        {
            string assetPath = AssetDatabase.GetAssetPath(animator);

            while (AssetCleanup.RemoveOrphans(assetPath)) { }
        }
    }
}