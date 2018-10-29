using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Numeralien.Utilities;
using App.UI;

    public class DungeonCamera : MonoBehaviour
    {

        [SerializeField] float zoomSpeed;
        [SerializeField] public Vector2 ZoomBounds;

        [SerializeField] float speedSmoothing;
        [SerializeField] float zoomSmoothing;

        [SerializeField] public Vector2 PanBoundsX;
        [SerializeField] public Vector2 PanBoundsY;

        Vector3 targetPos;
        float targetCamSize;

        bool dirty;
        public bool Dirty
        {
            get { return dirty; }
            set { dirty = value; }
        }

        public float Zoom { get { return Camera.main.orthographicSize; } }

        private void Awake()
        {
            targetCamSize = Camera.main.orthographicSize;
            targetPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {

            // Get input
            float z = Input.GetAxis("Mouse ScrollWheel");

            // Set target position and zoom level
            targetPos = new Vector3(Mathf.Clamp(targetPos.x, PanBoundsX.x, PanBoundsX.y), Mathf.Clamp(targetPos.y, PanBoundsY.x, PanBoundsY.y), 0);
            targetCamSize = Mathf.Clamp(targetCamSize + zoomSpeed * -z, ZoomBounds.x, ZoomBounds.y);

            // Smoothly adjust target position and zoom level
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetCamSize, zoomSmoothing * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPos, speedSmoothing * Time.deltaTime);


            // Set whether the Y position or zoom has changed (for the scroll effect)
            if (z != 0) dirty = true;
        }

        public void SetTargetPosition(Vector3 targetPos)
        {
            this.targetPos = new Vector3(targetPos.x, targetPos.y, transform.position.z);
            dirty = true;
        }

        public void SetTargetZoom(float targetZoom)
        {
            targetCamSize = targetZoom;
        }
    }
