using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int pointsForBuy;                        //used in generation.
    public bool primaryRoom;                        //Tells map weather to generate, or receive its map
    public GameObject LinePrefab;                   
    private Vector3[] Trajectories;                  //Useful post-gen
    private Vector2[] Locations;                     //useless post-gen
    private List<List<Vector2Int>> LinesTiles;       //Limited usage post-gen
    public List<Vector3Int> CaveTiles;              //efficient storage of data. useful for reconstruction
    public List<Vector4> OreTiles;              //efficient storage of data. useful for reconstruction
    public RoomCell[,] AllCells;                    //Simple container of other objects. Might be better off as a pointer to a single re-used group.
    public int sourceDir;                           //Mildly important for map linking.    
    public int destinationDir;                      //0-3, 0 being up, 1 being right, 2, being down, 3 being left
    private bool HasGenerated = false;              //Required for start logic
    public bool displayFrameTime;                   //use largely in debugging
    public bool reGen = false, forward = false, backward = false;//Temp variables to use in "map Navigation"                      
    public bool reCon = false;                      //use to Recontruct a map
    private Vector2Int minBound, maxBound;          //useless post-gen
    public Room nextRoom, previousRoom;    

    // Use this for initialization
    void Start()
    {
        LinesTiles = new List<List<Vector2Int>>();
        minBound = new Vector2Int(1, 1);
        maxBound = new Vector2Int(78, 78);
        Locations = new Vector2[3];
        Trajectories = new Vector3[3];
        if (primaryRoom)
        {
            AllCells = new RoomCell[80, 80];//instantiates all cubes
            for (int i = 0; i < 80; i++)
            {
                for (int j = 0; j < 80; j++)
                {
                    AllCells[i, j] = new RoomCell
                    {
                        cubeTemp = Instantiate(LinePrefab, new Vector3(i / 2.0f, j / 2.0f, 0), Quaternion.identity, gameObject.transform),
                        Gridlocation = new Vector2Int(i, j)
                    };
                }

            }
            if (nextRoom != null) { 
                Room temproom;
                temproom = nextRoom;
                temproom.AllCells = AllCells;
                while (temproom.nextRoom != null)
                {
                    temproom = temproom.nextRoom;
                    temproom.AllCells = AllCells;
                }
            }
            GeneratePrelim();
        }
    }
    void GeneratePrelim()
    {//Creates the Preliminary requirements to make those lines
       
        for (int i = 0; i < 3; i++)
        {
            List<Vector2Int> v2ITemp = new List<Vector2Int>();
            LinesTiles.Add(v2ITemp);
            int XMin = -5, XMax = 5, YMax = 5, YMin = -5;//Set max values for the the lines of erosion

            switch (sourceDir)
            {// set starting position and make sure we dont go immediately offscreen
                case 0: // coming from straight up
                    YMax = -1;
                    Locations[i] = new Vector2Int(Random.Range(30, 50), 78);
                    break;
                case 1://Coming from due Right
                    XMax = -1;
                    Locations[i] = new Vector2Int(78, Random.Range(30, 50));
                    break;
                case 2://coming from straight Down
                    YMin = 1;
                    Locations[i] = new Vector2Int(Random.Range(30, 50), 1);
                    break;
                case 3://Coming from due Left
                    XMin = 1;
                    Locations[i] = new Vector2Int(1, Random.Range(30, 50));
                    break;
            }          

            //increase range of options for non-opposite angles (L shaped passages, instead of I shaped ones).
            if (Mathf.Abs(sourceDir - destinationDir) != 2)
            {//if we have an 90 degree difference in our map, make lines crazier
                if (sourceDir == 1 || sourceDir == 3)
                {
                    XMax += (int)Mathf.Sign(XMax) * 2;
                    XMin += (int)Mathf.Sign(XMin) * 2; ;
                }
                else
                {
                    YMax += (int)Mathf.Sign(YMax) * 2;
                    YMin += (int)Mathf.Sign(YMin) * 2; ;
                }
            }

            Trajectories[i] = new Vector3(Random.Range(XMin, XMax), Random.Range(YMin, YMax), 0);
        }
        DrawLines();
        LiftTiles();
    }   
    
    private void DrawLines()
    {//Draws lines of action

        for (int i = 0; i < 3; i++)
        {
            float toSwitchAt, riseRun, runRise, absRiRu, absRuRi;
            int index=0;

            toSwitchAt = Mathf.Abs(Trajectories[i].x) + Mathf.Abs(Trajectories[i].y);//figure out how long this stretch is

            LinesTiles[i].Add(new Vector2Int((int)Locations[i].x, (int)Locations[i].y));//Starting point is incrimented

            riseRun = Trajectories[i].x / Trajectories[i].y;
            runRise = Trajectories[i].y / Trajectories[i].x;
            absRiRu = Mathf.Abs(riseRun);
            absRuRi = Mathf.Abs(runRise); 
            
            bool contWhile = true;
            while (contWhile)
            {
                if (index > toSwitchAt)//if its time to switch trajectories
                {
                    switch (destinationDir)
                    {//we need to know how to switch things up
                        case 0://up - Increase y, change x based on position from center
                               //Code for switching rise or Run;
                            if (Trajectories[i].y < 1)
                            {
                                Trajectories[i].y++;
                            }
                            else if (Mathf.Abs(Trajectories[i].x) > 1)
                            {
                                Trajectories[i].x -= Mathf.Sign(Trajectories[i].x);
                            }
                            else
                            {
                                Trajectories[i].y++;
                            }
                            break;
                        case 1://Right - Increase X, change Y based on position from center.
                            if (Trajectories[i].x < 1)
                            {
                                Trajectories[i].x++;
                            }
                            else if (Mathf.Abs(Trajectories[i].y) > 1)
                            {
                                Trajectories[i].y -= Mathf.Sign(Trajectories[i].y);
                            }
                            else
                            {
                                Trajectories[i].x++;
                            }
                            break;
                        case 2://Down - Decrease Y, change X based on position from center.
                            if (Trajectories[i].y > -1)
                            {
                                Trajectories[i].y--;
                            }
                            else if (Mathf.Abs(Trajectories[i].x) > 1)
                            {
                                Trajectories[i].x -= Mathf.Sign(Trajectories[i].x);
                            }
                            else
                            {
                                Trajectories[i].y--;
                            }
                            break;
                        case 3://Left - Decrease X, change Y based on position from center.
                            if (Trajectories[i].x > -1)
                            {
                                Trajectories[i].x--;
                            }
                            else if (Mathf.Abs(Trajectories[i].y) > 1)
                            {
                                Trajectories[i].y -= Mathf.Sign(Trajectories[i].y);
                            }
                            else
                            {
                                Trajectories[i].x--;
                            }
                            break;
                    }//Pull as much data out of the switch statement as possible.
                    riseRun = Trajectories[i].x / Trajectories[i].y;
                    runRise = Trajectories[i].y / Trajectories[i].x;
                    toSwitchAt = Mathf.Max(Mathf.Abs(Trajectories[i].x) + Mathf.Abs(Trajectories[i].y), 1);

                    //Calc absolute values of Rise and Run once, here.
                    absRiRu = Mathf.Abs(riseRun);
                    absRuRi = Mathf.Abs(runRise);
                }


                if (absRiRu >= absRuRi)//if the line is X dominant at present
                {
                    if (index > absRiRu)//make sure that we move in the opposite direction when we must
                    {
                        Locations[i].y += Mathf.Sign(Trajectories[i].y);
                        index = 0;
                        toSwitchAt -= absRiRu;
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
                        toSwitchAt -= absRuRi;
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
    }    

    void LiftTiles()
    {//Raises tiles alongside lines of action
        Vector2Int[] endpoints = new Vector2Int[3];
        int largestEP = 0;
        for (int i = 0; i < 3; i++)
        {//main lines
            foreach (Vector2Int tile in LinesTiles[i])
            {
                AllCells[tile.x, tile.y].RaiseTo(5, i);
            }
            endpoints[i] = LinesTiles[i][(LinesTiles[i].Count - 1)];
        }
        int[] deltaEP = new int[3]; //Difference in endpoints 
        deltaEP[0] = Mathf.Abs(endpoints[0].x - endpoints[1].x) + Mathf.Abs(endpoints[0].y - endpoints[1].y);  
        deltaEP[1] = Mathf.Abs(endpoints[2].x - endpoints[1].x) + Mathf.Abs(endpoints[2].y - endpoints[1].y);
        deltaEP[2] = Mathf.Abs(endpoints[0].x - endpoints[2].x) + Mathf.Abs(endpoints[0].y - endpoints[2].y);
        for (int i = 0; i < 3; i++)
        {
            if (deltaEP[i] > largestEP)
                largestEP = deltaEP[i];
        }

        int averageEP = (deltaEP[0] + deltaEP[1] + deltaEP[2]) / 3;
        if (averageEP < 18)
            averageEP = 18;
        if (averageEP < largestEP / 2)
            averageEP = largestEP / 2;
        //Make sure that there is enough room to deciment all the way down, and that there is enough room to make an ending.

        //We now have an average distance between lines of erosion. Now we should connect them, using sone simple math.
        /* Optimal Distance for each layer, with a depth of 5.   This nets a total "width" of 9
            1                                 1                 * For each set, multiple its base value by Average EP/9
              2 2                        2 2                    * take the Average Ep, % 9.
                 3 3 3             3 3 3                        * Add remainder to  each layer as appropriate - 2, 3, 1, 2, 3, 1, 4, 0
                       2 2     2 2                              * Then for each line, start with layer 5(the bottom) and work up, down, left and right as appropriate - this will create a circle of max depth.         
                           1 1                                  * Assign the edge of the circle to its relevent lists as an edge for up, down, left and right
                                                                * for all future layers on the circle, inciment only in the relevent directions, according to what list its in.
         */
        int minsize = averageEP / 9;
        int[] layerSize = { minsize, minsize * 2, minsize * 3, minsize * 2, minsize };//Correctly assign the distribution of value.
        switch (averageEP % 9)
        {
            case 0:
                break;
            case 1:
                layerSize[2] += 1;
                break;
            case 2:
                layerSize[2] += 1;
                layerSize[3] += 1;
                break;
            case 3:
                layerSize[1] += 1;
                layerSize[2] += 1;
                layerSize[3] += 1;
                break;
            case 4:
                layerSize[1] += 1;
                layerSize[2] += 2;
                layerSize[3] += 1;
                break;
            case 5:
                layerSize[1] += 1;
                layerSize[2] += 2;
                layerSize[3] += 2;
                break;
            case 6:
                layerSize[1] += 2;
                layerSize[2] += 2;
                layerSize[3] += 2;
                break;
            case 7:
                layerSize[1] += 1;
                layerSize[2] += 2;
                layerSize[3] += 2;
                layerSize[4] += 2;
                break;
            case 8:
                layerSize[0] += 1;
                layerSize[1] += 2;
                layerSize[2] += 2;
                layerSize[3] += 2;
                layerSize[4] += 1;
                break;
        }

        int tier = 4, index = 0;
        // List<Vector2Int> UBorder, DBorder, LBorder, RBorder; //create seperate lists for each set of borders- used later on for movement of zones of depth        
        while (tier >= 0)
        {//Lift intial territory, and add then create borders.
            for (int i = 0; i < 3; i++)
            {//For each line
                foreach (Vector2Int block in LinesTiles[i])
                {//For each tile  
                    for (int j = 0; j < layerSize[tier]; j++)
                    {//start with Maxsize, 0 

                        Vector2Int address = new Vector2Int(layerSize[tier] - j, 0 - j);
                        Vector2Int output = address + block;//Make sure the modification is applied to the action location
                        output.Clamp(minBound, maxBound);//And that that location is within the bounds of the map.                        
                        AllCells[output.x, output.y].RaiseTo(tier, i); // Raise the first tile, which is the center of the river, just for security
                        while ((address.x* address.x) + (address.y*address.y) < (layerSize[tier]* layerSize[tier]))
                        {//Increase the y value of the relative position until the distance from the block is too large.
                            address.y++;
                            output = address + block;
                            output.Clamp(minBound, maxBound);
                            AllCells[output.x, output.y].RaiseTo(tier, i);                     
                        }

                    }
                    for (int j = 0; j < layerSize[tier]; j++)
                    {//start with -Maxsize, 0
                        Vector2Int address = new Vector2Int(j - layerSize[tier], 0 - j);
                        Vector2Int output = address + block;
                        output.Clamp(minBound, maxBound);                      
                        AllCells[output.x, output.y].RaiseTo(tier, i);
                        while ((address.x * address.x) + (address.y * address.y) < (layerSize[tier] * layerSize[tier]))
                        {
                            address.y++;
                            output = address + block;
                            output.Clamp(minBound, maxBound);
                            AllCells[output.x, output.y].RaiseTo(tier, i);                      
                        }

                    }

                    index++;//keep track of current/last tile
                }
                index = 0;
            }
            tier--;
        }       
        DefineCave();
        AssignOres();
    }
    
    void DefineCave() {//Determine what tiles have been altered, and to what depth   
        CaveTiles.Clear();
        for(int i =0; i<80; i++)
        {
            for (int j = 0; j < 80; j++)
            {
                if(AllCells[i,j].Height > 0)
                {
                    CaveTiles.Add(new Vector3Int(i, j, AllCells[i, j].Height));
                }
            }
        }
        HasGenerated = true;
    }

    void RebuildCave()
    {
        foreach(Vector3Int tile in CaveTiles)
        {
            AllCells[tile.x, tile.y].IncrimentHieght(tile.z,0);//Faster operation for defined maps.
        }
        BuildOres();
    }
    void AssignOres()
    {
        OreTiles.Clear();
        int orePoints = pointsForBuy, maxPoints =9, currentVein, index;//make some local variables

        while(orePoints > 0)//while we have points left to spend
        {
            if(orePoints < maxPoints)// make sure we don't spend more points than we have available
            {
                maxPoints = orePoints;
            }
            currentVein = Random.Range(1, maxPoints+1);//give us a random value to use
            index = Random.Range(0, CaveTiles.Count);// and give us a random location
            Vector4 temp = new Vector4(CaveTiles[index].x, CaveTiles[index].y, CaveTiles[index].z+1, currentVein);//Combine the value, and location    
            CaveTiles[index] = new Vector3Int(CaveTiles[index].x, CaveTiles[index].y, CaveTiles[index].z + 1);
            OreTiles.Add(temp);//Add it to the list
            orePoints -= currentVein;//and expend those points
        }
        BuildOres();
    }
    void BuildOres()
    {
        foreach (Vector4 tile in OreTiles)
        {
            AllCells[(int)tile.x, (int)tile.y].AssignOreValue((int)tile.w);//Faster operation for defined maps.
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (displayFrameTime)
        {
            displayFrameTime = false;
            Debug.Log(Time.deltaTime);
        }
        else if (reGen)
        {
            DefineCave();
            reGen = false;
            foreach (Vector3Int tile in CaveTiles)
            {
                AllCells[tile.x, tile.y].Reset();
            }
            LinesTiles.Clear();
            GeneratePrelim();
            LiftTiles();
            displayFrameTime = true;
        }
        else if (reCon)
        {
            reCon = false;
            foreach (Vector3Int tile in CaveTiles)
            {
                AllCells[tile.x, tile.y].Reset();
            }
            RebuildCave();
            
        }
        else if (forward)
        {
            foreach (Vector3Int tile in CaveTiles)
            {
                AllCells[tile.x, tile.y].Reset();
            }
            forward = false;
            LinesTiles.Clear();
            ExtendCave();
        }
        else if (backward)
        {
            if (previousRoom != null)
            {
                foreach (Vector3Int tile in CaveTiles)
                {
                    AllCells[tile.x, tile.y].Reset();
                }
                previousRoom.RebuildCave();
                
            }
            backward = false;
        }
    }

    void ExtendCave()
    {//Move to the next cave        
        if(nextRoom != null)
        {//You know, if we're allowed to
            if(nextRoom.HasGenerated == true)
            {//if it already exists, do the fast draw.                
                nextRoom.RebuildCave();
            }
            else
            {//Otherwise, feed it in its pre-reqs, and get started;
                nextRoom.sourceDir = (destinationDir+2)%4;
                nextRoom.Trajectories = Trajectories;
                nextRoom.Locations = Locations;
                nextRoom.ConvertFromExtend();
                nextRoom.DrawLines();
                nextRoom.LiftTiles();
            }
        }
    }

    void ConvertFromExtend()
    {//Turn the input'd data into something useful, and generate what we can't get.

        //Start off by culling those trajectories and starting positions
        for (int i = 0; i < 3; i++)
        {
            Trajectories[i].x = Mathf.Clamp(Trajectories[i].x, -5-i, 5+i);
            Trajectories[i].y = Mathf.Clamp(Trajectories[i].y, -5-i, 5+i);
            List<Vector2Int> v2ITemp = new List<Vector2Int>();
            LinesTiles.Add(v2ITemp);
        }
            switch (sourceDir)
            {
            case 0://up 
                for (int i = 0; i < 3; i++)
                {
                    Locations[i].y = 78;
                }
                destinationDir = (int)Random.Range(1.0f, 3.1f);
            break;
            case 1://from right
                do {
                    destinationDir = (int)Random.Range(0.0f, 3.1f);
                 }while(destinationDir == 1);
                for (int i = 0; i < 3; i++)
                {
                    Locations[i].x = 78;
                }
                break;
            case 2://down
                do
                {
                    destinationDir = (int)Random.Range(0.0f, 3.1f);
                } while (destinationDir == 2);
                for (int i = 0; i < 3; i++)
                {
                    Locations[i].y =1;
                }
                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    Locations[i].x = 1;
                }
                destinationDir = (int)Random.Range(0.0f, 2.9f);
                break;
            }


        
    }
}
