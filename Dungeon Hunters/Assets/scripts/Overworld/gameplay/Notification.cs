using UnityEngine;
using App.UI;
using Overworld;

namespace App {
    public class Notification : MonoBehaviour {

        string label;
        MercenaryData mercenary;
        public MercenaryData Mercenary {
            get { return mercenary; }
        }

        private HexTile tile;
        public HexTile Tile {
            get { return tile; }
        }

        private bool isRequired;
        public bool IsRequired {  get { return isRequired; } }

        private int dayLimit;
        public int DayLimit { get { return dayLimit; } }

        public Notification(string pLabel, int pDayLimit = 0, bool pIsRequired = false, MercenaryData pMercenary = null) {
            mercenary = pMercenary;
            label = pLabel;

            isRequired = pIsRequired;
            dayLimit = pDayLimit;
        }

        public GameObject NotificationObject() {
            return GameObject.Instantiate(AppUI.Instance.NotificationObject);
        }

        public void Advance() {
            dayLimit--;
            if(dayLimit < 0) {
                NotificationController.Instance.Notifications.Remove(this);
            }
        }
    }
}
