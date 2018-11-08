using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Overworld
{
    public class EncounterEvent : MonoBehaviour
    {
        public int ExtraDayTimer;
        public bool IsRequired;

        // Also holds data that can spawn an encounter dialog.
        Dictionary<int, KeyValuePair<string, List<Option>>> EncounterData;

        public EncounterEvent()
        {
            ExtraDayTimer = 0;
            IsRequired = true;

            EncounterData = new Dictionary<int, KeyValuePair<string, List<Option>>>();

            // Get encounter data from the encounter manifest.
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


            
            // change the text

            // Spawn new options
        }

        private void SpawnOptions(int pDialogIndex)
        {
            // Clear existing option buttons

            // Iterate through the options at this index and create new option buttons.

            /// NOTE
            /// When an option is clicked, it should attach a callback to the function that
            /// advances the encounter. When that's called, it should call the corresponding
            /// callback method first.
        }

        public void EndEncounter()
        {
            // Clear existing option buttons

            // Clean up the encounter dialog
        }
    }
}
