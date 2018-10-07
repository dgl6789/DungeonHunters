using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeon.Graphing {
    public class Graph {
        // The number of gaussian parameters that a graph needs in order to generate.
        const int REQUIRED_PARAM_COUNT = 6;
        
        // The Root of the graph.
        public RoomNode Root { get; private set; }
        
        // The generation Seed
        public int Seed { get; set; }

        // Create a new graph. 
        public Graph(int pComplexity, GaussianParam[] pGaussianParams, int pSeed = 0) {
            Seed = pSeed;

            GenerateGraph(pComplexity, pGaussianParams);
        }

        // Generate the graph's nodes and their parent/child/edge relationships.
        public void GenerateGraph(int pComplexity, GaussianParam[] pGaussianParams) {
            ArrangeNodes(GenerateNodeList(pComplexity, pGaussianParams));

            // Hook in additional functionality here.
        }

        // Generate a list of all the nodes in the graph.
        public List<Node> GenerateNodeList(int pComplexity, GaussianParam[] pGaussianParams) {
            UnityEngine.Random.InitState(Seed);

            if(pGaussianParams.Length < REQUIRED_PARAM_COUNT) {
                UnityEngine.Debug.LogWarningFormat("Node generation failed! Required {0} gaussian params, but received {1}", REQUIRED_PARAM_COUNT, pGaussianParams.Length);
                return new List<Node>();
            }

            // Generate a list of all the nodes in this dungeon graph.
            List<Node> nodes = new List<Node>();

            // There is always one entrance, one big key, and one boss
            nodes.Add(new RoomNode(null, NodeData.Item(Data.Type, NodeType.Entrance)));
            nodes.Add(new RoomNode(null, NodeData.Item(Data.Type, NodeType.Boss)));
            nodes.Add(new RoomNode(null, NodeData.Item(Data.Type, NodeType.BigKey)));
            nodes.Add(new EdgeNode(null, null, NodeData.Item(Data.Type, NodeType.BigDoor)));

            // There are a number of keys. There is an equal number of doors.
            int numDoors = NumRooms(pGaussianParams, pComplexity, 0);
            for (int i = 0; i < numDoors; i++) {
                nodes.Add(new EdgeNode(null, null, NodeData.Item(Data.Type, NodeType.Door)));
                nodes.Add(new RoomNode(null, NodeData.Item(Data.Type, NodeType.Key)));
            }
            
            // There are a number of switches, each group of which corresponds to one barrier.
            int numSwitchGroups = NumRooms(pGaussianParams, pComplexity, 1);
            for (int i = 0; i < numSwitchGroups; i++) {
                // Add the barrier that corresponds with the switches.
                char id = (char)(i % 25 + 65);

                Node nBarrier = new EdgeNode(null, null, NodeData.Item(Data.Type, NodeType.Barrier), NodeData.Item(Data.ID, id), NodeData.Item(Data.Switches, new List<Node>()));
                List<Node> switchList = new List<Node>();

                nodes.Add(nBarrier);
                
                // Add a number of switches that correspond to the barrier
                int numSwitches = UnityEngine.Mathf.Clamp(NumRooms(pGaussianParams, pComplexity, 2), 2, int.MaxValue);
                for (int j = 0; j < numSwitches; j++) {
                    Node nSwitch = new RoomNode(null, NodeData.Item(Data.Type, NodeType.Switch), NodeData.Item(Data.ID, id));

                    nodes.Add(nSwitch);
                    switchList.Add(nSwitch);
                }

                // Set the barrier's switch list
                nBarrier.SetData(Data.Switches, new List<Node>(switchList));
            }
            
            // There are a number of filler rooms.
            int numFillerRooms = NumRooms(pGaussianParams, pComplexity, 3);
            for (int i = 0; i < numFillerRooms; i++) {
                nodes.Add(new Node(null, NodeData.Item(Data.Type, NodeType.Fill)));
            }

            // There are a small number of minibosses.
            int numBonusRooms = NumRooms(pGaussianParams, pComplexity, 4);
            for (int i = 0; i < numBonusRooms; i++) {
                nodes.Add(new Node(null, NodeData.Item(Data.Type, NodeType.Bonus)));
            }

            // There are a small number of bonus rooms.
            int numMinibosses = NumRooms(pGaussianParams, pComplexity, 5);
            for (int i = 0; i < numMinibosses; i++) {
                nodes.Add(new Node(null, NodeData.Item(Data.Type, NodeType.Miniboss)));
            }
            
            return nodes;
        }

        private int NumRooms(GaussianParam[] gParams, int pComplexity, int gaussianIndex) {
            return (int)Math.RandomNormal(gParams[gaussianIndex].Mean, gParams[gaussianIndex].StandardDeviation, Seed);
        }

        // Set the parent/child relationships of each node in the given list.
        public void ArrangeNodes(List<Node> pNodeList) {
            UnityEngine.Random.InitState(Seed);

            Root = pNodeList[0].ToRoomNode;
            RoomNode bossNode = pNodeList[1].ToRoomNode;
            RoomNode bigKeyNode = pNodeList[2].ToRoomNode;
            EdgeNode bigDoorNode = pNodeList[3].ToEdgeNode;

            List<RoomNode> parents = new List<RoomNode>();
            List<RoomNode> rooms = new List<RoomNode>();
            List<EdgeNode> edges = new List<EdgeNode>();

            // Remove the saved nodes.
            pNodeList.RemoveRange(0, 4);
            parents.Add(Root);

            // Organize the remaining nodes by behavior
            foreach(Node n in pNodeList) {
                if(n.IsBlocker)
                    edges.Add(n.ToEdgeNode);
                else
                    rooms.Add(n.ToRoomNode);
            }

            // For the rooms, assign them as parents if necessary and set each node's parent.
            foreach(RoomNode n in rooms) {
                parents[UnityEngine.Random.Range(0, parents.Count)].AddChild(n);

                if(n.IsValidParent) {
                    parents.Add(n);
                }
            }

            // Assign the remaining special room/edge nodes.
            parents[UnityEngine.Random.Range(0, parents.Count)].AddChild(bigKeyNode);
            parents[parents.Count - 1].AddChild(bossNode);
            parents[parents.Count - 1].AddEdgeNode(bossNode, bigDoorNode);

            // For the edges, assign their children and parents in a valid way
            foreach (EdgeNode n in edges) {
                List<RoomNode> children = new List<RoomNode>();
                List<RoomNode> validParents = new List<RoomNode>();
                int parentIndex = -1;
                
                foreach (RoomNode p in parents) {
                    if (ParentIsValidForBlocker(p, n)) validParents.Add(p);
                }

                parentIndex = UnityEngine.Random.Range(0, validParents.Count - 1);

               
                if (validParents.Count > 0) {
                    for (int i = 0; i < validParents[parentIndex].Children.Count; i++) {
                        if (validParents[parentIndex].Edges[i] == null) children.Add(validParents[parentIndex].Children[i]);
                    }
                }

                validParents[parentIndex].AddEdgeNode(children[UnityEngine.Random.Range(0, children.Count)], n);
            }
        }

        private bool ParentIsValidForBlocker(RoomNode pNode, EdgeNode pBlocker) {
            if (pNode.Children.Count == 0) return false;

            List<RoomNode> ancestors = AncestorsOf(Root, pNode);

            switch((NodeType)pBlocker[Data.Type]) {
                 case NodeType.Barrier:
                    int numSwitches = 0;
                    foreach(RoomNode n in ancestors) {
                        numSwitches += (NodeType)n[Data.Type] == NodeType.Switch && (char)n[Data.ID] == (char)pBlocker[Data.ID] ? 1 : 0;
                    }

                    return numSwitches == ((List<Node>)pBlocker[Data.Switches]).Count;

                case NodeType.Door:
                    int numKeys = 0;
                    foreach (RoomNode n in ancestors) {
                        numKeys += (NodeType)n[Data.Type] == NodeType.Key ? 1 : 0;
                    }

                    return numKeys > NumOfTypeInGraph(NodeType.Door, Root);
            }

            return false;
        }

        // Return a list of the ancestors of a given node.
        private List<RoomNode> AncestorsOf(RoomNode pNode, RoomNode pTarget) {
            List<RoomNode> descendants = DescendantsOf(pTarget);
            List<RoomNode> allNodes = DescendantsOf(Root);
            List<RoomNode> ascendants = new List<RoomNode>();

            foreach(RoomNode n in allNodes) {
                if (!descendants.Contains(n)) {
                    ascendants.Add(n);
                }
            }

            return ascendants;
        }

        private List<RoomNode> DescendantsOf(RoomNode pNode) {

            List<RoomNode> nodes = new List<RoomNode>();
            nodes.Add(pNode);

            foreach(RoomNode n in pNode.Children) {
                nodes.AddRange(DescendantsOf(n));
            }

            return nodes;
        }

        public int Count { get { return DescendantsOf(Root).Count; } }

        public int NumOfTypeInGraph(NodeType pType, RoomNode pNode) {
            foreach(RoomNode n in pNode.Children) {
                return NumOfTypeInGraph(pType, n) + (NodeType)pNode[Data.Type] == pType ? 1 : 0;
            }

            return (NodeType)pNode[Data.Type] == pType ? 1 : 0;
        }
    }
}
