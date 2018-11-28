using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;

namespace App {
    public enum MercenarySkills { Constitution, Endurance, Stealth, Medicine, Athletics, Knowledge, Smithing, Magic, Survival, Insight, Coercion, Deception, Intimidation, Performance, Spot }

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

        private int mind, body, spirit, rank;
        public int Mind { get { return mind; } }
        public int Body { get { return body; } }
        public int Spirit { get { return spirit; } }
        public int Rank { get { return rank; } }

        private int equipmentPower;
        public int EquipmentPower {
            get { return equipmentPower; }
        }
        public int MaxEquipmentPower {
            get {
                return Rank * 10;
            }
        }

        public Sprite Portrait;

        public List<MercenarySkills> Skills;

        public MercenaryData(string pName, int pMind, int pBody, int pSpirit, List<MercenarySkills> pSkills, int pRank = 1, Inventory pInventory = null) {
            name = pName;
            mind = pMind;
            body = pBody;
            spirit = pSpirit;
            rank = pRank;

            Skills = new List<MercenarySkills>();
            Skills.AddRange(pSkills);

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
