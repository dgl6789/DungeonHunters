using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WholeDungeon : MonoBehaviour {
    public GameObject RoomPrefab;
    public List<List<GameObject>> allRooms; // Pontentially not needed. Possibly a good idea to have though.
    public Room activeRoom;
    public bool forTesting;
    public bool forwardBack;
    public Button [] NavigationButtons;

	// Use this for initialization
	void Start () {
        CalcDirections();
	}

    private void Awake()
    {
        CalcDirections();
    }
    // Update is called once per frame
    void Update () {

        if (forTesting)
        {
            forTesting = false;
            TraverseRooms(forwardBack);
        }
	}

    public void TraverseRooms(bool isFoward)
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
                activeRoom.OnRoomSwitch(true);

            }
        }
        else{
            if (activeRoom.previousRoom != null)
            {//if we have forward to move to
                activeRoom.ClearRoom();                
                activeRoom = activeRoom.previousRoom;
                activeRoom.RebuildCave();
                activeRoom.OnRoomSwitch(false);
            }
        }

        CalcDirections();

    }

   void CalcDirections()
    {
        for (int i = 0; i < 8; i++)
        {
            NavigationButtons[i].gameObject.SetActive(false);
        }
        switch (activeRoom.sourceDir)
        {
            case 0:
                NavigationButtons[4].gameObject.SetActive(true);
                break;
            case 1:
                NavigationButtons[5].gameObject.SetActive(true);
                break;
            case 2:
                NavigationButtons[6].gameObject.SetActive(true);
                break;
            case 3:
                NavigationButtons[7].gameObject.SetActive(true);
                break;

        }

        switch (activeRoom.destinationDir)
        {
            case 0:
                NavigationButtons[0].gameObject.SetActive(true);
                break;
            case 1:
                NavigationButtons[1].gameObject.SetActive(true);
                break;
            case 2:
                NavigationButtons[2].gameObject.SetActive(true);
                break;
            case 3:
                NavigationButtons[3].gameObject.SetActive(true);
                break;

        }
    }
}
