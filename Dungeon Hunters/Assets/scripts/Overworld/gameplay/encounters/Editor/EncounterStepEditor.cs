using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace App.Data {
    [CustomEditor(typeof(EncounterStep))]
    [CanEditMultipleObjects]
    public class EncounterStepEditor : Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Index"));
            GUI.enabled = true;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Text"));

            Show(serializedObject.FindProperty("Options"), serializedObject);

            if (GUILayout.Button("Add Option")) {
                (serializedObject.targetObject as EncounterStep).AddOption();
            }

            serializedObject.ApplyModifiedProperties();
        }

        public static void Show(SerializedProperty list, SerializedObject serializedObject) {
            EditorGUILayout.PropertyField(list);

            if (list.arraySize == 0) return;

            EditorGUI.indentLevel += 1;
            for (int i = 0; i < list.arraySize; i++) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                if(GUILayout.Button("Remove")) {
                    (serializedObject.targetObject as EncounterStep).RemoveOption(i);
                }
                GUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel -= 1;
        }
    }
}