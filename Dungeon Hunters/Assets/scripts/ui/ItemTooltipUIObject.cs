using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using App;

namespace App.UI {
    public class ItemTooltipUIObject : MonoBehaviour {

        public RectTransform EquipmentBase;

        [SerializeField] TextMeshProUGUI ItemTitle;
        [SerializeField] TextMeshProUGUI ItemPowerLevel;
        [SerializeField] TextMeshProUGUI TypeText;
        [SerializeField] TextMeshProUGUI FlavorText;

        [SerializeField] Image[] EnchantmentImages;
        [SerializeField] Image TypeImage;

        public void Initialize(Item pItem) {
            ItemTitle.text = pItem.Name;
            ItemPowerLevel.text = "Power " + pItem.Power.ToString();
            TypeText.text = pItem.Type.ToString();
            TypeImage.sprite = AppUI.Instance.itemTypeSprites[(int)pItem.Type];
            FlavorText.text = pItem.FlavorText;

            switch (pItem.Type) {
                case ItemType.Resource:
                    EquipmentBase.gameObject.SetActive(false);
                    Rect orig = GetComponent<RectTransform>().rect;
                    orig.yMax -= EquipmentBase.rect.height;
                    GetComponent<RectTransform>().sizeDelta = new Vector2(orig.width, orig.height);
                    break;

                default:
                    EquipmentBase.gameObject.SetActive(true);

                    int numLocked = 5 - pItem.MaxEnchantments;

                    for(int i = 0; i < 5; i++) {
                        if(i > numLocked)
                            EnchantmentImages[i].sprite = AppUI.Instance.enchantmentLockedSprite;
                        else if(i < pItem.ActiveEnchantments.Count) {
                            EnchantmentImages[i].sprite = pItem.ActiveEnchantments[i].Image;
                            EnchantmentImages[i].color = Color.white;
                        } else {
                            EnchantmentImages[i].sprite = AppUI.Instance.enchantmentEmptySprite;
                        }
                    }

                    break;
            }
        }
    }
}
