using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App.UI;
using TMPro;

namespace App {
    public class ItemObject : MonoBehaviour {

        // instance of a scriptable object
        public Item Data;

        [SerializeField] RectTransform CountRect;
        [SerializeField] TextMeshProUGUI CountText;
        
        public void Initialize() {
            // If this isn't stackable, disable the stack count module
            if(Data.Stackable) {
                CountText.text = Data.StackSize.ToString();
            } else {
                CountRect.gameObject.SetActive(false);
            }
        }
    }
}
