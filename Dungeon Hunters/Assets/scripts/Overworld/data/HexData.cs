using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld {
    public class HexData {
        public Sprite DrawnSprite;
        public string Name;
        public string Details;
        
        public Resource[] resources;

        public HexData(Sprite sketch, string name, Resource[] resources) {
            this.resources = resources;

            DrawnSprite = sketch;
            Name = name;
            Details = "Nothing yet.";
        }

        public void SetDetails(string toAdd) {
            if (!Details.Equals("Nothing yet."))
                Details += "\n\n" + toAdd;
            else
                Details = toAdd;
        }
    }
}
