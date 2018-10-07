using System.Collections.Generic;

namespace Dungeon.Graphing {
    // An edge node is a special node which occurs between two regular nodes.
    public class EdgeNode : Node {

        // An edge node has exactly one child, and inherits exactly one parent (from Node.cs).
        public RoomNode Child { get; set; }

        // Create an edge node with data as params
        public EdgeNode(RoomNode pParent, RoomNode pChild, params KeyValuePair<Data, object>[] pData) : base (pParent, pData) { Child = pChild; }

        // Create a new room node with data as dictionary.
        public EdgeNode(RoomNode pParent, RoomNode pChild, Dictionary<Data, object> pData) : base(pParent, pData) { Child = pChild; }
    }
}
