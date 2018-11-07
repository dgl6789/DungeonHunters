using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numeralien.Utilities {
    public class RandomSprite : MonoBehaviour {

        [SerializeField] Sprite[] sprites;

        void Start() {
            GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }
}
