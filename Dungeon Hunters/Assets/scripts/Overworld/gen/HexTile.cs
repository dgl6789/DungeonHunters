using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Numeralien.Utilities;

namespace Overworld {
    public class HexTile : MonoBehaviour {
        public bool Obscured = true;

        [HideInInspector]
        public SpriteRenderer HexRenderer;

        Transform[] tileEffects;
        // 1 - Fog of War
        // 2 - Civilization Smoke
        // 3 - Mountain Clouds
        // 4 - Birds
        // 5 - Blight
        // 6 - Water Glint

        [ReadOnly]
        public HexData Data;

        Vector3 displayPosition;
        public Vector3 DisplayPosition {
            get { return displayPosition; }
            set {
                displayPosition = value;
                if (this) {
                    transform.position = displayPosition;
                }
            }
        }

        float blightLevel;
        public float BlightLevel {
            get { return blightLevel; }
            set {
                blightLevel = Mathf.Clamp(blightLevel + value, 0f, 0.75f);
                if(tileEffects[5]) {
                    SetTileEffectState(5, true, blightLevel);
                }
            }
        }

        [SerializeField, ReadOnly] TileType type;
        public TileType Type {
            get { return type; }
            set {
                type = value;

                foreach(HexSprite s in HexFunctions.Instance.HexSpriteLibrary) {
                    if(s.Type == value) {
                        HexRenderer.sprite = s.GetSprite();
                        return;
                    }
                }
            }
        }

        [SerializeField] HexAddress address;
        public HexAddress Address {
            get { return address; }
            set { address = value; }
        }

        private void Awake() {
            HexRenderer = GetComponent<SpriteRenderer>();

            tileEffects = GetComponentsInChildren<Transform>(true);

            Obscured = true;
        }

        public void SetTileEffectState(int index, bool enabled = true, params float[] parameters) {
            if (index >= tileEffects.Length || index <= 0) return;

            tileEffects[index].gameObject.SetActive(enabled);

            switch(index) {
                case 1: // Fog of war
                    if (Obscured && !enabled) {
                        Obscured = false;
                        tileEffects[index].gameObject.SetActive(false);

                        HexRenderer.enabled = true;
                    }
                    break;
                case 5: // Blight
                    tileEffects[index].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, parameters[0]);
                    break;
            }
        }

        public void Deobfuscate() {
            Obscured = false;
            SetTileEffectState(1, false);
        }
    }
}
