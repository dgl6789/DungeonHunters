using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Numeralien.Utilities;

namespace App.Data {
    [Serializable]
    public class EncounterStep : ScriptableObject {
        public int Index;

        public string Text;
        
        public List<EncounterOption> Options = new List<EncounterOption>();

#if UNITY_EDITOR
        // Add an empty option to the encounter step in the inspector.
        public void AddOption() {
            EncounterOption option = CreateInstance<EncounterOption>();
            option.name = "Option " + Index + "-" + Options.Count;
            option.ParentIndex = Index;

            AssetDatabase.AddObjectToAsset(option, this);
            AssetDatabase.SaveAssets();

            Options.Add(option);
        }

        public void UpdateOptionNames() {
            for(int i = 0; i < Options.Count; i++) {
                Options[i].name = "Option " + Index + "-" + i;
            }

            AssetDatabase.SaveAssets();
        }

        public void RemoveOption(int index) {
            DestroyImmediate(Options[index].Action, true);
            DestroyImmediate(Options[index], true);
            AssetDatabase.SaveAssets();

            Options.RemoveAt(index);

            UpdateOptionNames();
        }

        public void RemoveAllOptions() {
            int count = Options.Count;
            for(int i = 0; i < count; i++) {
                DestroyImmediate(Options[i].Action, true);
                DestroyImmediate(Options[i], true);
            }

            Options.Clear();
        }
#endif
    }
}
