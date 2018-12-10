using UnityEngine;
using App.UI;
using App.Data;
using Overworld;

namespace App {
    public class Notification {
        
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

        private string label;
        public string Label {
            get { return label; }
        }

        private int dayLimit;
        public int DayLimit { get { return dayLimit; } }

        private TaskType type;
        public TaskType Type {
            get { return type; }
        }

        public Notification(TaskType pType, EncounterEvent pEvent, string pLabel, int pDayLimit = 0, MercenaryData pMercenary = null, HexTile pTile = null) {
            mercenary = pMercenary;
            tile = pTile;
            type = pType;
            myEvent = pEvent;
            
            dayLimit = pDayLimit;

            label = pLabel;
        }

        public void Advance() {
            dayLimit--;
            if(dayLimit < 0) {
                NotificationController.Instance.Notifications.Remove(this);
            }
        }

        public void Resolve() {
            // Get the EncounterEvent associated with this Notification and begin the encounter resolution.
            EncounterController.Instance.InitializeNewEncounter(myEvent, mercenary);
        }
    }
}
