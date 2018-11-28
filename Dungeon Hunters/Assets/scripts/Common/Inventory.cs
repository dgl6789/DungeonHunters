using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace App {
    public class Inventory {

        private int[,] layout;

        private Dictionary<int, Item> items;

        public int KeyOf(Item pItem) {
            return items.FirstOrDefault(i => i.Value == pItem).Key;
        }

        public int PowerLevel {
            get {
                int p = 0;
                foreach(KeyValuePair<int, Item> kvp in items) {
                        p += kvp.Value.Power;
                }

                return p;
            }
        }

        public int EquippedPowerLevel {
            get {
                int p = 0;
                foreach (KeyValuePair<int, Item> kvp in items) {
                    if (Item.IsEquippable(kvp.Value))
                        p += kvp.Value.IsEquipped ? kvp.Value.Power : 0;
                }

                return p;
            }
        }

        public int Width {
            get { return layout.GetLength(0); }
        }

        public int Height {
            get { return layout.GetLength(1); }
        }

        public Inventory(int sizeX, int sizeY) {
            items = new Dictionary<int, Item>();

            layout = new int[sizeX, sizeY];
            for(int x = 0; x < layout.GetLength(0); x++) {
                for (int y = 0; y < layout.GetLength(1); y++) {
                    layout[x, y] = 0;
                }
            }
        }

        /// <summary>
        /// Add an item to the inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="x">X position in the inventory layout of the item's origin.</param>
        /// <param name="y">Y position in the inventory layout of the item's origin.</param>
        /// <returns>True if the add succeeded, false if not.</returns>
        public bool AddItem(Item item, int x, int y) {

            if (TestAddition(item, x, y)) {
                if (items.ContainsValue(item)) {
                    RemoveItem(item);
                    AddItem(item, x, y);
                }

                int k = GetSmallestUnusedKey();

                items.Add(k, item);
                item.Inventory = this;

                // Update the item's globalconfiguration indices
                for(int i = 0; i < item.GlobalConfiguration.Indices.Length; i++) {
                    item.GlobalConfiguration.Indices[i] = new Vector2Int(x + item.LocalConfiguration.Indices[i].x, y + item.LocalConfiguration.Indices[i].y);
                }

                // Update the inventory's layout
                foreach (Vector2Int i in item.GlobalConfiguration.Indices) {
                    layout[i.x, i.y] = k;
                }

                return true;
            } else if (item.Stackable && item.Name == GetItemFromLayout(x, y).Name) {
                // Combining two stacks of like items.
                GetItemFromLayout(x, y).StackSize += item.StackSize;
                return true;
            } else return false;
        }

        /// <summary>
        /// Test whether a given item shape is valid.
        /// </summary>
        /// <param name="item">The item whose shape to test.</param>
        /// <param name="x">The X position of the item's origin.</param>
        /// <param name="y">The Y position of the item's origin.</param>
        /// <returns></returns>
        public bool TestAddition(Item item, int x, int y) {
            // If the indices are not available, return false.
            foreach (Vector2Int i in item.LocalConfiguration.Indices) {
                int tx = x + i.x;
                int ty = y + i.y;

                if (tx < 0 || tx > layout.GetLength(0) - 1 || ty < 0 || ty > layout.GetLength(1) - 1) return false;

                if (!(layout[tx, ty] == 0 || layout[tx, ty] == KeyOf(item))) return false;
            }

            return true;
        }
        
        /// <summary>
        /// Remove an item from the inventory.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public void RemoveItem(Item item) {
            int targetKey = KeyOf(item);

            if (targetKey == 0) return;

            for(int x = 0; x < layout.GetLength(0); x++) {
                for (int y = 0; y < layout.GetLength(1); y++) {
                    if (layout[x, y] == targetKey) layout[x, y] = 0;
                }
            }

            items.Remove(targetKey);
            item.Inventory = null;
        }

        /// <summary>
        /// Get a list of all the items in the inventory.
        /// </summary>
        /// <returns>List of all the items in the inventory.</returns>
        public List<Item> GetItems() {
            List<Item> i = new List<Item>();

            foreach(KeyValuePair<int, Item> item in items) {
                if (!i.Contains(item.Value)) i.Add(item.Value);
            }

            return i;
        }

        /// <summary>
        /// Get a list of all the active enchantments in the inventory. These are
        /// enchantments on equipment that is equipped, or enchantments on items
        /// that are not equippable, like trinkets.
        /// </summary>
        /// <returns>List of all the active enchantments in the inventory.</returns>
        public List<Enchantment> GetActiveEnchantments() {
            List<Enchantment> enchantments = new List<Enchantment>();

            foreach (Item i in GetItems()) {
                if(Item.IsEnchantable(i) && (i.IsEquipped || !Item.IsEquippable(i))) {
                    enchantments.AddRange(i.ActiveEnchantments);
                }
            }

            return enchantments;
        }

        public Rect ItemBounds(Item i) {
            int targetIndex = KeyOf(i);

            int largestHIndex = 0;
            int smallestHIndex = int.MaxValue;
            int largestWIndex = 0;
            int smallestWIndex = int.MaxValue;

            if(items.ContainsValue(i)) {
                for(int x = 0; x < layout.GetLength(0); x++) {
                    for (int y = 0; y < layout.GetLength(1); y++) {
                        if(layout[x, y] == targetIndex) {
                            if (x > largestWIndex) largestWIndex = x;
                            if (y > largestHIndex) largestHIndex = y;
                            if (x < smallestWIndex) smallestWIndex = x;
                            if (y < smallestHIndex) smallestHIndex = y;
                        }
                    }
                }
            }

            return new Rect(smallestWIndex, smallestHIndex, largestWIndex - smallestWIndex + 1, largestHIndex - smallestHIndex + 1);
        }

        public Item GetItemFromLayout(int x, int y) {
            int index = -1;
            if (x > -1 && x < layout.GetLength(0) && y > -1 && y < layout.GetLength(1))
                index = layout[x, y];
            else return null;

            if (index > 0 && items.ContainsKey(index)) return items[index];

            return null;
        }

        public override string ToString() {
            string s = "";

            for (int x = 0; x < layout.GetLength(0); x++) {
                for (int y = 0; y < layout.GetLength(1); y++) {
                    s += (y < layout.GetLength(1) - 1) ? layout[x, y] + ", " : layout[x, y] + "\n";
                }
            }

            return s;
        }

        private int GetSmallestUnusedKey() {
            int i = 1;
            while (items.ContainsKey(i)) i++;

            return i;
        }
    }
}
