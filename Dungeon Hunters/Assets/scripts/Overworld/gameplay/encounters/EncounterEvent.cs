using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace App.Data
{
    [CreateAssetMenu(fileName = "Encounter Data", menuName = "Data/Encounter", order = 3)]
    public class EncounterEvent : ScriptableObject {
        public int ExtraDayTimer;
        public bool IsRequired;
        public string Name;

        public string[] Functionality;

        // Also holds data that can spawn an encounter dialog.
        Dictionary<int, KeyValuePair<string, List<Option>>> EncounterData;

        void Awake()
        {
            ExtraDayTimer = 0;
            IsRequired = true;

            EncounterData = new Dictionary<int, KeyValuePair<string, List<Option>>>();

            // Get encounter data from the encounter manifest.

            Name = "Encounter Name";
        }

        public void BeginEncounter()
        {
            // Initialize the encounter dialog with the initial text

            // Spawn the initial options
        }
        
        public void AdvanceEncounter(Option pOptionChosen)
        {
            // Call the function attached to the option
            Type manifest = Type.GetType("OptionManifest");
            MethodInfo method = manifest.GetMethod(pOptionChosen.OptionCallbackName);

            // Call the function
            
            // change the text

            // Spawn new options
        }

        private void SpawnOptions(int pDialogIndex)
        {
            // Clear existing option buttons

            // Iterate through the options at this index and create new option buttons.
            
        }

        public void EndEncounter()
        {
            // Clear existing option buttons

            // Clean up the encounter dialog
        }
    }
}
