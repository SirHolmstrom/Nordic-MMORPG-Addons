#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DamageTrackerManager))]
public class DamageTrackerManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DamageTrackerManager manager = (DamageTrackerManager)target;

        EditorGUILayout.HelpBox("Click 'Setup RPC Handler' to add the event required for Damage Tracker to work, it will look for all player prefabs in your NetworkManagerMMO.", MessageType.Info);

        if (GUILayout.Button("Setup RPC Handler"))
        {
            manager.SetupDamageRPCToPlayers();
        }
    }
}

#endif