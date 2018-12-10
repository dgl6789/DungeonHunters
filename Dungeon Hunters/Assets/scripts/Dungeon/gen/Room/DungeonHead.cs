using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonHead : MonoBehaviour
{
    public Vector3[] Trajectories;                  //Useful post-gen
    public Vector2[] Locations;                     //useless post-gen
    private List<List<Vector2Int>> LinesTiles;       //Limited usage post-gen
    public List<Vector3Int> CaveTiles;              //efficient storage of data. useful for reconstruction
    public GameObject[,] AllGameObjects;            //just really used to shut the consol up
    public RoomCell[,] AllCells;                    //Simple container of other objects. Might be better off as a pointer to a single re-used group.
    public bool HasGenerated = false;              //Required for start logic
    private Vector2Int minBound, maxBound;          //useless post-gen
    public Vector3Int[,] RiverInformation;
    public Room NorthBranch, SouthBranch, EastBranch, WestBranch;
    [SerializeField] WholeDungeon myDungeon;
    public GameObject LinePrefab;
    public GameObject mobPrefab;


    // Use this for initialization
    void Start()
    {
        LinesTiles = new List<List<Vector2Int>>();
        minBound = new Vector2Int(1, 1);
        maxBound = new Vector2Int(78, 78);
        Locations = new Vector2[6];
        Trajectories = new Vector3[6];
        RiverInformation = new Vector3Int[80, 80];
        AllCells = new RoomCell[80, 80];//instantiates all tile datapieces
        AllGameObjects = new GameObject[80, 80];//instantiates all tile Gameobjects (this makes them passable to other rooms) 
        for (int i = 0; i < 80; i++)
        {
            for (int j = 0; j < 80; j++)
            {
                AllGameObjects[i, j] = Instantiate(LinePrefab, new Vector3(i / 2.0f, j / 2.0f, 0), Quaternion.identity, gameObject.transform);
                AllCells[i, j] = AllGameObjects[i, j].GetComponent<RoomCell>();
                AllCells[i, j].Gridlocation = new Vector2Int(i, j);
            }
        }
        myDungeon = GetComponentInParent<WholeDungeon>();
        GenerateLattice();

    }


    void GenerateLattice()
    {
        //assign the set starting positions and directions for the line-drawing calculations.
        for (int i = 0; i < 3; i++)
        {
            List<Vector2Int> v2ITemp = new List<Vector2Int>();
            LinesTiles.Add(v2ITemp);

            Locations[i] = new Vector2Int(0, 32 + (i * 10));
            Trajectories[i] = new Vector3(3, 1 - (1 * i), 0);
        }
        for (int i = 0; i < 3; i++)
        {
            List<Vector2Int> v2ITemp = new List<Vector2Int>();
            LinesTiles.Add(v2ITemp);

            Locations[3 + i] = new Vector2Int(32 + (i * 10), 0);
            Trajectories[3 + i] = new Vector3(1 - (1 * i), 3, 0);
        }

        float riseRun, runRise, absRiRu, absRuRi;
        //drawlines equivalent
        for (int i = 0; i < 6; i++)
        {
            bool contWhile = true;
            int index = 0;
            while (contWhile)
            {

                riseRun = Trajectories[i].x / Trajectories[i].y;
                runRise = Trajectories[i].y / Trajectories[i].x;
                absRiRu = Mathf.Abs(riseRun);
                absRuRi = Mathf.Abs(runRise);

                if (absRiRu >= absRuRi)//if the line is X dominant at present
                {
                    if (index > absRiRu)//make sure that we move in the opposite direction when we must
                    {
                        Locations[i].y += Mathf.Sign(Trajectories[i].y);
                        index = 0;
                    }
                    else//standard movement in primary direction
                    {
                        Locations[i].x += Mathf.Sign(Trajectories[i].x);
                        index++;
                    }
                }
                else//if the line is Y dominant at present
                {
                    if (index > absRuRi)//make sure that we move in the opposite direction when we must
                    {
                        Locations[i].x += Mathf.Sign(Trajectories[i].x);
                        index = 0;
                    }
                    else//standard movement in primary direction
                    {
                        Locations[i].y += Mathf.Sign(Trajectories[i].y);
                        index++;
                    }
                }

                if (Locations[i].x > -1.0f && Locations[i].x < 78.1f && Locations[i].y > -1.0f && Locations[i].y < 78.1f)//if we aren't out of bounds, increment
                {
                    LinesTiles[i].Add(new Vector2Int((int)Locations[i].x, (int)Locations[i].y));
                }
                else//if we are out of bounds, increment
                {
                    contWhile = false;
                }
            }
        }
        //Lines of erosion ahve been drawn, now erode around them.
        Vector2Int[] endpoints = new Vector2Int[6];
        for (int i = 0; i < 6; i++)
        {//main lines
            int temp = 0;
            foreach (Vector2Int tile in LinesTiles[i])
            {
                AllCells[tile.x, tile.y].RaiseTo(5, i,temp,0);
                temp++;
            }
            endpoints[i] = LinesTiles[i][(LinesTiles[i].Count - 1)];
        }
        int[] layerSize = { 9, 8, 6, 3, 1 };        
        int tier = 4, riverIndex = 0;
        // List<Vector2Int> UBorder, DBorder, LBorder, RBorder; //create seperate lists for each set of borders- used later on for movement of zones of depth        
        while (tier >= 0)
        {//Lift intial territory, and add then create borders.
            for (int i = 0; i < 6; i++)
            {//For each line
                riverIndex = 0;
                foreach (Vector2Int block in LinesTiles[i])
                {//For each tile                      
                    for (int j = 0; j <= layerSize[tier]; j++)
                    {//start with Maxsize, 0 

                        Vector2Int address = new Vector2Int(layerSize[tier] - j, 0 - j);
                        Vector2Int output = address + block;//Make sure the modification is applied to the action location
                        output.Clamp(minBound, maxBound);//And that that location is within the bounds of the map.                        
                        if (AllCells[output.x, output.y].RaiseTo(tier, i, riverIndex,0))// Raise the first tile, which is the center of the river, just for security
                            RiverInformation[output.x, output.y] = new Vector3Int(i, riverIndex, 0);
                        while ((address.x * address.x) + (address.y * address.y) < (layerSize[tier] * layerSize[tier]))
                        {//Increase the y value of the relative position until the distance from the block is too large.
                            address.y++;
                            output = address + block;
                            output.Clamp(minBound, maxBound);
                            if (AllCells[output.x, output.y].RaiseTo(tier, i, riverIndex, Mathf.Abs(address.x) + Mathf.Abs(address.y)))
                                RiverInformation[output.x, output.y] = new Vector3Int(i, riverIndex, Mathf.Abs(address.x) + Mathf.Abs(address.y));
                        }

                    }
                    for (int j = 0; j <= layerSize[tier]; j++)
                    {//start with -Maxsize, 0
                        Vector2Int address = new Vector2Int(j - layerSize[tier], 0 - j);
                        Vector2Int output = address + block;
                        output.Clamp(minBound, maxBound);
                        AllCells[output.x, output.y].RaiseTo(tier, i, riverIndex,0);
                        while ((address.x * address.x) + (address.y * address.y) < (layerSize[tier] * layerSize[tier]))
                        {
                            address.y++;
                            output = address + block;
                            output.Clamp(minBound, maxBound);
                            if (AllCells[output.x, output.y].RaiseTo(tier, i, riverIndex, Mathf.Abs(address.x) + Mathf.Abs(address.y)))
                                RiverInformation[output.x, output.y] = new Vector3Int(i, riverIndex, Mathf.Abs(address.x) + Mathf.Abs(address.y));
                        }

                    }

                    riverIndex++;//keep track of the current tile within the River
                }

            }
            tier--;
        }
        DefineCave();
    }

    void DefineCave()
    {//Determine what tiles have been altered, and to what depth   
        CaveTiles.Clear();
        for (int i = 0; i < 80; i++)
        {
            for (int j = 0; j < 80; j++)
            {
                if (AllCells[i, j].Height > 0)
                {
                    CaveTiles.Add(new Vector3Int(i, j, AllCells[i, j].Height));
                }
            }
        }
        HasGenerated = true;
    }

    public void ExtendCave(List<Monster> incMobList, int Dir, Room incRoom)
    {//Move to the next cave
        switch (Dir)
        {
            case 0:
                NorthBranch = incRoom;
                NorthBranch.DeclareToPreventErrors();
                NorthBranch.sourceDir = 2;
                NorthBranch.Trajectories = new Vector3[] { Trajectories[3], Trajectories[4], Trajectories[5] };
                NorthBranch.Locations = new Vector2[] { Locations[3], Locations[4], Locations[5] };
                NorthBranch.myDungeon = myDungeon;
                NorthBranch.AllCells = AllCells;
                NorthBranch.ConvertFromExtend(incMobList);
                NorthBranch.AssignMonsters(incMobList);
                break;
            case 1:
                EastBranch = incRoom;
                EastBranch.DeclareToPreventErrors();
                EastBranch.sourceDir = 3;
                EastBranch.Trajectories = new Vector3[] { Trajectories[0], Trajectories[1], Trajectories[2] };
                EastBranch.Locations = new Vector2[] { Locations[0], Locations[1], Locations[2] };
                EastBranch.myDungeon = myDungeon;
                EastBranch.AllCells = AllCells;
                EastBranch.ConvertFromExtend(incMobList);
                EastBranch.AssignMonsters(incMobList);
                break;
            case 2:
                SouthBranch = incRoom;
                SouthBranch.sourceDir = 0;  
                SouthBranch.DeclareToPreventErrors();
                SouthBranch.Trajectories = new Vector3[] { -Trajectories[3], -Trajectories[4], -Trajectories[5] };
                SouthBranch.Locations = new Vector2[] { Locations[3], Locations[4], Locations[5] };
                SouthBranch.myDungeon = myDungeon;
                SouthBranch.AllCells = AllCells;    
                SouthBranch.ConvertFromExtend(incMobList);
                SouthBranch.AssignMonsters(incMobList);
                break;
            case 3:
                WestBranch = incRoom;    
                WestBranch.DeclareToPreventErrors();
                WestBranch.sourceDir = 1;
                WestBranch.Trajectories = new Vector3[] { -Trajectories[0], -Trajectories[1], -Trajectories[2] };
                WestBranch.Locations = new Vector2[] { Locations[0], Locations[1], Locations[2] };
                WestBranch.myDungeon = myDungeon;
                WestBranch.AllCells = AllCells;
                WestBranch.ConvertFromExtend(incMobList);
                WestBranch.AssignMonsters(incMobList);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RebuildCave()
    {
        foreach (Vector3Int tile in CaveTiles)
        {
            AllCells[tile.x, tile.y].IncrimentHeight(tile.z, 0,0,0);//Faster operation for defined maps.
        }
    }

    public void ClearRoom()
    {
        foreach (Vector3Int tile in CaveTiles)
        {
            AllCells[tile.x, tile.y].Reset();
        }
    }
}


