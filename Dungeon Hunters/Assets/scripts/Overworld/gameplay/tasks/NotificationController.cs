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

        public int NeedsResolutionCount {
            get {
                int num = 0;

                foreach (Notification n in Notifications) {
                    if (n.IsRequired && n.DayLimit == 0)
                        num++;
                }

                return num;
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

            string s = NeedsResolutionCount == 1 ? NeedsResolutionCount + " Notification Remaining" : NeedsResolutionCount + " Notifications Remaining";

            AppUI.Instance.AdvanceDayButton.interactable = !HasRequiredNotifications;

            AppUI.Instance.NotificationCount.text = s;
        }

        public void UpdateNotificationUI() {
            // Clear the existing notifications
            foreach (RectTransform g in NotificationContainer.transform) {
                if(g != NotificationContainer)
                    Destroy(g.gameObject);
            }

            // Sort the notification list by days remaining ascending
            List<Notification> orderedNotifications = Notifications.OrderBy(n => n.DayLimit).ToList();

            for(int i = 0; i < orderedNotifications.Count; i++) {
                // Instantiate a notification object with the correct data in the notification container.
                GameObject n = Instantiate(NotificationObject, NotificationContainer.transform);
                RectTransform rect = n.GetComponent<RectTransform>();
                
                n.GetComponent<NotificationUI>().AssignData(orderedNotifications[i]);

                rect.anchoredPosition = new Vector2(rect.localPosition.x,
                    (NotificationContainer.GetComponent<RectTransform>().rect.yMax - NotificationObject.GetComponent<RectTransform>().rect.height / 2) - i * (rect.rect.height + 3));
            }
        }

        /// <summary>
        /// When a notification is clicked, either focus to its tile and select its mercenary, 
        /// or go to the resolution.
        /// </summary>
        /// <param name="index">index of the notification to check in the notifications list</param>
        public void ClickNotification(int index) {
            // Should focus on the tile the merc is currently on and open the merc's page
            if(Notifications[index].IsRequired && Notifications[index].DayLimit != 0) {
                TileSelector.Instance.SetTarget(Notifications[index].Mercenary.Location);

                AppUI.Instance.SetSelectedMercenary(Notifications[index].Mercenary);
                AppUI.Instance.SwitchPage(2);
            } 
            // Should resolve the notification
            else if(Notifications[index].DayLimit == 0) {
                Notifications[index].Resolve();

                MercenaryData m = Notifications[index].Mercenary;
                Notifications.RemoveAt(index);

                UpdateNotificationUI();
                UpdateNotificationBadgeText();
                AppUI.Instance.UpdateMercInteractionButton(m);
            }
        }
    }
}
