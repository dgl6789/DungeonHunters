using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App {
    public class Item {

        private string name;
        public string Name {
            get { return name; }
        }

        public Vector2Int Center {
            get {
                Vector2Int total = Vector2Int.zero;
                foreach(Vector2Int i in Shape) {
                    total.x += i.x;
                    total.y += i.y;
                }

                return total * (int)(1.0f / Shape.Length);
            }
        }

        public Vector2Int[] Shape;

        public Item(string pName, Vector2Int[] pShape) {
            name = pName;
            Shape = pShape;
        }

        public void RotateClockwise() {
            Rotate(90.0f);
        }

        public void RotateCounterclockwise() {
            Rotate(-90.0f);
        }

        private void Rotate(float pAngle) {
            for (int i = 0; i < Shape.Length; i++) {
                Vector2Int center = Center;
                Vector2Int newPoint = new Vector2Int(Shape[i].x, Shape[i].y);

                newPoint.x -= center.x;
                newPoint.y -= center.y;

                float s = Mathf.Sin(pAngle);
                float c = Mathf.Cos(pAngle);

                int newX = Mathf.RoundToInt(newPoint.x * c - newPoint.y * s);
                int newY = Mathf.RoundToInt(newPoint.x * c + newPoint.y * s);

                Shape[i] = new Vector2Int(newX, newY);
            }
        }

        public void FlipHorizontal() {
            for (int i = 0; i < Shape.Length; i++) {
                Shape[i].x *= -1;
            }
        }

        public void FlipVertical() {
            for (int i = 0; i < Shape.Length; i++) {
                Shape[i].y *= -1;
            }
        }
    }
}
