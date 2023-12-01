#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Linq;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(EmoteManager))]
public class EmoteManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.HelpBox("Click 'Update Animators' to synchronize the animation clips from EmoteData to the specified Animator Controllers. This will update existing states or add new states based on the animations defined in EmoteManager.", MessageType.Info);
        EditorGUILayout.HelpBox("If animation clips are called 'mixamo.com' or have duplicated names, 'Update Animators' will create new animation clips in order to make sure the addon works as intended.", MessageType.Info);

        EmoteManager emoteManager = (EmoteManager)target;

        if (GUILayout.Button("Update Animators"))
        {
            UpdateAnimators(emoteManager);
        }
    }

    #region NONE GUI LOGIC

    private void UpdateAnimators(EmoteManager emoteManager)
    {
        // First, we want to ensure all animation clips have unique names,
        // this will create duplicates and rename them otherwise..
        EnsureUniqueClipNames(emoteManager);

        EditorApplication.delayCall += () =>
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

                // collect states to remove.
                var validClipNames = emoteManager.Emotes.SelectMany(e => e.animationClips).Select(c => c.name).ToList();
                var statesToRemove = stateMachine.states
                    .Where(s => !validClipNames.Contains(s.state.name) && s.state.name != emoteManager.Empty_Animator_State)
                    .ToList();

                // Remove states in a separate step
                foreach (var state in statesToRemove)
                {
                    // used for workaround.
                    state.state.name = "_Remove";
                }

                // update the state machine after state removal.
                stateMachine.states = stateMachine.states.Where(s => s.state != null).ToArray();


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

                // **** REMOVE STATES WORKAROUND TO NOT BREAK FOCUSED ANIMATOR WINDOW. **** //

                EditorApplication.delayCall += () => RemoveMarkedStates(stateMachine);

                EditorUtility.SetDirty(animatorController);

            }

            // ensure the state machine is updated and refreshed.
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        };


    }

    private void RemoveMarkedStates(AnimatorStateMachine stateMachine)
    {
        var statesToRemove = stateMachine.states
            .Where(s => s.state.name.Equals("_Remove"))
            .ToList();

        foreach (var state in statesToRemove)
        {
            stateMachine.RemoveState(state.state);
        }
    }


    private int FindOrCreateLayer(AnimatorController animatorController, string layerName)
    {
        var layers = animatorController.layers;
        int layerIndex = -1;

        // look for the layer and get its index.
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].name == layerName)
            {
                layerIndex = i;
                break;
            }
        }

        // if the layer doesn't exist, create it.
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

            // add the state machine to the controller's assets only if it's new.
            if (AssetDatabase.GetAssetPath(stateMachine) == string.Empty)
            {
                AssetDatabase.AddObjectToAsset(stateMachine, animatorController);
                AssetDatabase.SaveAssets();
            }

            return layersList.Count - 1;
        }
        else
        {
            // if the layer exists but doesn't have a state machine, create and assign it
            var layer = layers[layerIndex];
            if (layer.stateMachine == null)
            {
                AnimatorStateMachine stateMachine = new AnimatorStateMachine();
                layer.stateMachine = stateMachine;
                animatorController.layers[layerIndex] = layer;

                // add the new state machine to the controller's assets
                if (AssetDatabase.GetAssetPath(stateMachine) == string.Empty)
                {
                    AssetDatabase.AddObjectToAsset(stateMachine, animatorController);
                    AssetDatabase.SaveAssets();
                }
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

    private void EnsureUniqueClipNames(EmoteManager emoteManager)
    {
        // dictionary to keep track of the number of duplicates for each clip name.
        Dictionary<string, int> clipNameCounts = new Dictionary<string, int>();

        foreach (var emoteData in emoteManager.Emotes)
        {
            for (int i = 0; i < emoteData.animationClips.Length; i++)
            {
                AnimationClip clip = emoteData.animationClips[i];
                string originalName = clip.name;

                if (originalName.Equals("mixamo.com", StringComparison.OrdinalIgnoreCase) || clipNameCounts.ContainsKey(originalName))
                {
                    // increment the count for this clip name.
                    int nextCount = clipNameCounts.ContainsKey(originalName) ? clipNameCounts[originalName] + 1 : 1;
                    clipNameCounts[originalName] = nextCount;

                    // create a new unique name for the clip.
                    string uniqueName = $"{emoteData.identifier}_{nextCount}";

                    // duplicate the clip and rename it.
                    AnimationClip newClip = new AnimationClip();
                    EditorUtility.CopySerialized(clip, newClip);
                    newClip.name = uniqueName;

                    // replace the clip in the emoteData with the new unique clip.
                    emoteData.animationClips[i] = newClip;

                    // save the new clip as an asset
                    string assetPath = AssetDatabase.GetAssetPath(clip);
                    string pathToNewAsset = System.IO.Path.GetDirectoryName(assetPath) + "/" + uniqueName + ".anim";
                    AssetDatabase.CreateAsset(newClip, pathToNewAsset);
                }

                else
                {
                    // if it's not a duplicate, add it to the dictionary with a count of 1.
                    clipNameCounts[originalName] = 1;
                }
            }
        }

        // save the changes to the assets
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    #endregion

}

#endif