using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Numeralien.Utilities;

namespace Overworld {
    public class HexTile : MonoBehaviour {
        bool obscured = true;

        [HideInInspector]
        public SpriteRenderer HexRenderer;

        GameObject fog;
        GameObject blight;
        GameObject smoke;
        GameObject mountainClouds;
        GameObject birds;

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
                blightLevel = Mathf.Clamp(blightLevel + value, 0f, 0.5f);
                if(blight) {
                    SetBlight(blightLevel);
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

            Transform[] ch = GetComponentsInChildren<Transform>();

            fog = ch[1].gameObject;
            smoke = ch[2].gameObject;
            mountainClouds = ch[3].gameObject;
            birds = ch[4].gameObject;
            blight = ch[5].gameObject;

            SetSmoke(false);
            mountainClouds.SetActive(false);
            blight.SetActive(false);
            birds.SetActive(false);
        }

        private void OnMouseDown() {
            Camera.main.GetComponent<OverworldCamera>().Cursor.SetTarget(this);
        }

        public void RemoveFog() {
            if (obscured)
            {
                obscured = false;
                fog.SetActive(false);

                HexRenderer.enabled = true;
            }
        }

        public void SetSmoke(bool enabled = true) {
            smoke.SetActive(enabled);
        }

        public void SetBlight(float intensity, bool enabled = true) {
            blight.SetActive(enabled);
            blight.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, intensity);
        }

        public void SetMountainClouds() {
            mountainClouds.SetActive(true);
        }

        public void SetBirds() {
            birds.SetActive(true);
        }
    }
}
