using Numeralien.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App {
    public enum ShapeBase { I, O, T, J, L, S, Z, DOT, DASH }

    public enum ItemType { Trinket, Armor, Weapon, Resource }

    public class ItemShape {
        ShapeBase Shape;

        private Vector2Int[] indices;
        public Vector2Int[] Indices {
            get { return indices; }
        }

        public ItemShape(ShapeBase pShape) {
            Shape = pShape;

            indices = GetInitialConfiguration(Shape);
        }

        public static Vector2Int[] GetInitialConfiguration(ShapeBase pBase) {
            switch(pBase) {
                default:
                case ShapeBase.DOT:
                    return new Vector2Int[] { new Vector2Int(0, 0) };
                case ShapeBase.I:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) };
                case ShapeBase.O:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0) };
                case ShapeBase.T:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 1) };
                case ShapeBase.J:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) };
                case ShapeBase.L:
                    return new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 0) };
                case ShapeBase.DASH:
                    return new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1) };
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

        public string Name;

        public int BasePower;

        public int Power {
            get {
                int p = BasePower;

                foreach(Enchantment e in ActiveEnchantments) {
                    p += e.BasePower * e.Rank;
                }

                return p;
            }
        }

        public string FlavorText;

        public ItemType Type;

        public bool Stackable;
        [ReadOnly] public int StackSize = 1;

        public List<Enchantment> EnchantmentPool;

        [HideInInspector] public List<Enchantment> ActiveEnchantments;

        [HideInInspector] public Inventory Inventory;

        [HideInInspector] public bool IsInitialized;

        [HideInInspector] public Vector2Int AbsoluteCenter;

        public ShapeBase Shape;
        [HideInInspector] public ItemShape Configuration;

        public Sprite Image;

        public void InitializeShape() {
            if (!IsInitialized) {
                Configuration = new ItemShape(Shape);
                IsInitialized = true;
            }
        }

        public void GenerateEnchantments(int toGenerate) {
            ActiveEnchantments.Clear();

            for(int i = 0; i < Mathf.Min(EnchantmentPool.Count, toGenerate); i++) {
                ActiveEnchantments.Add(EnchantmentPool[Random.Range(0, EnchantmentPool.Count)]);
            }
        }
    }
}
