using UnityEngine;

namespace App.Data {
    [CreateAssetMenu(fileName = "Mercenary Data", menuName = "Data/Mercenaries", order = 1)]
    public class MercenaryManifest : ScriptableObject {
        public string[] Names;
    }
}