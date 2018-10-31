using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;

namespace App {
    public class MercenaryData {

        private HexTile location;
        public HexTile Location {
            get { return location; }
        }

        private HexTile destination;
        public HexTile Destination {
            get { return destination; }
        }

        private List<HexTile> CurrentPath;
        private int PathIndex;

        private string name;
        public string Name { get { return name; } }

        private int mind, body, spirit;
        public int Mind { get { return mind; } }
        public int Body { get { return body; } }
        public int Spirit { get { return spirit; } }

        public MercenaryData(HexTile pLocation) {
            location = pLocation;
        }
    }
}
