using Numeralien.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App {
    public enum EnchantmentType { Sharpness, Protection, Luck, Foraging }

    [CreateAssetMenu(fileName = "Enchantment", menuName = "Data/Enchantment", order = 4)]
    public class Enchantment : ScriptableObject {

        public EnchantmentType Type;

        public int Power;

        public Sprite Image;
    }
}
