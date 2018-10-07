using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld {

    public class DataMap {

        public ushort width;
        public ushort height;

        public List<HexAddress> FocalPoints;

        public Dictionary<HexAddress, float> Data;

        public DataMap(ushort width, ushort height) {
            FocalPoints = new List<HexAddress>();
            Data = new Dictionary<HexAddress, float>();

            this.width = width;
            this.height = height;
            
            GenerateBaseData();
        }

        void GenerateBaseData() {
            for (int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    Data.Add(new HexAddress(x, y), 0f);
                }
            }
        }

        public void GenerateCoastMap() {
            // First, generate circles at the focal points
            List<HexAddress> addresses = new List<HexAddress>(Data.Keys);

            for (int i = 0; i < 5; i++)
                FocalPoints.Add(new HexAddress(Random.Range(width / 4, width - (width / 4)), Random.Range(height / 4, height - (height / 4))));

            foreach (HexAddress a in FocalPoints) {
                int radius = Random.Range(HexMap.Instance.coastShapeMin, HexMap.Instance.coastShapeMax + 1);
                
                foreach (HexAddress k in addresses) {
                    float d = Mathf.Sqrt(Mathf.Pow(k.X - a.X, 2) + Mathf.Pow(k.Y - a.Y, 2));

                    if (d < radius) {
                        Data[k] = 1.0f;
                    }
                }
            }
            
            // Next, generate some noise
            foreach (HexAddress a in addresses) {
                // Get a noise value for the current address.
                float x = ((float)a.X / (float)width) * HexMap.Instance.coastPerlinScale;
                float y = ((float)a.Y / (float)height) * HexMap.Instance.coastPerlinScale;

                float x2 = ((float)a.X / (float)width) * (HexMap.Instance.coastPerlinScale / 4);
                float y2 = ((float)a.Y / (float)height) * (HexMap.Instance.coastPerlinScale / 4);

                float sample1 = Mathf.PerlinNoise(HexMap.Instance.Seed + x, HexMap.Instance.Seed + y);
                float sample2 = Mathf.PerlinNoise(HexMap.Instance.Seed + x2, HexMap.Instance.Seed + y2);

                // Integrate the noise
                Data[a] += sample1;
                Data[a] -= sample2 * 0.3f;
                
            }

            // Then smooth...
            Smooth(HexMap.Instance.coastSmoothingSteps);
        }

        public void GenerateVegetationMap() {
            // First, get a list of all the tile addresses.
            List<HexAddress> addresses = new List<HexAddress>(Data.Keys);
            
            foreach (HexAddress a in addresses) {

                // Generate noise (additively).
                // Get a noise value for the current address.
                float x = ((float)a.X / (float)width) * HexMap.Instance.vegetationPerlinScale;
                float y = ((float)a.Y / (float)height) * HexMap.Instance.vegetationPerlinScale;
                float x2 = ((float)a.X / (float)width) * HexMap.Instance.vegetationPerlinFineScale;
                float y2 = ((float)a.Y / (float)height) * HexMap.Instance.vegetationPerlinFineScale;

                float sample1 = Mathf.PerlinNoise(HexMap.Instance.Seed + x, HexMap.Instance.Seed + y);
                float sample2 = Mathf.PerlinNoise(HexMap.Instance.Seed + x2, HexMap.Instance.Seed + y2);

                // Integrate the noise
                Data[a] += sample1;
                Data[a] += sample2;
            }

            // Smooth it, baby.
            Smooth(HexMap.Instance.vegetationSmoothingSteps);
        }

        public void GenerateOceanMap() {
            // Regenerate the base data. It needs to be BIGGER than the land maps.
            Data = new Dictionary<HexAddress, float>();

            for (int x = -HexMap.Instance.OceanPadding; x < width + HexMap.Instance.OceanPadding; x++) {
                for (int y = -HexMap.Instance.OceanPadding; y < height + HexMap.Instance.OceanPadding; y++) {
                    Data.Add(new HexAddress(x, y), 0f);
                }
            }

            List<HexAddress> addresses = new List<HexAddress>(Data.Keys);

            foreach (HexAddress a in addresses) {
                float d = Mathf.Sqrt(Mathf.Pow(width / 2 - a.X, 2) + Mathf.Pow(height / 2 - a.Y, 2));

                // It's inside the circle, and it is a land tile in the hexmap.
                if (HexMap.Instance.Tiles.ContainsKey(a)) { Data[a] = 0.5f; }

                // It's inside the circle, and isn't a land tile.
                else if (!HexMap.Instance.Tiles.ContainsKey(a) && d < HexMap.Instance.OceanPadding) Data[a] = 1.0f;

                // Otherwise, indicate that it should be an obscured area. (Out of bounds... for THIS island)
                else Data[a] = -1.0f;
            }
        }

        void Smooth(int pNumSteps) {
            List<HexAddress> addresses = new List<HexAddress>(Data.Keys);

            for (int i = 0; i < pNumSteps; i++) {
                foreach (HexAddress a in addresses) {
                    float avg = 0f;
                    int total = 0;

                    foreach (Vector2Int n in HexFunctions.Instance.NeighborIndices) {
                        avg = Data[a];
                        total = 1;
                        HexAddress c = a + n;

                        if (Data.ContainsKey(c)) {
                            avg += Data[c];
                            total++;
                        }
                    }

                    Data[a] = avg / total;
                }
            }
        }

        public override string ToString() {
            string s = "";

            foreach(KeyValuePair<HexAddress, float> kvp in Data) {
                s += kvp.Key + " " + kvp.Value + "\n";
            }

            return s;
        }
    }
}