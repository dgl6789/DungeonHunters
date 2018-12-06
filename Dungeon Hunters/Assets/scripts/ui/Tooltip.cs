using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App.UI {
    public enum TooltipType { Item, Info }

    public class Tooltip : MonoBehaviour {

        [HideInInspector] public bool FollowMouse;
        RectTransform rectTransform;

        public TooltipType Type;

        // Use this for initialization
        void Start() {
            rectTransform = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update() {
            if (FollowMouse) {
                rectTransform.position = GetTooltipPosition(Type);
            }
        }

        /// <summary>
        /// Get the correct position of the active tooltip basesd on the mouse position and tooltip rect.
        /// </summary>
        /// <returns>The position of the tooltip.</returns>
        public static Vector2 GetTooltipPosition(TooltipType pType, Item pItem = null) {
            // Just update the tooltip's position 
            // It should be anchored to the mouse such that no part of it is off screen
            Vector3 mp = Input.mousePosition;
            Vector2 position = new Vector2();
            Rect targetRect = GetTargetTooltipRect(pType, pItem);
            targetRect.size = Vector2.Scale(targetRect.size, AppUI.Instance.UICanvas.transform.lossyScale);

            if (mp.y + targetRect.height > Screen.height) {
                // Choose one of the top corners instead.
                if (mp.x - targetRect.width < 0) {
                    // Should follow the top left corner
                    position = new Vector2(mp.x + (targetRect.width / 2), mp.y - (targetRect.height / 2));
                } else {
                    // Should follow the top right corner
                    position = new Vector2(mp.x - (targetRect.width / 2), mp.y - (targetRect.height / 2));
                }
            } else {
                // Choose one of the bottom corners
                if (mp.x - targetRect.width < 0) {
                    // Should follow the bottom left corner
                    position = new Vector2(mp.x + (targetRect.width / 2), mp.y + (targetRect.height / 2));
                } else {
                    // Should follow the bottom right corner
                    position = new Vector2(mp.x - (targetRect.width / 2), mp.y + (targetRect.height / 2));
                }
            }

            return position;
        }

        public static Rect GetTargetTooltipRect(TooltipType pType, Item pItem = null) {
            Rect targetRect = new Rect();

            switch (pType) {
                default:
                case TooltipType.Info: targetRect = AppUI.Instance.infoTooltipObject.GetComponent<RectTransform>().rect; break;
                case TooltipType.Item: targetRect = AppUI.Instance.ItemTooltipObject.GetComponent<RectTransform>().rect; break;
            } 

            if (pItem == null) return targetRect;

            switch (pItem.Type) {
                case ItemType.Resource:
                    targetRect.yMax -= AppUI.Instance.ItemTooltipObject.GetComponent<ItemTooltipUIObject>().EquipmentBase.rect.height;
                    break;
            }

            return targetRect;
        }
    }
}
