using System.Collections.Generic;

namespace Dungeon.Graphing {
    // Enum of data types that a node can carry
    public enum Data {
        Type,
        ID,
        Switches,
    }

    // Types of nodes
    public enum NodeType {
        Entrance,
        Boss,
        Miniboss,
        Door,
        Key,
        BigKey,
        BigDoor,
        Fill,
        Bonus,
        Switch,
        Barrier,
    }

    public class NodeData {
        // The maximum number of children a node can have before it can no longer accept more.
        public const int MAXIMUM_CHILDREN = 4;

        // Small convenience override for a key value pair
        public static KeyValuePair<Data, object> Item(Data data, object obj) { return new KeyValuePair<Data, object>(data, obj); }

        // List of node types that can be parents.
        public static List<NodeType> ValidParentTypes = new List<NodeType>() { NodeType.Entrance, NodeType.Fill, NodeType.Key, NodeType.Switch, };

        // List of node types that impede traversal from one node to another.
        public static List<NodeType> Blockers = new List<NodeType>() { NodeType.Door, NodeType.BigDoor, NodeType.Barrier, };
    }
}
