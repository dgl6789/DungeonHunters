using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace App.Data {
    [CustomEditor(typeof(EncounterOption))]
    [CanEditMultipleObjects]
    public class EncounterOptionEditor : Editor {
        
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            GUILayout.ExpandWidth(true);

            if (GUILayout.Button("Basic", EditorStyles.miniButtonLeft)) {
                (serializedObject.targetObject as EncounterOption).SetBasicAction();
            }

            if (GUILayout.Button("Check", EditorStyles.miniButtonMid)) {
                (serializedObject.targetObject as EncounterOption).SetCheckAction();
            }

            if (GUILayout.Button("Dungeoneer", EditorStyles.miniButtonMid))
            {
                (serializedObject.targetObject as EncounterOption).SetDungeoneerAction();
            }


            if (GUILayout.Button("Contest", EditorStyles.miniButtonRight)) {
                (serializedObject.targetObject as EncounterOption).SetContestAction();
            }

            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}