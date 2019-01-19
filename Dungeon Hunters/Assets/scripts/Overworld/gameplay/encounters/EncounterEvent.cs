using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace App.Data
{
    public enum EncounterTags { FORAGING, FIGHTING, MYSTICISM }

    [Serializable]
    [CreateAssetMenu(fileName = "Encounter Data", menuName = "Data/Encounter", order = 3)]
    public class EncounterEvent : ScriptableObject {

        public EncounterTags[] Tags;
        
        public List<EncounterStep> Steps = new List<EncounterStep>();

#if UNITY_EDITOR
        // Add an empty encounter step object to the inspector.
        public void AddStep() {
            EncounterStep step = CreateInstance<EncounterStep>();
            step.name = "Step " + Steps.Count;
            step.Index = Steps.Count;

            AssetDatabase.AddObjectToAsset(step, this);
            AssetDatabase.SaveAssets();

            Steps.Add(step);
        }

        public void RemoveStep(int index) {
            if (index < 0 || index > Steps.Count - 1) return;
            
            Steps[index].RemoveAllOptions();
            
            DestroyImmediate(Steps[index], true);
            AssetDatabase.SaveAssets();

            Steps.RemoveAt(index);

            UpdateStepNames();
        }

        public void UpdateStepNames() {
            for (int i = 0; i < Steps.Count; i++) {
                Steps[i].name = "Step " + i;
                Steps[i].Index = i;
            }

            AssetDatabase.SaveAssets();
        }
#endif
    }
}
