using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using UnityEngine;
using Numeralien.Utilities;

namespace Overworld
{
    public enum TileType {
        NULL,
        TALL_MOUNTAIN,
        MOUNTAIN,
        FOREST,
        SPARSE_FOREST,
        DENSE_FOREST,
        GRASSLAND,
        HIGHLAND,
        WATER,
        OCEAN,
        ISLAND,
        OCEAN_ISLAND,
        VILLAGE,
        FARMLAND,
        DUNGEON,
        STRONGHOLD
    };

    [System.Serializable]
    public class HexSprite {
        [SerializeField] public TileType Type;
        [SerializeField] public Sprite[] Sprites;
        [SerializeField] public Sprite SketchedSprite;

        public Sprite GetSprite() { return Sprites[Random.Range(0, Sprites.Length)]; }
    }

    public class HexFunctions : MonoBehaviour {
        public static HexFunctions Instance;

        public Sprite BaseHexSprite;

        public HexSprite[] HexSpriteLibrary;
        public HexSprite[] AlternateSpriteLibrary;

        public List<Vector2Int> NeighborIndices;

        [HideInInspector]
        public float TILE_SCALE, OFFSET_X, OFFSET_Y, OFFSET_Z, TILE_RADIUS;

        List<TileType> LandTypes = new List<TileType>{
            TileType.TALL_MOUNTAIN, TileType.MOUNTAIN, TileType.FOREST,
            TileType.SPARSE_FOREST, TileType.DENSE_FOREST, TileType.GRASSLAND,
            TileType.HIGHLAND, TileType.DUNGEON, TileType.STRONGHOLD,
            TileType.ISLAND
        };

        public bool IsLand(HexTile tile) { return LandTypes.Contains(tile.Type); }

        List<TileType> FarmViableTypes = new List<TileType>{
            TileType.FOREST, TileType.SPARSE_FOREST, TileType.DENSE_FOREST, TileType.GRASSLAND
        };

        void Awake() {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            DontDestroyOnLoad(this);

            TILE_SCALE = BaseHexSprite.pixelsPerUnit;
            OFFSET_X = 1 / TILE_SCALE * (BaseHexSprite.rect.width - 1);
            OFFSET_Y = 1 / TILE_SCALE * (BaseHexSprite.rect.height - 12);
            OFFSET_Z = 1 / TILE_SCALE;

            TILE_RADIUS = 0.55f;
        }

        public int AdjacentLandTiles(HexAddress address) {
            int n = 0;

            foreach (Vector2Int i in NeighborIndices) {
                HexAddress a = address + i;
                if (HexMap.Instance.Tiles.ContainsKey(a) && LandTypes.Contains(HexMap.Instance.Tiles[a].Type)) n++;
            }

            return n;
        }

        public bool IsViableForFarm(HexAddress a) {
            return HexMap.Instance.Tiles.ContainsKey(a) && FarmViableTypes.Contains(HexMap.Instance.Tiles[a].Type);
        }

        public int AdjacentTilesOfTypes(HexAddress address, params TileType[] tileTypes) {
            int n = 0;
            foreach (TileType t in tileTypes) {
                n += AdjacentTilesOfType(address, t);
            }

            return n;
        }

        public int AdjacentTilesOfType(HexAddress address, TileType pType, bool invert = false) {
            int n = 0;

            foreach (Vector2Int i in NeighborIndices) {
                HexAddress a = address + i;
                if (!invert) {
                    if (HexMap.Instance.Tiles.ContainsKey(a) && HexMap.Instance.Tiles[a].Type == pType) n++;
                } else {
                    if (HexMap.Instance.Tiles.ContainsKey(a) && HexMap.Instance.Tiles[a].Type != pType) n++;
                }
            }

            return n;
        }

        public HexTile WasTileClickedAt(Vector3 clickPosition) {
            HexTile h = null;
            List<HexTile> possibleMatches = new List<HexTile>();

            foreach(HexTile tile in HexMap.Instance.Tiles.Values.ToList()) {
                if (Vector2.Distance(clickPosition, tile.DisplayPosition) < TILE_RADIUS) possibleMatches.Add(tile);
            }

            float minDist = float.MaxValue;

            foreach(HexTile tile in possibleMatches) {
                float thisDist = Vector2.Distance(clickPosition, tile.DisplayPosition);
                if (!h || thisDist < minDist) {
                    h = tile;
                    minDist = thisDist;
                }
            }

            return h;
        }

        public List<HexPathNode> GetPathFromTo(HexPathNode pStart, HexPathNode pEnd) {
            List<HexPathNode> openSet = new List<HexPathNode>();
            List<HexPathNode> closedSet = new List<HexPathNode>();

            openSet.Add(pStart);

            while (openSet.Count > 0) {
                HexPathNode current = openSet[0];
                for (int i = 1; i < openSet.Count; i++) {
                    if (openSet[i].F < current.F || (openSet[i].F == current.F && openSet[i].H < current.H)) {
                        current = openSet[i];
                    }
                }
            
                openSet.Remove(current);
                closedSet.Add(current);

                // Found a path
                if (current == pEnd) {
                    List<HexPathNode> path = new List<HexPathNode>();
            
                    HexPathNode c = pEnd;
                    while (c != pStart) {
                        path.Add(c);
                        c = current.Parent;
                    }
            
                    path.Reverse();
            
                    return path;
                }
            
                foreach (HexPathNode n in GetAdjacentPathNodes(current)) {
                    if (closedSet.Contains(n)) continue;
            
                    int costToNeighbor = current.G + DistanceFromTo(current, n) + n.BaseCost;
                    if (costToNeighbor < n.G || !openSet.Contains(n)) {
                        n.G = costToNeighbor;
                        n.H = DistanceFromTo(n, pEnd);
            
                        n.Parent = current;
            
                        if (!openSet.Contains(n)) openSet.Add(n);
                        n.Tile.HexRenderer.color = new Color(1, 1, 0);
                    }
                }
            }

            // Failed to find a path
            return new List<HexPathNode>();
        }

        public List<HexPathNode> GetAdjacentPathNodes(HexPathNode pOrigin) {
            List<HexPathNode> neighbors = new List<HexPathNode>();

            foreach (Vector2Int i in NeighborIndices) {
                if (!HexMap.Instance.Tiles.ContainsKey(i + pOrigin.Address)) continue;
                
                neighbors.Add(HexMap.Instance.Tiles[i + pOrigin.Address].Data.PathNode);
            }

            return neighbors;
        }

        public int DistanceFromTo(HexPathNode a, HexPathNode b) {
            int dX = Mathf.Abs(a.Address.X - b.Address.X);
            int dY = Mathf.Abs(a.Address.Y - b.Address.Y);

            return 10 * (dX > dY ? dX : dY);
        }

        public Sprite GetDungeonSprite(TileType type) {
            foreach(HexSprite s in HexSpriteLibrary) {
                if(s.Type == TileType.DUNGEON) {
                    switch(type) {
                        case TileType.MOUNTAIN:
                        case TileType.TALL_MOUNTAIN:
                            return s.Sprites[0];
                        case TileType.DENSE_FOREST:
                        case TileType.FOREST:
                        case TileType.SPARSE_FOREST:
                            return s.Sprites[1];
                        default:
                            return s.Sprites[2];
                    }
                }
            }

            return null;
        }

        public string TileTypeToString(TileType type) {
            switch(type) {
                default:
                case TileType.NULL:
                    return "Null";
                case TileType.TALL_MOUNTAIN:
                    return "Tall Mountain";
                case TileType.MOUNTAIN:
                    return "Mountain";
                case TileType.FOREST:
                case TileType.SPARSE_FOREST:
                case TileType.DENSE_FOREST:
                    return "Forest";
                case TileType.GRASSLAND:
                    return "Grassland";
                case TileType.HIGHLAND:
                    return "Highland";
                case TileType.WATER:
                    return "Water";
                case TileType.OCEAN:
                    return "Ocean";
                case TileType.ISLAND:
                case TileType.OCEAN_ISLAND:
                    return "Island";
                case TileType.VILLAGE:
                    return "Village";
                case TileType.FARMLAND:
                    return "Farmland";
                case TileType.DUNGEON:
                    return "Dungeon";
                case TileType.STRONGHOLD:
                    return "Stronghold";
            }
        }

        public int GetRoughTerrainFactor(TileType type) {
            switch (type) {
                default:
                case TileType.NULL:
                case TileType.ISLAND:
                case TileType.OCEAN_ISLAND:
                case TileType.VILLAGE:
                case TileType.FARMLAND:
                case TileType.STRONGHOLD:
                case TileType.GRASSLAND:
                    return 1;
                case TileType.FOREST:
                case TileType.SPARSE_FOREST:
                case TileType.DENSE_FOREST:
                case TileType.HIGHLAND:
                case TileType.DUNGEON:
                    return 2;
                case TileType.TALL_MOUNTAIN:
                case TileType.MOUNTAIN:
                    return 3;
                case TileType.WATER:
                    return 4;
                case TileType.OCEAN:
                    return 5;
            }
        }

        public string GetTilePrefix(TileType type) {
            switch(type) {
                default:
                    return "An Unremarkable ";
            }
        }
    }
}