using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace App.Data {
    [Serializable]
    public class EncounterOption : ScriptableObject {
        public int SuccessStepIndex;
        public int FailureStepIndex;
    
        public string Text;
        public EncounterAction Action;

        private int parentIndex;
        public int ParentIndex { set { parentIndex = value; } }

#if UNITY_EDITOR
        public void SetBasicAction() {
            EncounterAction action = CreateInstance<EncounterAction>();
            action.name = "Basic Action [" + parentIndex + "]";
    
            DestroyImmediate(Action, true);
            AssetDatabase.AddObjectToAsset(action, this);
            AssetDatabase.SaveAssets();
            Action = action;
        }
    
        public void SetCheckAction() {
            EncounterCheckAction action = CreateInstance<EncounterCheckAction>();
            action.name = "Check Action [" + parentIndex + "]";

            DestroyImmediate(Action, true);
            AssetDatabase.AddObjectToAsset(action, this);
            AssetDatabase.SaveAssets();
            Action = action;
        }
        public void SetDungeoneerAction()
        {
            EncounterDungeoneerAction action = CreateInstance<EncounterDungeoneerAction>();
            action.name = "Dungeoneer Action [" + parentIndex + "]";

            DestroyImmediate(Action, true);
            AssetDatabase.AddObjectToAsset(action, this);
            AssetDatabase.SaveAssets();
            Action = action;
        }

        public void SetContestAction() {
            EncounterContestAction action = CreateInstance<EncounterContestAction>();
            action.name = "Contest Action [" + parentIndex + "]";

            DestroyImmediate(Action, true);
            AssetDatabase.AddObjectToAsset(action, this);
            AssetDatabase.SaveAssets();
            Action = action;
        }
#endif
    }
}