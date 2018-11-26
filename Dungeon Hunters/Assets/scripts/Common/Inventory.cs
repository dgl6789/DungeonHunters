using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App {
    public class Inventory {

        private int[,] layout;

        private List<Item> items;
        public Item this[Item item] {
            get {
                return items[items.IndexOf(item)];
            }
        }

        public int PowerLevel {
            get {
                int p = 0;
                foreach(Item i in items) {
                    p += i.Power;
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
            items = new List<Item>();

            layout = new int[sizeX, sizeY];
            for(int x = 0; x < layout.GetLength(0); x++) {
                for (int y = 0; y < layout.GetLength(1); y++) {
                    layout[x, y] = -1;
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
            if (!item.IsInitialized) item.InitializeShape();

            if (TestAddition(item, x, y)) {
                if (items.Contains(item)) {
                    RemoveItem(item);
                    AddItem(item, x, y);
                }

                items.Add(item);
                item.Inventory = this;

                foreach (Vector2Int i in item.Configuration.Indices) {
                    layout[i.x + x, i.y + y] = items.IndexOf(item);
                }

                item.AbsoluteCenter = new Vector2Int(x, y);

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
            foreach (Vector2Int i in item.Configuration.Indices) {
                int tX = i.x + x;
                int tY = i.y + y;

                if (tX < -1 || tX > layout.GetLength(0) || 
                    tY < -1 || tY > layout.GetLength(1) || 
                    !(layout[tX, tY] == -1 || layout[tX, tY] == items.IndexOf(item)))
                    return false;
            }

            return true;
        }
        
        /// <summary>
        /// Remove an item from the inventory.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public void RemoveItem(Item item) {
            if (!items.Contains(item)) return;

            int targetIndex = items.IndexOf(item);

            for(int x = 0; x < layout.GetLength(0); x++) {
                for (int y = 0; y < layout.GetLength(1); y++) {
                    if (layout[x, y] == targetIndex) layout[x, y] = -1;
                }
            }

            items.Remove(item);
            item.Inventory = null;
        }

        public Rect ItemBounds(Item i) {
            int targetIndex = items.IndexOf(i);

            int largestHIndex = 0;
            int smallestHIndex = int.MaxValue;
            int largestWIndex = 0;
            int smallestWIndex = int.MaxValue;

            if(items.Contains(i)) {
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

            if (index > -1 && index < items.Count)
                return items[index];

            return null;
        }
    }
}
