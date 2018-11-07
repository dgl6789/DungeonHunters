using Overworld;
using UnityEngine;

namespace App.Data {
    [CreateAssetMenu(fileName = "Encounter Manifest", menuName = "Data/Encounters", order = 2)]
    public class EncounterManifest : ScriptableObject {
        public EncounterEvent[] Encounters;
    }
}