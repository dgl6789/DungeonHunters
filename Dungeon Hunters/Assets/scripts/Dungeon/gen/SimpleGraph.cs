using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dungeon.Graphing;

public class SimpleGraph : MonoBehaviour {
    [SerializeField]
    Sprite[] NodeSprites;

    [SerializeField]
    Sprite[] SwitchSprites;

    [SerializeField]
    Sprite[] BarrierSprites;

    [SerializeField]
    GameObject nodeImagePrefab;

    [SerializeField]
    GameObject nodeLinkerPrefab;

    [SerializeField]
    GaussianParam[] generationParams;

    [SerializeField]
    Vector2 displayOffset;

    [SerializeField]
    int seed;
    public int Seed { get { return seed; } }

    [SerializeField]
    bool randomSeed;

    [SerializeField]
    int complexity;
    public int Complexity { get { return complexity; } }

    Graph currentGraph;

    [SerializeField]
    Transform graphTransform;

    int layoutCounter;

    Vector2 avgPos;

    // Use this for initialization
    void Start () {
        Regenerate();
	}

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Regenerate();
        }
    }

    public void Regenerate()
    {
        ClearGraph();
        currentGraph = new Graph(Complexity, generationParams, randomSeed ? Random.Range(int.MinValue, int.MaxValue) : Seed);
        DisplayGraph();

        avgPos /= currentGraph.Count;
        Camera.main.transform.position = new Vector3(avgPos.x, avgPos.y, Camera.main.transform.position.z);
    }

    void ClearGraph()
    {
        foreach(Transform t in graphTransform.GetComponentsInChildren<Transform>())
        {
            if(t != graphTransform) Destroy(t.gameObject);
        }

        layoutCounter = 0;
        avgPos = Vector2.zero;
    }

    // Display the generated graph 
    void DisplayGraph () {
        DisplayNode(currentGraph.Root);
        PositionNode(currentGraph.Root, 0);
        AddNodeLinks(currentGraph.Root);
    }

    private void PositionNode(RoomNode pNode, int pDepth) {
        // Position each node in the graph...
        for (int i = 0; i < pNode.Children.Count; i++) {
            // r = whether this child is on the right of the parent
            PositionNode(pNode.Children[i], pDepth + 1);
        }

        layoutCounter++;

        float x = layoutCounter * displayOffset.x;
        float y = pDepth * displayOffset.y;

        pNode.Object.transform.localPosition = new Vector2(x, y);

        foreach (EdgeNode e in pNode.Edges) {
            // Ignore connections with no specified edge node.
            if (e != null) {
                e.Object.transform.position = (e.Parent.Object.transform.position + e.Child.Object.transform.position) / 2;
                e.Object.transform.parent = e.Parent.Object.transform;
            }
        }

        avgPos += (Vector2)pNode.Object.transform.position;
    }

    private void AddNodeLinks(RoomNode pNode) {
        for(int i = 0; i < pNode.Children.Count; i++) {
            GameObject g = Instantiate(nodeLinkerPrefab, pNode.Object.transform);
            
            Vector2 dir = (pNode.Children[i].Object.transform.position - pNode.Object.transform.position).normalized;
            g.transform.right = dir;

            g.transform.localScale = new Vector2(Vector2.Distance(pNode.Object.transform.position, pNode.Children[i].Object.transform.position), 1);
            g.transform.position = (pNode.Object.transform.position + pNode.Children[i].Object.transform.position) / 2;

            AddNodeLinks(pNode.Children[i]);
        }
    }

    // Display a room node, recursing through its children.
    private void DisplayNode(RoomNode pNode) {
        SpawnNode(pNode);

        // Spawn each node in the graph...
        foreach(RoomNode n in pNode.Children) {
            DisplayNode(n);

            foreach (EdgeNode e in n.Edges) {
                // Ignore connections with no specified edge node.
                if (e != null)
                    DisplayNode(e);
            }
        }
    }

    // Display an edge node (no recursion).
    private void DisplayNode(EdgeNode pNode) { SpawnNode(pNode); }

    // Create an object of a node.
    private void SpawnNode(Node pNode) {
        GameObject g = Instantiate(nodeImagePrefab, graphTransform);
        g.name = "Node (" + (NodeType)pNode[Data.Type] + ")";
        SpriteRenderer i = g.GetComponent<SpriteRenderer>();

        if (pNode == null) {
            return;
        }

        if ((NodeType)pNode[Data.Type] == NodeType.Switch) {
            i.sprite = SwitchSprites[(char)pNode[Data.ID] % SwitchSprites.Length];
        } else if ((NodeType)pNode[Data.Type] == NodeType.Barrier) {
            i.sprite = BarrierSprites[(char)pNode[Data.ID] % BarrierSprites.Length];
        } else {
            i.sprite = NodeSprites[(int)(NodeType)pNode[Data.Type]];
        }

        if (pNode.HasData(Data.ID) && (char)pNode[Data.ID] != ' ') {
            // Change the node's text
        }

        pNode.Object = g;
    }
}
