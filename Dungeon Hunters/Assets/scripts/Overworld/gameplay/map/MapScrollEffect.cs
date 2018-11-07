using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Numeralien.Utilities;

namespace Overworld {
    public class MapScrollEffect : MonoBehaviour {

        OverworldCamera Cam;

        bool dirty;

        [SerializeField] float yMagnitude;
        [SerializeField] float xMagnitude;

        List<HexTile> Tiles;

        // Use this for initialization
        void Start() {
            Cam = Camera.main.GetComponent<OverworldCamera>();

            Tiles = HexMap.Instance.Tiles.Values.ToList();

            dirty = true;
        }

        // Update is called once per frame
        void Update() {
            if(dirty) {
                foreach(HexTile t in Tiles) {
                    // Update the display address of each tile depending on the camera's current position

                    // Subtract a small distance from the y position of the tile depending on its position above the
                    // camera's center. It'll look like it's stretching out to a horizon, hopefully.
                    t.DisplayPosition = new Vector3(
                        DisplacementX(t),
                        DisplacementY(t), // Multiply Y by a magnitude factor (geometric increase) and the distance between it and the y-center of the camera.
                        t.DisplayPosition.z
                    );
                }
            }
            dirty = Cam.Dirty;

        }

        public void GetTileList() {
            Tiles = HexMap.Instance.Tiles.Values.ToList();
        }

        public float DisplacementY(HexTile tile) {
            return tile.Address.ToUnity.y - Mathf.Clamp((yMagnitude * (tile.DisplayPosition.y) * tile.Address.Y), 0, float.MaxValue) * Math.Map(Cam.Zoom, Cam.ZoomBounds.x, Cam.ZoomBounds.y, 0f, 1f);
        }

        public float DisplacementX(HexTile tile) {
            return tile.Address.ToUnity.x + ((Math.Map(Cam.transform.position.x, Cam.PanBoundsX.x, Cam.PanBoundsX.y, -1f, 1f) * tile.Address.Y * xMagnitude) * Math.Map(Cam.Zoom, Cam.ZoomBounds.x, Cam.ZoomBounds.y, 0f, 1f));
        }
    }
}
