using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace App.UI {
    public class NotificationUI : MonoBehaviour {

        [SerializeField] TextMeshProUGUI LabelField;
        [SerializeField] TextMeshProUGUI DaysField;
        [SerializeField] Image Portrait;
        [SerializeField] Image TileImage;

        public void AssignData(Notification pData) {
            LabelField.text = pData.Label;
            DaysField.text = pData.DayLimit + " Days Remaining";
            // Portrait.sprite = pData.Mercenary.Portrait;
            TileImage.sprite = pData.Tile.Data.DrawnSprite;
        }
    }
}
