using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App {
    public class StrongholdController : MonoBehaviour {

        public static StrongholdController Instance;

        [HideInInspector] public Inventory StrongholdInventory;

        // Use this for initialization
        void Awake() {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        private void Start() {
            // TODO: Load stronghold data from save

            StrongholdInventory = new Inventory(5, 5);
        }

        // Update is called once per frame
        void Update() {

        }
    }
}