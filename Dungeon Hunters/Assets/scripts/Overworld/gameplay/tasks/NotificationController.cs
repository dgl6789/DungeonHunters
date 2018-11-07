using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using App.UI;

namespace App {
    public class NotificationController : MonoBehaviour {

        public List<Notification> Notifications;

        public static NotificationController Instance;

        [SerializeField] GameObject NotificationContainer;
        [SerializeField] GameObject NotificationObject;

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

            Notifications = new List<Notification>();

            UpdateNotificationBadgeText();
        }

        public void AddNotification(Notification n) {
            Notifications.Add(n);

            UpdateNotificationBadgeText();

            UpdateNotificationUI();
        }

        public void UpdateNotificationBadgeText() {
            AppUI.Instance.NotificationBadge.text = "" + Notifications.Count;
            AppUI.Instance.NotificationCount.text = Notifications.Count + " Notifications Remaining";
        }

        public void UpdateNotificationUI() {
            // Clear the existing notifications
            foreach (RectTransform g in NotificationContainer.GetComponentsInChildren<RectTransform>()) {
                if(g != NotificationContainer)
                    Destroy(g.gameObject);
            }

            // Sort the notification list by days remaining ascending
            List<Notification> orderedNotifications = Notifications.OrderBy(n => n.DayLimit).ToList();

            for(int i = 0; i < orderedNotifications.Count; i++) {
                // Instantiate a notification object with the correct data in the notification container.
                GameObject n = Instantiate(NotificationObject, NotificationContainer.transform);
                n.GetComponent<NotificationUI>().AssignData(orderedNotifications[i]);
                n.GetComponent<RectTransform>().anchoredPosition = new Vector2(n.GetComponent<RectTransform>().localPosition.x, i * n.GetComponent<RectTransform>().rect.height);
            }
        }
    }
}
