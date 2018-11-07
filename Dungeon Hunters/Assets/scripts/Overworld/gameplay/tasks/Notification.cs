using UnityEngine;
using App.UI;
using App.Data;
using Overworld;

namespace App {
    public class Notification {
<<<<<<< HEAD:Dungeon Hunters/Assets/scripts/Overworld/gameplay/Notification.cs

        string label;
=======
        
>>>>>>> bfbb8475ecec48e281a36d7b247ec358bf340dfb:Dungeon Hunters/Assets/scripts/Overworld/gameplay/tasks/Notification.cs
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

<<<<<<< HEAD:Dungeon Hunters/Assets/scripts/Overworld/gameplay/Notification.cs
=======
        private string label;
        public string Label {
            get { return label; }
        }

>>>>>>> bfbb8475ecec48e281a36d7b247ec358bf340dfb:Dungeon Hunters/Assets/scripts/Overworld/gameplay/tasks/Notification.cs
        private bool isRequired;
        public bool IsRequired {  get { return isRequired; } }

        private int dayLimit;
        public int DayLimit { get { return dayLimit; } }

        private TaskType type;
        public TaskType Type {
            get { return type; }
        }

<<<<<<< HEAD:Dungeon Hunters/Assets/scripts/Overworld/gameplay/Notification.cs
        public Notification(TaskType pType, EncounterEvent pEvent, int pDayLimit = 0, bool pIsRequired = false, MercenaryData pMercenary = null, HexTile pTile = null) {
            mercenary = pMercenary;
            tile = pTile;
            type = pType;
=======
        public Notification(TaskType pType, EncounterEvent pEvent, string pLabel, int pDayLimit = 0, bool pIsRequired = false, MercenaryData pMercenary = null, HexTile pTile = null) {
            mercenary = pMercenary;
            tile = pTile;
            type = pType;
            myEvent = pEvent;
>>>>>>> bfbb8475ecec48e281a36d7b247ec358bf340dfb:Dungeon Hunters/Assets/scripts/Overworld/gameplay/tasks/Notification.cs

            isRequired = pIsRequired;
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
            Debug.Log("Resolved.");
        }
    }
}
