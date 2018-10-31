using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App.UI;

namespace App {
    public class NotificationController : MonoBehaviour {

        public List<Notification> Notifications;

        public static NotificationController Instance;

        public bool HasRequiredNotifications {
            get {
                // The notification controller unlocks the next day
                // when there are no notifications in the list that
                // are required and have no time remaining.
                foreach(Notification n in Notifications) {
                    if (n.IsRequired && n.DayLimit == 0)
                        return true;
                }

                return false;
            }
        }

        public bool HasOptionalNotifications
        {
            get
            {
                // Return whether there are notifications
                // that will go away if the day is advanced
                // (but won't block advancement entirely).
                foreach (Notification n in Notifications)
                {
                    if (!n.IsRequired && n.DayLimit == 0)
                        return true;
                }

                return false;
            }
        }

        // Use this for initialization
        void Awake() {
            // Put the single in singleton
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            UpdateNotificationBadgeText();
        }

        public void AddNotification(Notification n) {
            Notifications.Add(n);

            UpdateNotificationBadgeText();
        }

        public void UpdateNotificationBadgeText() {
            AppUI.Instance.NotificationBadge.text = "" + Notifications.Count;
        }
    }
}
