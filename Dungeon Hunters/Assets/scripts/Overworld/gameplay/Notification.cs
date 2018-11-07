using UnityEngine;
using App.UI;
using Overworld;

namespace App {
    public class Notification {

        string label;
        MercenaryData mercenary;
        public MercenaryData Mercenary {
            get { return mercenary; }
        }

        private HexTile tile;
        public HexTile Tile {
            get { return tile; }
        }

        private EncounterEvent myEvent;
        public EncounterEvent Event {
            get { return Event; }
        }

        private bool isRequired;
        public bool IsRequired {  get { return isRequired; } }

        private int dayLimit;
        public int DayLimit { get { return dayLimit; } }

        private TaskType type;
        public TaskType Type {
            get { return type; }
        }

        public Notification(TaskType pType, EncounterEvent pEvent, int pDayLimit = 0, bool pIsRequired = false, MercenaryData pMercenary = null, HexTile pTile = null) {
            mercenary = pMercenary;
            tile = pTile;
            type = pType;

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
