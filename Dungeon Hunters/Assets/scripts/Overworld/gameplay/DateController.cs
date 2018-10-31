using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Numeralien.Utilities;

namespace App.UI {
    public class DateController : MonoBehaviour {

        public static DateController Instance;

        [SerializeField, ReadOnly] public Date CurrentDate;

        // Use this for initialization
        void Awake() {
            // Put the single in singleton
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            CurrentDate = new Date(3, 5, 998);
            AppUI.Instance.DateText.text = CurrentDate.ToString();
        }

        public void AdvanceDate() {
            if (!NotificationController.Instance.HasRequiredNotifications) {
                AdvanceNotifications();
            } else if (NotificationController.Instance.HasRequiredNotifications) {
                // Otherwise show the message that says there are notifications remaining.
            } else if (NotificationController.Instance.HasOptionalNotifications) {
                // Print a warning that advancing the day will throw out optional notifications.
                // If the button is pressed again, advance.
                AdvanceNotifications();
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

            AppUI.Instance.DateText.text = CurrentDate.ToString();
        }
    }
}