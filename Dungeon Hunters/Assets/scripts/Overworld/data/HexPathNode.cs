using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld {
    public class HexPathNode {

        public HexTile Tile { get; private set; }
        public HexAddress Address { get { return Tile.Address; } }

        public int BaseCost { get { return HexFunctions.Instance.GetRoughTerrainFactor(Tile.Type); } }

        public int G { get; set; }
        public int H { get; set; }
        public int F { get { return G + H; } }
        
        public HexPathNode Parent;

        public HexPathNode(HexTile pTile) {
            Tile = pTile;
        }
    }
}
