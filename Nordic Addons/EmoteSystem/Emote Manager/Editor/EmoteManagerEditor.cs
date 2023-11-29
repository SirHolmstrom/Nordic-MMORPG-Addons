using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Linq;
using System;

[CustomEditor(typeof(EmoteManager))]
public class EmoteManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.HelpBox("Click 'Update Animators' to synchronize the animation clips from EmoteData to the specified Animator Controllers. This will update existing states or add new states based on the animations defined in EmoteManager.", MessageType.Info);

        EmoteManager emoteManager = (EmoteManager)target;

        if (GUILayout.Button("Update Animators"))
        {
            UpdateAnimators(emoteManager);
        }
    }

    private void UpdateAnimators(EmoteManager emoteManager)
    {
        // grab the animator controllers from the manager.
        AnimatorController[] animatorControllers = emoteManager.animatorControllers.ToArray();

        foreach (var animatorController in animatorControllers)
        {
            int layerIndex = FindOrCreateLayer(animatorController, emoteManager.Emote_Animator_Layer);

            // make sure we have the 'empty' state.
            EnsureDefaultState(animatorController, layerIndex);

            AnimatorStateMachine stateMachine = animatorController.layers[layerIndex].stateMachine;

            // first, handle renaming and updating existing states.
            foreach (var state in stateMachine.states)
            {
                AnimationClip clip = state.state.motion as AnimationClip;
                if (clip != null)
                {
                    var emoteData = emoteManager.Emotes.FirstOrDefault(e => e.animationClips.Contains(clip));
                    if (emoteData != null)
                    {
                        state.state.name = clip.name;  // Rename state to match clip name
                    }
                }
            }

            // then, remove states that are no longer in EmoteManager.
            var validClipNames = emoteManager.Emotes.SelectMany(e => e.animationClips).Select(c => c.name).ToList();
            var statesToRemove = stateMachine.states.Where(s => !validClipNames.Contains(s.state.name) && s.state.name != emoteManager.Empty_Animator_State).ToList();
            foreach (var state in statesToRemove)
            {
                stateMachine.RemoveState(state.state);
            }

            // finally, add new states for each clip in EmoteManager.
            foreach (var emoteData in emoteManager.Emotes)
            {
                foreach (var clip in emoteData.animationClips)
                {
                    if (!stateMachine.states.Any(s => s.state.name == clip.name))
                    {
                        var newState = stateMachine.AddState(clip.name);
                        newState.motion = clip;
                    }
                }
            }

            EditorUtility.SetDirty(animatorController);
        }

        AssetDatabase.Refresh();

    }

    private int FindOrCreateLayer(AnimatorController animatorController, string layerName)
    {
        var layers = animatorController.layers;
        int layerIndex = -1;

        // look for the layer and get its index
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].name == layerName)
            {
                layerIndex = i;
                break;
            }
        }

        // if the layer doesn't exist, create it
        if (layerIndex == -1)
        {
            AnimatorStateMachine stateMachine = new AnimatorStateMachine();

            var newLayer = new AnimatorControllerLayer
            {
                name = layerName,
                defaultWeight = 1f,
                stateMachine = stateMachine
            };

            var layersList = layers.ToList();
            layersList.Add(newLayer);
            animatorController.layers = layersList.ToArray();

            AssetDatabase.AddObjectToAsset(stateMachine, animatorController);
            AssetDatabase.SaveAssets();

            return layersList.Count - 1;
        }
        else
        {
            // if the layer exists but doesn't have a state machine, create and assign it.
            var layer = layers[layerIndex];
            if (layer.stateMachine == null)
            {
                AnimatorStateMachine stateMachine = new AnimatorStateMachine();
                layer.stateMachine = stateMachine;
                animatorController.layers[layerIndex] = layer;

                AssetDatabase.AddObjectToAsset(stateMachine, animatorController);
                AssetDatabase.SaveAssets();
            }

            return layerIndex;
        }
    }


    private void EnsureDefaultState(AnimatorController animatorController, int layerIndex)
    {
        // check if the layerIndex is within the valid range.
        if (layerIndex < 0 || layerIndex >= animatorController.layers.Length)
        {
            Debug.LogError("Invalid layer index passed to EnsureDefaultState.");
            return;
        }

        AnimatorStateMachine stateMachine = animatorController.layers[layerIndex].stateMachine;

        // retrieve the name of the empty state.
        string emptyStateName = ((EmoteManager)target).Empty_Animator_State;

        // check if the default state already exists.
        var defaultState = stateMachine.states.FirstOrDefault(s => s.state.name == emptyStateName).state;
        if (defaultState == null)
        {
            // create the default state if it doesn't exist.
            defaultState = stateMachine.AddState(emptyStateName);
        }

        // set the created state as the default state.
        stateMachine.defaultState = defaultState;
    }


}
