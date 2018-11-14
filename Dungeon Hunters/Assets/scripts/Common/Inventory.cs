using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App {
    public class Inventory {

        List<Item> items;
        public Item this[Item item] {
            get {
                return items[items.IndexOf(item)];
            }
        }

        public bool AddItem(Item item) {
            items.Add(item);

            return true;
        }
    }
}
