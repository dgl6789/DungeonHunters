using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;

namespace App {
    public enum MercenarySkills { Constitution, Endurance, Stealth, Medicine, Athletics, Knowledge, Smithing, Magic, Survival, Insight, Coercion, Deception, Intimidation, Performance, Spot }

    public class MercenaryData {

        private HexTile location;
        public HexTile Location {
            get { return location; }
            set { location = value; }
        }

        private HexTile destination;
        public HexTile Destination {
            get { return destination; }
            set { destination = value; }
        }

        private List<HexTile> CurrentPath;
        private int PathIndex;

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
    }
}
