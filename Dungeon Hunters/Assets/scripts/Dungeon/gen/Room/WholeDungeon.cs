using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WholeDungeon : MonoBehaviour {
    public GameObject RoomPrefab;
    public List<List<GameObject>> allRooms; // Pontentially not needed. Possibly a good idea to have though.
    public Room activeRoom;
    public bool forTesting;
    public bool forwardBack;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (forTesting)
        {
            forTesting = false;
            TraverseRooms(forwardBack);
        }
	}

    void TraverseRooms(bool isFoward)
    {//Code to move from room to room
        //REQUIRES CAMERA CODE TO INDICATE CHANGE OR MOVEMENT
        //Not sure how to do it though, probably for DOM

        if (isFoward)
        {// if we are moving forward
            if(activeRoom.nextRoom != null)
            {//if we have forward to move to
                activeRoom.ClearRoom();
                activeRoom.ExtendCave();
                activeRoom = activeRoom.nextRoom; 
            }
        }
        else{
            if (activeRoom.previousRoom != null)
            {//if we have forward to move to
                activeRoom.ClearRoom();               
                activeRoom = activeRoom.previousRoom;
                activeRoom.RebuildCave();
            }
        }

    }
}
