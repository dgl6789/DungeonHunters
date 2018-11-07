using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;

namespace App {
    public enum MercenarySkills { Constitution, Endurance, Stealth, Medicine, Athletics, Knowledge, Smithing, Magic, Survival, Insight, Coercion, Deception, Intimidation, Performance, Spot }

    public class MercenaryData {

        public GameObject LocationMarker;

        private HexTile location;
        public HexTile Location {
            get { return location; }
            set { location = value; }
        }
        
        public List<HexTile> CurrentPath;
        private int PathIndex;
        private int PathTerrainCounter;

        private string name;
        public string Name { get { return name; } }

        private int mind, body, spirit, rank;
        public int Mind { get { return mind; } }
        public int Body { get { return body; } }
        public int Spirit { get { return spirit; } }
        public int Rank { get { return rank; } }

        public Sprite Portrait;

        public List<MercenarySkills> Skills;

        public MercenaryData(string pName, int pMind, int pBody, int pSpirit, List<MercenarySkills> pSkills, int pRank = 1) {
            name = pName;
            mind = pMind;
            body = pBody;
            spirit = pSpirit;
            rank = pRank;

            Skills = new List<MercenarySkills>();
            Skills.AddRange(pSkills);
        }

        public void SetPath(List<HexTile> pPath) {
            CurrentPath = pPath;
            PathIndex = 0;
            PathTerrainCounter = 0;
        }

        public void SetLocation(HexTile pTile) {
            location = pTile;
        }

        public void UpdateLocation() {
            if (CurrentPath != null) {
                PathTerrainCounter++;

                if (PathTerrainCounter == HexFunctions.Instance.GetRoughTerrainFactor(Location.Type)) {
                    PathIndex++;
                    PathTerrainCounter = 0;

                    Location = CurrentPath[PathIndex];

                    if (PathIndex == CurrentPath.Count - 1) {
                        CurrentPath = null;
                    }
                }
            }
        }
    }
}
