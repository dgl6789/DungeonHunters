using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace App.UI {
    public class NotificationUI : MonoBehaviour {

        [SerializeField] TextMeshProUGUI LabelField;
        [SerializeField] TextMeshProUGUI DaysField;

        [SerializeField] Button FocusMercButton;
        [SerializeField] Button FocusDestinationButton;
        [SerializeField] Button FinishButton;

        public void AssignData(Notification pData) {
            LabelField.text = pData.Label;
            
            DaysField.text = pData.DayLimit == 0 ? "Ready! Click the checkmark to resolve." : "Ready in " + pData.DayLimit + " days.";

            FocusMercButton.onClick.AddListener(() => TileSelector.Instance.SetTarget(pData.Mercenary.Location));
            FocusDestinationButton.onClick.AddListener(() => TileSelector.Instance.SetTarget(pData.Tile));

            // Listener for when the notification is ready to turn in
            FinishButton.onClick.AddListener(() => {
                    NotificationController.Instance.ClickNotification(NotificationController.Instance.Notifications.IndexOf(pData));
                    TileSelector.Instance.SetTarget(pData.Tile);
                }
            );
            
            FinishButton.interactable = (pData.DayLimit == 0);
        }
    }
}
