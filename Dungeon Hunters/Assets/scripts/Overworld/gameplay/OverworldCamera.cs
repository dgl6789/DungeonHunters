using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Numeralien.Utilities;
using App.UI;

namespace Overworld {
    public class OverworldCamera : MonoBehaviour {

        [SerializeField] float speed;
        [SerializeField] float zoomSpeed;
        [SerializeField] public Vector2 ZoomBounds;

        [SerializeField] float speedSmoothing;
        [SerializeField] float zoomSmoothing;

        [SerializeField] float panBorderThickness = 5f;
        [SerializeField] public Vector2 PanBoundsX;
        [SerializeField] public Vector2 PanBoundsY;

        Vector3 targetPos;
        float targetCamSize;

        [SerializeField] public TileSelector Cursor;

        [SerializeField] RectTransform zoomNib;
        [SerializeField] RectTransform zoomMeter;
        [SerializeField] float zoomMeterPadding;

        bool dirty;
        public bool Dirty {
            get { return dirty; }
            set { dirty = value; }
        }

        public float Zoom { get { return Camera.main.orthographicSize; } }

        private void Awake() {
            targetCamSize = Camera.main.orthographicSize;
            targetPos = transform.position;
        }

        // Update is called once per frame
        void Update() {
            Vector2 mousePos = Input.mousePosition;

            bool mouseIsInsideField =
            (Mathf.Pow(mousePos.x - Screen.width / 2, 4) / Mathf.Pow((Screen.width / 2) - panBorderThickness, 4)) +
            (Mathf.Pow(mousePos.y - Screen.height / 2, 4) / Mathf.Pow((Screen.height / 2) - panBorderThickness, 4))
            <= 1;

            float mousePanX = 0;
            float mousePanY = 0;

            if (!mouseIsInsideField) {
                mousePanX = mousePos.x / (Screen.width / 2) - 1f;
                mousePanY = mousePos.y / (Screen.height / 2) - 1f;
            }
            
            // Get input
            float x = Mathf.Clamp(Input.GetAxisRaw("Horizontal") + mousePanX, -1f, 1f);
            float y = Mathf.Clamp(Input.GetAxisRaw("Vertical") + mousePanY, -1f, 1f);
            float z = Input.GetAxis("Mouse ScrollWheel");

            // Set target position and zoom level
            targetPos = new Vector3(Mathf.Clamp(x * speed + targetPos.x, PanBoundsX.x, PanBoundsX.y), Mathf.Clamp(y * speed + targetPos.y, PanBoundsY.x, PanBoundsY.y), 0);
            targetCamSize = Mathf.Clamp(targetCamSize + zoomSpeed * -z, ZoomBounds.x, ZoomBounds.y);

            // Smoothly adjust target position and zoom level
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetCamSize, zoomSmoothing * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPos, speedSmoothing * Time.deltaTime);

            // Adjust the indicator on the UI to indicate zoom level
            float maxY = zoomMeter.rect.yMax - zoomMeterPadding;
            float minY = -(maxY);

            zoomNib.anchoredPosition = new Vector2(-8, -Math.Map(Camera.main.orthographicSize, ZoomBounds.x, ZoomBounds.y, minY, maxY));

            // Set whether the Y position or zoom has changed (for the scroll effect)
            if (y != 0 || z != 0) dirty = true;
        }

        public void SetTargetPosition(Vector3 targetPos) {
            this.targetPos = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        }

        public void SetTargetZoom(float targetZoom) {
            targetCamSize = targetZoom;
        }
    }
}