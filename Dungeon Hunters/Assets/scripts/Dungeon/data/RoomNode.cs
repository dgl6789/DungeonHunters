using System.Collections.Generic;

namespace Dungeon.Graphing {
    public class RoomNode : Node {
        // The children of this node.
        public List<RoomNode> Children { get; private set; }
        
        // The edge nodes that appear between this node and a child of this node.
        public List<EdgeNode> Edges { get; private set; }

        // Can this node have more children?
        public bool CanAcceptChildren {  get { return Children.Count <= NodeData.MAXIMUM_CHILDREN; } }

        // Create a new room node with data as params.
        public RoomNode(RoomNode pParent, params KeyValuePair<Data, object>[] pData) : base (pParent, pData) {
            Children = new List<RoomNode>();
            Edges = new List<EdgeNode>();
        }

        // Create a new room node with data as dictionary.
        public RoomNode(RoomNode pParent, Dictionary<Data, object> pData) : base(pParent, pData) {
            Children = new List<RoomNode>();
            Edges = new List<EdgeNode>();
        }

        // Add a child to this node.
        // Returns whether the operation succeeded.
        public bool AddChild(RoomNode pChild) {
            if (Children.Contains(pChild) || !CanAcceptChildren)
                return false;

            Children.Add(pChild);
            pChild.Parent = this;
            Edges.Add(null);
            return true;
        }

        // Add an edge node between this node and a child of this node
        // Returns whether the operation succeeded.
        public bool AddEdgeNode(RoomNode pChild, EdgeNode pEdge) {
            if (!Children.Contains(pChild)) return false;

            Edges[Children.IndexOf(pChild)] = pEdge;
            pEdge.Parent = this;
            pEdge.Child = pChild;
            return true;
        }
    }
}
