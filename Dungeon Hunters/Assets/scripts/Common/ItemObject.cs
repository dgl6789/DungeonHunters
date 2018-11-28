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

        [SerializeField] RectTransform EquippedRect;

        public void Initialize() {
            // If this isn't stackable, disable the stack count module
            if(Data.Stackable && Data.StackSize > 1) {
                CountRect.gameObject.SetActive(true);
                CountText.text = Data.StackSize.ToString();
            } else {
                CountRect.gameObject.SetActive(false);
            }
            
            EquippedRect.gameObject.SetActive(Data.IsEquipped);
        }

        public void UpdateStackSize() {
            // Change the stack count module text, or hide it
            if (Data.StackSize > 1) {
                CountText.text = Data.StackSize.ToString();
            } else {
                CountRect.gameObject.SetActive(false);
            }
        }

        public void UpdateIsEquipped() {
            EquippedRect.gameObject.SetActive(Data.IsEquipped);
        }
    }
}
