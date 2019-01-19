using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Numeralien.Utilities;

namespace App.UI {
    public class DateController : MonoBehaviour {

        public static DateController Instance;

        [SerializeField] public Date CurrentDate;

        public float RandomEncounterChance;

        // Use this for initialization
        void Awake() {
            // Put the single in singleton
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            CurrentDate = new Date(3, 5, 998);
            AppUI.Instance.DateText.text = CurrentDate.ToString();
        }

        public void AdvanceDate() {
            if (!NotificationController.Instance.HasNotifications) {
                AdvanceNotifications();
            } else {
                // Otherwise show the message that says there are notifications remaining.
            }
        }

        /// <summary>
        /// Advance the notifications in the notification controller
        /// and adjust the date data and text field.
        /// </summary>
        void AdvanceNotifications()
        {
            CurrentDate.Advance();

            foreach (Notification n in NotificationController.Instance.Notifications) { n.Advance(); }
            foreach(MercenaryData m in MercenaryController.Instance.Mercenaries) { m.UpdateLocation(); }

            MercenaryController.Instance.UpdateLocationPins();

            NotificationController.Instance.UpdateNotificationUI();
            NotificationController.Instance.UpdateNotificationBadgeText();

            AppUI.Instance.DateText.text = CurrentDate.ToString();

            foreach(MercenaryData m in MercenaryController.Instance.Mercenaries) {
                if(m.IsTraveling) {
                    if(Random.Range(0f, 1f) < RandomEncounterChance) {
                        NotificationController.Instance.AddNotification(
                            new Notification(TaskType.RANDOM, EncounterController.Instance.GetRandomEncounter(), TaskController.Instance.ParseNotificationLabel(TaskType.RANDOM, m), 0, m, m.Location)
                        );
                    }
                }
            }
        }
    }
}