using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Data {
    [Serializable]
    [CreateAssetMenu(fileName = "Encounter Data", menuName = "Data/Creature", order = 5)]
    public class CreatureObject : ScriptableObject {

        [SerializeField] public string Name;

        [SerializeField] public StatBlock Stats;
    }
}