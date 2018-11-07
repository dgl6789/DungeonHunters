using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Numeralien.Utilities;
using System.Linq;
using App.UI;
using App;

namespace Overworld {
    /// <summary>
    /// HexMap defines an object of a map in which the game takes place.
    /// Its main functionality revolves around saving, loading, and generating maps.
    /// </summary>
    public class HexMap : MonoBehaviour {

        public static HexMap Instance;

        [HideInInspector]
        public Dictionary<HexAddress, HexTile> Tiles = new Dictionary<HexAddress, HexTile>();

        public GameObject TilePrefab;

        [Header("Basic Generation Parameters")]

        [SerializeField] public int GenerationSteps = 2;

        [SerializeField] public float MapSizeTarget;
        [SerializeField] public int GenerationRetryMax;

        public ushort Width;
        public ushort Height;

        [SerializeField] ushort meanInitialTileCount;
        [SerializeField] ushort stdDevInitialTileCount;
        [SerializeField] float rangeGapChance;
        [SerializeField] float tallMountainChance;

        public float neighborOffsetStdDev;

        [SerializeField] public int Seed;

        [Header("Coastline Generation Parameters")]

        public int coastShapeMin;
        public int coastShapeMax;

        public float coastPerlinScale;
        public float coastPerlinFineScale;

        public int coastSmoothingSteps;
        [SerializeField] public float MapCutoff;

        [Header("Heightmap Generation Parameters")]

        public float heightPerlinScale;
        public float heightPerlinFineScale;

        public int heightSmoothingSteps;

        public float HighlandDenominator;

        [Header("Vegetation Generation Parameters")]

        public float vegetationPerlinScale;
        public float vegetationPerlinFineScale;

        public int vegetationSmoothingSteps;

        public float VegetationDenseCutoff;
        public float VegetationNormalCutoff;
        public float VegetationSparseCutoff;

        [Header("Ocean Generation Parameters")]

        public int OceanPadding;

        public float CoastSize;
        public float IslandRate;

        [Header("Civilization Generation Parameters")]

        public float villageRate;

        public float dungeonRate;
        public float dungeonCountMean;
        public float dungeonCountStdDev;

        public float farmlandRate;

        public float birdSpawnRate;

        [Header("Dev Tools")]
        [SerializeField] bool randomSeed = true;
        [SerializeField] bool deobfuscateOnGeneration;

        [SerializeField] Image GenerationProgressBar;
        [SerializeField] Text GenerationProgressText;

        // Data Maps
        DataMap islandMap;
        DataMap heightMap;
        DataMap vegetationMap;
        DataMap oceanMap;

        /// <summary>
        /// Init singleton and seed random
        /// </summary>
        void Awake() {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            if (!randomSeed)
                Random.InitState(Seed);
        }

        /// <summary>
        /// Regenerate when G is pressed
        /// </summary>
        void Update() {
            if (Input.GetKeyDown(KeyCode.G)) {

                StartCoroutine("Generate");
            }
        }

        public IEnumerator Generate() {
            yield return StartCoroutine(UpdateProgress(0f, "Seeding..."));

            int tries = 0;

            do {
                Reseed();

                yield return StartCoroutine(UpdateProgress(0.05f, "Cleaning Old Data..."));
                tries++;
                // Clear the existing tiles
                Tiles.Clear();
                foreach (Transform t in transform.GetComponentInChildren<Transform>()) {
                    if (t != transform) Destroy(t.gameObject);
                }
                yield return StartCoroutine(UpdateProgress(0.1f, "Creating Mountain Ranges..."));
                // Generate a number of mountain ranges from which to propagate the rest of the landmass
                // Get the number of ranges to generate
                int numToGenerate = Mathf.RoundToInt(Math.RandomNormal(meanInitialTileCount, stdDevInitialTileCount, Seed, 2, 5));

                List<HexAddress> used = new List<HexAddress>();
                List<HexTile> initialHexes = new List<HexTile>();

                List<HexTile> currentRange = new List<HexTile>();

                islandMap = new DataMap(Width, Height);

                for (int i = 0; i < numToGenerate; i++) {
                    // Reset the current range.
                    currentRange.Clear();

                    // Generate two mountains as the start and end of the range.
                    for (int j = 0; j < 2; j++) {
                        HexAddress a = new HexAddress(Random.Range(Width / 4, Width - (Width / 4)), Random.Range(Height / 4, Height - (Height / 4)));

                        do {
                            a = new HexAddress(Random.Range(Width / 4, Width - (Width / 4)), Random.Range(Height / 4, Height - (Height / 4)));
                        } while (used.Contains(a));

                        HexTile h = GenerateTile(a, TileType.TALL_MOUNTAIN);

                        used.Add(a);
                        initialHexes.Add(h);
                        currentRange.Add(h);
                    }

                    HexAddress currentAddress = currentRange[0].Address;
                    HexAddress targetAddress = currentRange[1].Address;

                    islandMap.FocalPoints.Add(currentAddress);
                    islandMap.FocalPoints.Add(targetAddress);

                    // While the range is not complete...
                    while (currentAddress != targetAddress) {
                        // Get the desired direction toward the address of currentRange[1].
                        Vector2Int d = new Vector2Int();

                        // If the end tile is above this one, add 1 to y, if it is below subtract 1 from y, else do nothing.
                        if (targetAddress.X > currentAddress.X) d.x++;
                        else if (targetAddress.X < currentAddress.X) d.x--;

                        // If the end tile is to the right of this one, add 1 to x, if it is to the left subrtact 1, else do nothing.
                        if (targetAddress.Y > currentAddress.Y) d.y++;
                        else if (targetAddress.Y < currentAddress.Y) d.y--;

                        // Make sure we stay adjacent to the current address
                        if (d.y == d.x && d.x + d.y != 0) {
                            if (Random.Range(0f, 1.0f) >= 0.5f) d.y = 0;
                            else d.x = 0;
                        }

                        // Set currentAddress to the adjacent tile in the desired direction, plus a random factor.
                        for (int v = 0; v < 6; v++) {
                            if (HexFunctions.Instance.NeighborIndices[v].Equals(d)) {
                                int index = (v + Random.Range(-1, 2));
                                index = (index % 6 + 6) % 6;

                                d = HexFunctions.Instance.NeighborIndices[index];
                                break;
                            }
                        }

                        // The random factor can slightly alter the direction.
                        currentAddress += new HexAddress(d.x, d.y);

                        if (currentAddress.Equals(targetAddress)) break;

                        if (Tiles.ContainsKey(currentAddress)) continue;

                        // Generate a tall mountain or regular mountain tile at currentAddress.
                        HexTile h = GenerateTile(
                            currentAddress,
                            Random.Range(0f, 1.0f) < tallMountainChance ? TileType.TALL_MOUNTAIN : TileType.MOUNTAIN
                        );

                        used.Add(currentAddress);
                        initialHexes.Add(h);

                        // Add the generated tile to currentRange.
                        currentRange.Add(h);
                        if (used.Count % 3 == 0) islandMap.FocalPoints.Add(h.Address);
                    }

                    // Have a chance to delete one of the tiles in the range.
                    if (Random.Range(0f, 1f) <= rangeGapChance) {
                        int index = Random.Range(0, currentRange.Count);
                        Destroy(Tiles[currentRange[index].Address].gameObject);
                        Tiles.Remove(currentRange[index].Address);
                    }
                }

                yield return StartCoroutine(UpdateProgress(0.3f, "Generating Coastline..."));

                // Generate the coastline
                islandMap.GenerateCoastMap();

                yield return StartCoroutine(UpdateProgress(0.5f, "Blocking Out Island..."));
                // Fill out the coast with grassland to start
                foreach (KeyValuePair<HexAddress, float> kvp in islandMap.Data) {
                    if (islandMap.Data[kvp.Key] >= MapCutoff && !Tiles.ContainsKey(kvp.Key)) {
                        GenerateTile(kvp.Key, TileType.GRASSLAND);
                    }
                }

            } while (tries < GenerationRetryMax && ((float)Tiles.Count / (float)(Width * Height)) < MapSizeTarget);

            // iterate over the tiles, choosing a new type based on the surroundings of each
            // At this point in generation, Tiles contains all the tiles the island will have.

            // Generate forests.
            yield return StartCoroutine(UpdateProgress(0.6f, "Generating Forests..."));

            vegetationMap = new DataMap(Width, Height);
            vegetationMap.GenerateVegetationMap();

            foreach (KeyValuePair<HexAddress, float> kvp in vegetationMap.Data) {
                if(Tiles.ContainsKey(kvp.Key) && (Tiles[kvp.Key].Type != TileType.MOUNTAIN && Tiles[kvp.Key].Type != TileType.TALL_MOUNTAIN)) {
                    if (vegetationMap.Data[kvp.Key] >= VegetationDenseCutoff) Tiles[kvp.Key].Type = TileType.DENSE_FOREST;
                    else if (vegetationMap.Data[kvp.Key] >= VegetationNormalCutoff) Tiles[kvp.Key].Type = TileType.FOREST;
                    else if(vegetationMap.Data[kvp.Key] >= VegetationSparseCutoff) Tiles[kvp.Key].Type = TileType.SPARSE_FOREST;
                }
            }

            // Generate forested areas.
            yield return StartCoroutine(UpdateProgress(0.7f, "Generating Highlands..."));

            foreach (KeyValuePair<HexAddress, HexTile> kvp in Tiles) {
                int n = HexFunctions.Instance.AdjacentTilesOfTypes(kvp.Value.Address, TileType.MOUNTAIN, TileType.TALL_MOUNTAIN, TileType.HIGHLAND);
                if ((n > 0)) {
                    if (Random.Range(0f, 1.0f) < n / HighlandDenominator && HexFunctions.Instance.IsViableForFarm(kvp.Key)) Tiles[kvp.Key].Type = TileType.HIGHLAND;
                }
            }

            // Generate oceans, lakes, and small islands thereupon.
            yield return StartCoroutine(UpdateProgress(0.8f, "Generating Ocean..."));

            oceanMap = new DataMap(Width, Height);
            oceanMap.GenerateOceanMap();

            foreach (KeyValuePair<HexAddress, float> kvp in oceanMap.Data) {
                // Debug.Log(kvp.Value);
                if (!Tiles.ContainsKey(kvp.Key)) {
                    if (kvp.Value == 1.0f) {
                        if(Random.Range(0f, 1f) < IslandRate)
                            GenerateTile(kvp.Key, TileType.OCEAN_ISLAND);
                        else
                            GenerateTile(kvp.Key, TileType.OCEAN);
                    }
                }
            }

            // Generate special areas where people live.
            yield return StartCoroutine(UpdateProgress(0.85f, "Generating Civilization..."));

            // Spawn the stronghold with some farms
            foreach (KeyValuePair<HexAddress, HexTile> kvp in Tiles) {
                if (HexFunctions.Instance.IsViableForFarm(kvp.Value.Address)) {
                    // Spawn the stronghold, farms, and break
                    kvp.Value.Type = TileType.STRONGHOLD;
                    
                    foreach(MercenaryData merc in MercenaryController.Instance.Mercenaries) {
                        merc.SetLocation(kvp.Value);
                    }

                    MercenaryController.Instance.UpdateLocationPins();

                    foreach (Vector2Int n in HexFunctions.Instance.NeighborIndices) {
                        if(farmlandRate > Random.Range(0f, 1f) && Tiles.ContainsKey(n + kvp.Value.Address) && HexFunctions.Instance.IsViableForFarm(n + kvp.Value.Address)) {
                            Tiles[n + kvp.Value.Address].Type = TileType.FARMLAND;
                        }
                    }
                    break;
                }
            }

            // Spawn some villages with farms
            foreach (KeyValuePair<HexAddress, HexTile> kvp in Tiles) {
                if (Random.Range(0f, 1f) < villageRate && HexFunctions.Instance.IsViableForFarm(kvp.Value.Address)) {
                    // Spawn the stronghold and break
                    kvp.Value.Type = TileType.VILLAGE;

                    foreach (Vector2Int n in HexFunctions.Instance.NeighborIndices) {
                        if (farmlandRate > Random.Range(0f, 1f) && Tiles.ContainsKey(n + kvp.Value.Address) && HexFunctions.Instance.IsViableForFarm(n + kvp.Value.Address)) {
                            Tiles[n + kvp.Value.Address].Type = TileType.FARMLAND;
                        }
                    }
                    break;
                }
            }

            // Generate the dungeons
            int numDungeons = Mathf.RoundToInt(Math.RandomHalfNormal(dungeonCountMean, dungeonCountStdDev, Seed));

            while (numDungeons > 0) {
                foreach (KeyValuePair<HexAddress, HexTile> kvp in Tiles) {
                    if (Random.Range(0f, 1f) < dungeonRate && HexFunctions.Instance.IsLand(kvp.Value)) {
                        // Spawn the dungeon
                        Sprite s = HexFunctions.Instance.GetDungeonSprite(kvp.Value.Type);
                        kvp.Value.Type = TileType.DUNGEON;
                        kvp.Value.HexRenderer.sprite = s;
                        numDungeons--;
                        
                    }
                }
            }


            // Generate the hex data layer
            foreach (KeyValuePair<HexAddress, HexTile> kvp in Tiles)
            {
                Sprite s = null;
                foreach(HexSprite h in HexFunctions.Instance.HexSpriteLibrary)
                {
                    if(h.Type == kvp.Value.Type) {
                        s = h.SketchedSprite;
                        break;
                    }
                }

                kvp.Value.Data = new HexData(s, HexFunctions.Instance.TileTypeToString(kvp.Value.Type), new Resource[0]);
                kvp.Value.Data.PathNode = new HexPathNode(kvp.Value);
            }

            yield return StartCoroutine(UpdateProgress(0.9f, "Prettyifying..."));

            // Additional sprite picking/deobfuscation in special cases.
            foreach (KeyValuePair<HexAddress, HexTile> kvp in Tiles) {
                Sprite s = null;
                switch (kvp.Value.Type) {
                    case TileType.STRONGHOLD:
                        Tiles[kvp.Key].SetTileEffectState(2, true);
                        kvp.Value.HexRenderer.sortingOrder = 1;

                        DeobfuscateRadius(3, kvp.Value);
                        TileSelector.Instance.SetTarget(kvp.Value);
                        kvp.Value.SetTileEffectState(4, true);
                        break;
                    case TileType.VILLAGE:
                        kvp.Value.SetTileEffectState(2, true);
                        break;
                    case TileType.DUNGEON:
                        // BlightRadius(2, kvp.Value);
                        break;
                    case TileType.OCEAN:
                        kvp.Value.SetTileEffectState(1, false);

                        // Change ocean tiles adjacent to land into water tiles.
                        List<HexAddress> toWater = new List<HexAddress>();

                        if (HexFunctions.Instance.AdjacentLandTiles(kvp.Key) > 0) { toWater.Add(kvp.Key); }

                        foreach(HexAddress a in toWater) {
                            Tiles[a].Type = Random.Range(0f, 1.0f) < IslandRate ? TileType.ISLAND : TileType.WATER;
                            if (Tiles[a].Type == TileType.ISLAND) Tiles[a].SetTileEffectState(4, true);
                            else if (Random.Range(0f, 1f) < 0.075f) kvp.Value.SetTileEffectState(6, true);
                        }

                        if (kvp.Value.Type == TileType.OCEAN) if (Random.Range(0f, 1f) < 0.075f) kvp.Value.SetTileEffectState(6, true);
                        break;

                    case TileType.OCEAN_ISLAND:
                        if (HexFunctions.Instance.AdjacentLandTiles(kvp.Key) > 0) { kvp.Value.Type = TileType.ISLAND; kvp.Value.SetTileEffectState(4, true); }
                        break;

                    case TileType.MOUNTAIN:
                        if (HexFunctions.Instance.AdjacentTilesOfTypes(kvp.Key, TileType.MOUNTAIN, TileType.TALL_MOUNTAIN) < 4) {
                            int sp = GetMountainSprite(kvp.Key);

                            if (HexFunctions.Instance.AdjacentTilesOfTypes(kvp.Key, TileType.FOREST, TileType.SPARSE_FOREST, TileType.DENSE_FOREST) > 1) {
                                if (sp != -1)
                                    s = HexFunctions.Instance.AlternateSpriteLibrary[1].Sprites[sp];
                            } else {
                                if (sp != -1)
                                    s = HexFunctions.Instance.AlternateSpriteLibrary[0].Sprites[sp];
                            }
                        } else if (HexFunctions.Instance.AdjacentLandTiles(kvp.Key) < 6) {
                            s = HexFunctions.Instance.AlternateSpriteLibrary[2].GetSprite();
                        }

                        kvp.Value.HexRenderer.enabled = true;

                        break;
                    case TileType.TALL_MOUNTAIN:
                        if (HexFunctions.Instance.AdjacentLandTiles(kvp.Key) < 6) {
                            s = HexFunctions.Instance.AlternateSpriteLibrary[3].GetSprite();
                        }

                        kvp.Value.SetTileEffectState(3, true);
                        kvp.Value.SetTileEffectState(1, false);
                        break;
                    case TileType.NULL:
                        kvp.Value.SetTileEffectState(1, false);
                        break;
                }

                if (deobfuscateOnGeneration) kvp.Value.SetTileEffectState(1, false);

                if(s) kvp.Value.HexRenderer.sprite = s;

                if (Random.Range(0f, 1f) < birdSpawnRate) kvp.Value.SetTileEffectState(4, true);
            }

            yield return StartCoroutine(UpdateProgress(1.0f, "Done."));

            GetComponent<MapScrollEffect>().GetTileList();

            // Test the pathfinding
            // List<HexTile> ts = Enumerable.ToList(Tiles.Values);
            // 
            // foreach(HexPathNode node in HexFunctions.Instance.GetPathFromTo(ts[Random.Range(0, ts.Count)].Data.PathNode, ts[Random.Range(0, ts.Count)].Data.PathNode)) {
            //     node.Tile.HexRenderer.color = new Color(1, 0, 0);
            // }

            yield return null;
        }

        int GetMountainSprite(HexAddress a) {
            HexAddress w = new HexAddress(a.X - 1, a.Y);
            HexAddress e = new HexAddress(a.X + 1, a.Y);
            HexAddress sw = new HexAddress(a.X - 1, a.Y - 1);
            HexAddress se = new HexAddress(a.X, a.Y + 1);

            bool wM = (Tiles.ContainsKey(w) && (Tiles[w].Type == TileType.MOUNTAIN || Tiles[w].Type == TileType.TALL_MOUNTAIN));
            bool eM = (Tiles.ContainsKey(e) && (Tiles[e].Type == TileType.MOUNTAIN || Tiles[e].Type == TileType.TALL_MOUNTAIN));
            bool swM = (Tiles.ContainsKey(sw) && (Tiles[sw].Type == TileType.MOUNTAIN || Tiles[sw].Type == TileType.TALL_MOUNTAIN));
            bool seM = (Tiles.ContainsKey(se) && (Tiles[se].Type == TileType.MOUNTAIN || Tiles[se].Type == TileType.TALL_MOUNTAIN));

            // If southwest + west are not mountain, make it the left texture.
            if (!swM && !wM) { return 0; }
            
            // If southeast + east are not mountain, make it the right texture.
            else if (!seM && !eM) { return 2; }
            
            // If southwest or southeast are not mountain, make it the middle texture
            else if (wM || eM) { return 1; }
            
            // Otherwise keep the current texture.
            else return -1;
        }

        /// <summary>
        /// Generate a tile with data.
        /// </summary>
        /// <param name="pAddress">The HexAddress at which to index this tile.</param>
        /// <param name="pType">The type of tile to generate.</param>
        /// <returns>The object of the tile that was generated.</returns>
        HexTile GenerateTile(HexAddress pAddress, TileType pType = TileType.NULL)
        {
            HexTile h = Instantiate(TilePrefab, this.transform).GetComponent<HexTile>();

            h.Address = pAddress;
            h.Type = pType;
            
            Tiles.Add(pAddress, h);

            h.DisplayPosition = pAddress.ToUnity;

            return h;
        }

        /// <summary>
        /// Provide a new seed for the random number generator.
        /// </summary>
        void Reseed(bool random = true) {
            int seed;

            if (random)
                seed = Random.Range(short.MinValue, short.MaxValue);
            else
                seed = Seed;

            Random.InitState(seed);

            Seed = seed;
        }

        /// <summary>
        /// Update an optional progress bar that shows during generation.
        /// </summary>
        /// <param name="pAmount">New amount the progress bar should show.</param>
        /// <param name="pMessage">The message that should be shown with the bar.</param>
        public IEnumerator UpdateProgress(float pAmount, string pMessage) {
            if (GenerationProgressBar && GenerationProgressText) {
                GenerationProgressText.text = pMessage;
                GenerationProgressBar.fillAmount = Mathf.Clamp01(pAmount);
            }

            yield return null;
        }

        void DeobfuscateRadius(int radius, HexTile origin) {
            foreach(HexTile tile in Tiles.Values.ToList()) {
                if(Vector2.Distance(tile.DisplayPosition, origin.DisplayPosition) < radius) {
                    tile.SetTileEffectState(1, false);
                }
            }
        }

        void BlightRadius(int radius, HexTile origin) {
            foreach (HexTile tile in Tiles.Values.ToList()) {
                float d = Vector2.Distance(tile.DisplayPosition, origin.DisplayPosition);

                if (d < radius) {
                    float b = Math.Map(d, 0, radius, 0.5f, 0f);

                    tile.BlightLevel = b;
                }
            }
        }
    }
}