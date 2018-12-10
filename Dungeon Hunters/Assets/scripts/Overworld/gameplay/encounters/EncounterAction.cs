using System;
using UnityEngine;

namespace App.Data {
    [Serializable]
    public class EncounterAction : ScriptableObject {
        public virtual bool Success(StatBlock sb) { return true; }
    }
}
