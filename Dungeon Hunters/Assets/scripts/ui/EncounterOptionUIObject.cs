using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using App.Data;

namespace App {
    public class EncounterOptionUIObject : MonoBehaviour {
        
        [SerializeField] TextMeshProUGUI Text;

        public void Initialize(EncounterOption Data) {
            Text.text = Data.Text;
        }
    }
}