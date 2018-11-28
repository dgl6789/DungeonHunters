using Numeralien.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App {
    public enum ShapeBase { I, O, T, J, DOT, DASH }

    public enum ItemType { Trinket, Armor, Weapon, Resource }

    public class ItemShape {
        ShapeBase Shape;

        private Vector2Int[] indices;
        public Vector2Int[] Indices {
            get { return indices; }
        }

        public ItemShape(ShapeBase pShape, int pRotation) {
            Shape = pShape;

            indices = GetInitialConfiguration(Shape, pRotation);
        }

        public static Vector2Int[] GetInitialConfiguration(ShapeBase pBase, int pRotation) {
            switch(pBase) {
                default:
                case ShapeBase.DOT:
                    // Dots have uniform rotation
                    // O
                    return new Vector2Int[] { new Vector2Int(0, 0) };
                case ShapeBase.I:
                    // Rotations for I shape
                    // o
                    // O
                    // o
                    switch(pRotation) {
                        default:
                        case 0:
                        case 2:
                            return new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(0, -1) };
                        case 1:
                        case 3:
                            return new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) };
                    }
                case ShapeBase.O:
                    // O shape has uniform rotation
                    // o o
                    // O o
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(1, 0) };
                case ShapeBase.T:
                    // Rotations for T shape
                    // o O o
                    //   o
                    switch (pRotation) {
                        default:
                        case 0:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) };
                        case 1:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
                        case 2:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1) };
                        case 3:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
                    }
                case ShapeBase.J:
                    // Rotations for J shape
                    //   o
                    // o O
                    switch (pRotation) {
                        default:
                        case 0:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1) };
                        case 1:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1) };
                        case 2:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1) };
                        case 3:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, -1) };
                    }
                case ShapeBase.DASH:
                    // Rotations for Dash shape
                    // O o
                    switch (pRotation) {
                        default:
                        case 0:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1) };
                        case 1:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(-1, 0) };
                        case 2:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, -1) };
                        case 3:
                            return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0) };
                    }
            }
        }

        public override string ToString() {
            string s = "";
            foreach(Vector2Int i in indices) {
                s += i == indices[indices.Length - 1] ? i.ToString() : i.ToString() + ", ";
            }

            return s;
        }
    }

    [CreateAssetMenu(fileName = "Item", menuName = "Data/Item", order = 3)]
    public class Item : ScriptableObject {

        // Instance-agnostic fields //
        public string Name;

        public int BasePower;

        public string FlavorText;

        public ItemType Type;

        public ShapeBase Shape;

        public bool Stackable;

        public int MaxEnchantments;
        public List<Enchantment> EnchantmentPool;

        public Sprite Image;

        // Instance-specific fields //
        [HideInInspector] public int StackSize = 1;

        [HideInInspector] public bool IsEquipped;

        [HideInInspector] public List<Enchantment> ActiveEnchantments;
        [HideInInspector] public Inventory Inventory;

        [HideInInspector] public ItemShape LocalConfiguration;
        [HideInInspector] public ItemShape GlobalConfiguration;
        [HideInInspector] public int RotationIndex;

        // Total power = sum of enchantments' powers + item base power
        public int Power {
            get {
                int p = BasePower;

                foreach (Enchantment e in ActiveEnchantments) {
                    p += e.Power;
                }

                return p;
            }
        }

        /// <summary>
        /// Generate an item from a prefab item definition.
        /// </summary>
        /// <param name="pItem">The instance of an item that should be initialized.</param>
        /// <param name="pGenerateEnchantments">Whether random enchantments should be generated for the item.</param>
        /// <param name="x">The x index in the inventory at which to initialize.</param>
        /// <param name="y">The y index in the inventory at which to initialize.</param>
        /// <param name="rotation">The rotation of the shape (0-3)</param>
        /// <param name="pInventory">The inventory, if any, that this item should be initialized into.</param>
        /// <returns>An initialized instance of the provided item prefab.</returns>
        public static Item Initialize(Item pItem, int x = -1, int y = -1, int rotation = 0, bool pGenerateEnchantments = false, Inventory pInventory = null) {
            // Instantiate the instance
            Item instance = Instantiate(pItem);

            // Set up the local configuration so that the item knows where its own
            // local origin and bounds are.
            instance.LocalConfiguration = new ItemShape(instance.Shape, rotation);

            // Set up the global configuration so that it's in the
            // local coordinate space of its context (if a context was given).
            instance.GlobalConfiguration = new ItemShape(instance.Shape, rotation);
            if(x > -1 && y > -1) {
                for (int i = 0; i < instance.GlobalConfiguration.Indices.Length; i++) {
                    instance.GlobalConfiguration.Indices[i] = new Vector2Int(x + instance.GlobalConfiguration.Indices[i].x, y + instance.GlobalConfiguration.Indices[i].y);
                }
            }

            // Set the rotation index if applicable (so the UI knows how to rotate the sprite)
            instance.RotationIndex = IsUniformRotatingShape(pItem.Shape) ? 0 : rotation;
            
            // Associate this item with an inventory if one was given
            if(pInventory != null) instance.Inventory = pInventory;

            // Generate enchantments, if necessary
            if (pGenerateEnchantments && IsEnchantable(pItem)) GenerateEnchantments(instance, pItem.MaxEnchantments);

            return instance;
        }

        /// <summary>
        /// Add random enchantments to an instance of an item.
        /// </summary>
        /// <param name="toGenerate"></param>
        public static void GenerateEnchantments(Item pItem, int toGenerate) {
            pItem.ActiveEnchantments.Clear();

            for(int i = 0; i < Mathf.Min(pItem.EnchantmentPool.Count, toGenerate); i++) {
                Enchantment toAdd = pItem.EnchantmentPool[Random.Range(0, pItem.EnchantmentPool.Count)];

                if(!pItem.ActiveEnchantments.Contains(toAdd))
                    pItem.ActiveEnchantments.Add(toAdd);
            }
        }

        /// <summary>
        /// Return whether an item is enchantable. Works with instances and prefabs.
        /// </summary>
        /// <param name="pItem">Item to check.</param>
        /// <returns>Whether the given item is enchantable.</returns>
        public static bool IsEnchantable(Item pItem) {
            // This should just return true if the item isn't one that never should
            // receive enchantments, such as a resource. The list could expand in
            // the future, but for now this is a kind of verbose way of doing this.
            return pItem.Type != ItemType.Resource;
        }

        /// <summary>
        /// Get whether an item must be equipped in order to gain the benefits
        /// of its power level and enchantments.
        /// </summary>
        /// <param name="pItem">Item to inspect.</param>
        /// <returns>Whether the item is of an equippable type.</returns>
        public static bool IsEquippable(Item pItem) {
            return
                pItem.Type == ItemType.Armor ||
                pItem.Type == ItemType.Weapon;
        }

        #region Transformation Getters
        /// <summary>
        /// Get the rect (relative to the item's own local coordinate system) of the item's bounding box.
        /// </summary>
        /// <param name="pItem">Item to get bounds for.</param>
        /// <returns>Item bounds in local item coordiantes.</returns>
        public static Rect GetLocalBounds(Item pItem) { return GetBounds(pItem, false); }

        /// <summary>
        /// Get the rect (relative to the item's inventory's coordinate system) of the item's bounding box.
        /// </summary>
        /// <param name="pItem">Item to get bounds for.</param>
        /// <returns>Item bounds in global item coordinates.</returns>
        public static Rect GetGlobalBounds(Item pItem) { return GetBounds(pItem, true); }

        /// <summary>
        /// Get the rect (relative to the item's own local coordinate system) of a specific point in the item's configuration.
        /// </summary>
        /// <param name="pItem">Item to get point bounds for.</param>
        /// <param name="x">X coordinate of the point in the item's configuration.</param>
        /// <param name="y">Y coordinate of the point in the item's configuration.</param>
        /// <returns>Rect of the point in the item's local configuration.</returns>
        public static Rect GetLocalPointRect(Item pItem, int x, int y) {
            Rect bounds = GetLocalBounds(pItem);
            return new Rect(Mathf.Abs(bounds.x) - x, Mathf.Abs(bounds.y) - y, 1, 1);
        }

        /// <summary>
        /// Get the bounds of an item.
        /// </summary>
        /// <param name="pItem">Item to get bounds for.</param>
        /// <param name="pGlobal">Whether to get the global (inventory) coordinates or the local item coordinates.</param>
        /// <returns>Bounds of the given item.</returns>
        private static Rect GetBounds(Item pItem, bool pGlobal = false) {
            int largestHIndex = 0;
            int smallestHIndex = int.MaxValue;
            int largestWIndex = 0;
            int smallestWIndex = int.MaxValue;

            Vector2Int[] it = pGlobal ? pItem.GlobalConfiguration.Indices : pItem.LocalConfiguration.Indices;

            foreach(Vector2Int i in it) {
                if (i.x > largestWIndex) largestWIndex = i.x;
                if (i.y > largestHIndex) largestHIndex = i.y;
                if (i.x < smallestWIndex) smallestWIndex = i.x;
                if (i.y < smallestHIndex) smallestHIndex = i.y;
            }

            return new Rect(smallestWIndex, smallestHIndex, largestWIndex - smallestWIndex + 1, largestHIndex - smallestHIndex + 1);
        }

        public static Rect LocalBoundsBeforeRotation(Item pItem) {
            ItemShape bbr = new ItemShape(pItem.Shape, 0);

            int largestHIndex = 0;
            int smallestHIndex = int.MaxValue;
            int largestWIndex = 0;
            int smallestWIndex = int.MaxValue;

            foreach (Vector2Int i in bbr.Indices) {
                if (i.x > largestWIndex) largestWIndex = i.x;
                if (i.y > largestHIndex) largestHIndex = i.y;
                if (i.x < smallestWIndex) smallestWIndex = i.x;
                if (i.y < smallestHIndex) smallestHIndex = i.y;
            }

            return new Rect(smallestWIndex, smallestHIndex, largestWIndex - smallestWIndex + 1, largestHIndex - smallestHIndex + 1);
        }

        /// <summary>
        /// Get whether the shape has specific rotation matrices (i.e. rotation of an object
        /// with the shape changes the coordinates of the local or global system)
        /// </summary>
        /// <param name="pShape">Shape to inspect.</param>
        /// <returns>Whether the shape will change under any 90 degree rotation.</returns>
        private static bool IsUniformRotatingShape(ShapeBase pShape) {
            return pShape == ShapeBase.DOT || pShape == ShapeBase.O;
        }
        #endregion
    }
}
