using System.Collections.Generic;

namespace Dungeon.Graphing {
    public class Node {
        // This node's neighbors.
        public RoomNode Parent { get; set; }

        // This node's data.
        public Dictionary<Data, object> Data { get; internal set; }

        // Indexer checks for null keys.
        public object this[Data d] {
            get { return Data.ContainsKey(d) ? Data[d] : null; }
            private set { Data[d] = value; }
        }

        // Optional gameobject to associate with this node.
        public UnityEngine.GameObject Object { get; set; }

        // Is this node a type which blocks progression between nodes?
        public bool IsBlocker { get { return NodeData.Blockers.Contains((NodeType)this[Graphing.Data.Type]); } }

        // Is this node valid as a parent?
        public bool IsValidParent { get { return NodeData.ValidParentTypes.Contains((NodeType)this[Graphing.Data.Type]); } }

        // Get this node as a room node (dirty cast)
        public RoomNode ToRoomNode { get { return new RoomNode(Parent, Data); } }

        // Get this node as an edge node (dirty cast)
        public EdgeNode ToEdgeNode { get { return new EdgeNode(Parent, null, Data); } }

        // Get whether the node has data of type
        public bool HasData(Data pDataKey) { return Data.ContainsKey(pDataKey); }

        // The depth of this node within a tree
        public int Depth(int d = 0) {
            return Parent != null ? Parent.Depth(d + 1) : d;
        }

        // Create a node with data as a lsit of key value pairs
        public Node(RoomNode pParent, params KeyValuePair<Data, object>[] pData) {
            Parent = pParent;

            Data = new Dictionary<Data, object>();
            foreach (KeyValuePair<Data, object> d in pData) { SetData(d.Key, d.Value); }
        }

        // Create a node with data as a dictionary
        public Node(RoomNode pParent, Dictionary<Data, object> pData) {
            Parent = pParent;

            Data = pData;
        }
        
        // Add or set node data.
        public void SetData(Data pDataType, object pData) {
            if(Data.ContainsKey(pDataType))
                this[pDataType] = pData;
            else
                Data.Add(pDataType, pData);
        }

        // Return a string containing all of this node's information.
        // This is mostly for debugging.
        public override string ToString() {
            string s = "";
            foreach (KeyValuePair<Data, object> d in Data) {
                s += string.Format("{0}: {1} ", d.Key, d.Value);
            }
            return s;
        }
    }
}
