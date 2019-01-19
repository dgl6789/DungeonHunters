using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Numeralien.Utilities;

namespace Overworld
{
    [System.Serializable]
    public class HexAddress
    {
        [SerializeField]
        private int x;
        public int X { get { return x; } set { x = value; } }

        [SerializeField]
        private int y;
        public int Y { get { return y; } set { y = value; } }

        public static HexAddress operator+ (HexAddress a, HexAddress b) { return new HexAddress(a.X + b.X, a.Y + b.Y); }
        public static HexAddress operator+ (HexAddress a, Vector2Int b) { return new HexAddress(a.X + b.x, a.Y + b.y); }
        public static HexAddress operator+ (Vector2Int a, HexAddress b) { return new HexAddress(a.x + b.X, a.y + b.Y); }

        public HexAddress(int x, int y)
        {
            SetCoordinates(x, y);
        }

        public HexAddress(HexAddress a)
        {
            SetCoordinates(a.X, a.Y);
        }

        private void SetCoordinates(int pX, int pY)
        {
            x = pX;
            y = pY;
        }

        public Vector3 ToUnity {
            get {
                return new Vector3(
                    x * HexFunctions.Instance.OFFSET_X + (y % 2 == 0 ? HexFunctions.Instance.OFFSET_X / 2 : HexFunctions.Instance.OFFSET_X),
                    y * HexFunctions.Instance.OFFSET_Y,
                    y * HexFunctions.Instance.OFFSET_Y
                );
            }
        }

        public override string ToString() {
            return string.Format("({0}, {1})", X, Y);
        }

        public override bool Equals(object obj) {
            HexAddress other = (HexAddress)obj; 
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode() {
            var hashCode = -151278297;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(ToUnity);
            return hashCode;
        }
    }
}
