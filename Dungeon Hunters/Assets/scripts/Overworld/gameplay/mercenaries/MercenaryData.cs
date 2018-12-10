using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;
using App.Data;

namespace App {
    public class MercenaryData {

        public GameObject LocationMarker;

        public Inventory Inventory;
        public List<Item> Equipment;

        private HexTile location;
        public HexTile Location {
            get { return location; }
            set { location = value; }
        }
        
        public List<HexTile> CurrentPath;
        private int PathIndex;
        private int PathTerrainCounter;

        private string name;
        public string Name { get { return name; } }

        private StatBlock stats;
        public StatBlock Stats {
            get { return stats; }
        }

        private int equipmentPower;
        public int EquipmentPower {
            get { return equipmentPower; }
        }
        public int MaxEquipmentPower {
            get {
                return Stats.Rank * 10;
            }
        }

        public bool IsTraveling {
            get { return CurrentPath != null; }
        }

        public Sprite Portrait;

        public MercenaryData(string pName, StatBlock pStats, int pRank = 1, Inventory pInventory = null) {
            name = pName;
            stats = pStats;

            Equipment = new List<Item>();

            if(Inventory == null) {
                // Inventories are always 5x5 for now.
                Inventory = new Inventory(5, 5);
            } else {
                Inventory = pInventory;
            }
        }

        /// <summary>
        /// Equip an item.
        /// </summary>
        /// <param name="pItem">Item to equip.</param>
        public bool EquipItem(Item pItem) {
            // Can only equip an item if its power wouldn't cause the mercenary to
            // exceed its current maximum equipment power
            if (EquipmentPower + pItem.Power > MaxEquipmentPower) return false;

            equipmentPower += pItem.Power;

            pItem.IsEquipped = true;
            Equipment.Add(pItem);

            return true;
        }

        /// <summary>
        /// Unequip an item.
        /// </summary>
        /// <param name="pItem">Item to unequip.</param>
        public void UnequipItem(Item pItem) {
            if (Equipment.Contains(pItem)) {
                equipmentPower -= pItem.Power;

                pItem.IsEquipped = false;
                Equipment.Remove(pItem);
            }
        }

        #region Pathfinding
        public void SetPath(List<HexTile> pPath) {
            CurrentPath = pPath;
            PathIndex = 0;
            PathTerrainCounter = 0;
        }

        public void SetLocation(HexTile pTile) {
            location = pTile;
        }

        public void UpdateLocation() {
            if (CurrentPath != null) {
                PathTerrainCounter++;

                if (PathTerrainCounter == HexFunctions.Instance.GetRoughTerrainFactor(Location.Type)) {
                    PathIndex++;
                    PathTerrainCounter = 0;

                    Location = CurrentPath[PathIndex];

                    if (PathIndex == CurrentPath.Count - 1) {
                        CurrentPath = null;
                    }
                }
            }
        }
        #endregion
    }
}
