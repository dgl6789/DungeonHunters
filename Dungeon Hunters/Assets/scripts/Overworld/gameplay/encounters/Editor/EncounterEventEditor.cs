using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using App.Data;

[CustomEditor(typeof(EncounterEvent))]
[CanEditMultipleObjects]
public class EncounterEventEditor : Editor {

    public override void OnInspectorGUI() {
        serializedObject.Update();

        Show(serializedObject.FindProperty("Tags"), serializedObject);
        ShowWithRemoveButton(serializedObject.FindProperty("Steps"), serializedObject);
        
        if (GUILayout.Button("Add Encounter Step")) {
            (serializedObject.targetObject as EncounterEvent).AddStep();
        }

        serializedObject.ApplyModifiedProperties();
    }

    public static void ShowWithRemoveButton(SerializedProperty list, SerializedObject serializedObject) {
        EditorGUILayout.PropertyField(list);

        EditorGUI.indentLevel += 1;
        for (int i = 0; i < list.arraySize; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            if (GUILayout.Button("Remove")) {
                (serializedObject.targetObject as EncounterEvent).RemoveStep(i);
            }
            GUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel -= 1;
    }

    public static void Show(SerializedProperty list, SerializedObject serializedObject) {
        EditorGUILayout.PropertyField(list);
        
        EditorGUI.indentLevel += 1;
        if (list.isExpanded) {
            EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
            for (int i = 0; i < list.arraySize; i++) {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            }
        }
        EditorGUI.indentLevel -= 1;
    }
}
