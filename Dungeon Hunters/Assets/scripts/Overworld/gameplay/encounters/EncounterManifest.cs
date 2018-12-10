using Overworld;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace App.Data {
    [CreateAssetMenu(fileName = "Encounter Manifest", menuName = "Data/Encounters", order = 2)]
    public class EncounterManifest : ScriptableObject {
        public EncounterEvent[] Encounters;
    }
}