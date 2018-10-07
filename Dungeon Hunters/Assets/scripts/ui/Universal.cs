using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App.UI {
    public class Universal : MonoBehaviour {
        
        public Camera MainCamera;
        public Canvas Canvas;
        
        public static Universal Instance = null;

        private void Awake() {
            MainCamera = FindObjectOfType<Camera>();
            Canvas = FindObjectOfType<Canvas>();
        }
    }
}
