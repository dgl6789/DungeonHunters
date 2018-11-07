using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Overworld;

namespace App.UI {
    public class TileSelector : MonoBehaviour {

        HexTile targetTile;
        public HexTile TargetTile {
            get { return targetTile; }
        }
        Animator anim;

        [SerializeField] TextMeshProUGUI targetedTileText;
        [SerializeField] Image targetedTileImage;
        [SerializeField] TextMeshProUGUI targetedTileDescription;

        public static TileSelector Instance;

        // Use this for initialization
        void Awake() {
            anim = GetComponent<Animator>();

            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        private void Update() {
            if (targetTile) transform.position = targetTile.transform.position;

            if (Input.GetButtonDown("RightClick")) {
                HexTile h = CheckTileClick();

                if (h) SetTarget(h);
            }
        }

        public void SetTarget(HexTile tile) {
            targetTile = tile;

            Refocus();

            transform.position = targetTile.transform.position;

            // When the target is set, open the tile in the report section of the book based on its data.
            targetedTileText.text = tile.Data.Name;
            targetedTileImage.sprite = tile.Data.DrawnSprite;
            targetedTileDescription.text = tile.Data.Details;

            // If the stronghold is clicked, open the stronghold management panel.
            if (tile.Type == TileType.STRONGHOLD) { if (AppUI.Instance.lastPageOpened != 3) AppUI.Instance.SwitchPage(3); }

            anim.SetBool("Selection", true);
        }

        public void Deselect() {
            targetTile = null;

            anim.SetBool("Selection", false);
        }

        private HexTile CheckTileClick() {
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));

            return HexFunctions.Instance.WasTileClickedAt(clickPosition);
        }

        public void Refocus()
        {
            if (targetTile)
            {
                if (!AppUI.Instance.leftPanelOpen)
                    Camera.main.GetComponent<OverworldCamera>().SetTargetPosition(targetTile.transform.position);
                else
                {
                    float height = Camera.main.orthographicSize * 2.0f;
                    float width = height * Camera.main.aspect;

                    Camera.main.GetComponent<OverworldCamera>().SetTargetPosition(new Vector3(
                        targetTile.transform.position.x - (width / 4),
                        targetTile.transform.position.y,
                        targetTile.transform.position.z));
                }
            }
        }
    }
}
