using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dungeon;



public class WholeDungeon : MonoBehaviour {
    enum DisplayState {None, Attack, Walk, Stance, Data }
    private DisplayState myDisplayState = 0;
    public GameObject RoomPrefab;
    public List<List<GameObject>> allRooms; // Pontentially not needed. Possibly a good idea to have though.
    public Room activeRoom;
    public bool forTesting;
    public bool forwardBack;
    public Button [] NavigationButtons;
    public List<Mercenary> AllActiveMercenaries;
    public List<Monster> AllActiveMonsters;
    public int ActiveMerc;
    public bool dirty= true;

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
        if (dirty)
        {
            DrawTick();
            dirty = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UndrawTick();
            ActiveMerc--;
            if (ActiveMerc < 0)
                ActiveMerc = AllActiveMercenaries.Count - 1;
            dirty = true;          
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            UndrawTick();
            ActiveMerc++;
            ActiveMerc = ActiveMerc % AllActiveMercenaries.Count;
            dirty = true;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            UndrawTick();
            myDisplayState = DisplayState.Walk;
            dirty = true;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            UndrawTick();
            myDisplayState = DisplayState.Attack;
            dirty = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            UndrawTick();
            myDisplayState = DisplayState.Stance;
            dirty = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            UndrawTick();
            myDisplayState = DisplayState.Data;
            dirty = true;
        }
    }

    public void AdvanceTurn()
    {
        foreach (Mercenary merc in AllActiveMercenaries)
        {//reset merc data on every turn
            merc.Movement = 5;
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
                activeRoom.AssignMonsters(AllActiveMonsters);
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

    void CharacterTick()
    {
        foreach(Mercenary merc in AllActiveMercenaries)
        {
            activeRoom.HighLightZones(1, merc.gridPosition,0,0);
        }
    }

    void CharacterTick(int Index, bool Active)
    {//Draws or undraws a single character
        if (Active)
        {
            activeRoom.HighLightZones(1, AllActiveMercenaries[Index].gridPosition, 0, 0);
        }
        else
        {
            activeRoom.HighLightZones(0, AllActiveMercenaries[Index].gridPosition, 0, 0);
        }
    }

    void MovementTick(bool activating)
    {//Draws or removes the movement selector
        if (activating)
        {
            activeRoom.HighLightZones(2, AllActiveMercenaries[ActiveMerc].gridPosition, 1, AllActiveMercenaries[ActiveMerc].Movement);           
        }
        else
        {            
           activeRoom.HighLightZones(0, AllActiveMercenaries[ActiveMerc].gridPosition, 0, AllActiveMercenaries[ActiveMerc].Movement);            
        }
    }
    
    void RangeTick(bool activating)
    {//draws a character's weapon range- to be used to select targets.
        if (activating)
        {
            activeRoom.HighLightZones(3, AllActiveMercenaries[ActiveMerc].gridPosition, 3, AllActiveMercenaries[ActiveMerc].Movement);
        }
        else
        {
            activeRoom.HighLightZones(0, AllActiveMercenaries[ActiveMerc].gridPosition, 3, AllActiveMercenaries[ActiveMerc].Movement);
        }
    }

    public void MoveActiveMerc(Vector2Int index)
    {
        if(ActiveMerc != -1)
        {//ensure that we aren't moving a character that can't be moved
            MovementTick(false);
            CharacterTick(ActiveMerc, false);
            int totalMovement = (Mathf.Abs(AllActiveMercenaries[ActiveMerc].gridPosition.x - index.x) + Mathf.Abs(AllActiveMercenaries[ActiveMerc].gridPosition.y - index.y));
            AllActiveMercenaries[ActiveMerc].gridPosition = index;
            AllActiveMercenaries[ActiveMerc].Movement -= totalMovement;
            MovementTick(true);
            CharacterTick(ActiveMerc, true);
            MobTick();
        }
    }

    public void MobTick()
    {
        foreach(Monster mob in activeRoom.ActiveMonsters)
        {
            activeRoom.HighLightZones(4, mob.gridPosition, 0, 0);
        }
    }

    void DrawTick()
    {
        switch (myDisplayState)
        {
            case DisplayState.None:
                CharacterTick();
                MobTick();
                break;
            case DisplayState.Walk:
                MovementTick(true);
                CharacterTick();
                MobTick();
                break;
            case DisplayState.Attack:
                CharacterTick();
                MobTick();
                activeRoom.HighLightTargets(AllActiveMercenaries[ActiveMerc].gridPosition, 2,5, true);      
                break;
            case DisplayState.Stance:                
                foreach (Mercenary Merc in AllActiveMercenaries)
                {
                    activeRoom.HighLightZones(7, Merc.gridPosition, 1, 2);
                }
                foreach (Monster Mob in activeRoom.ActiveMonsters)
                {
                    activeRoom.HighLightZones(6, Mob.gridPosition, 1, 2);
                }
                CharacterTick();
                MobTick();
                break;
            default:
               
                MobTick();
                break;
        }
        CharacterTick();
    }

    void UndrawTick()
    {
        switch (myDisplayState)
        {
            case DisplayState.None:
                break;
            case DisplayState.Walk:
                MovementTick(false);
                break;
            case DisplayState.Attack:
                activeRoom.HighLightTargets(AllActiveMercenaries[ActiveMerc].gridPosition, 2, 5, false);
                CharacterTick();
                MobTick();
                break;
            case DisplayState.Stance:
                foreach (Mercenary Merc in AllActiveMercenaries)
                {
                    activeRoom.HighLightZones(0, Merc.gridPosition, 1, 2);
                }
                foreach (Monster Mob in activeRoom.ActiveMonsters)
                {
                    activeRoom.HighLightZones(0, Mob.gridPosition, 1, 2);
                }
                CharacterTick();
                MobTick();
                break;

        }
        CharacterTick();
        MobTick();
    }
    
    public Monster GetMobFromLoc(Vector2Int incPos)
    {//get a monster at a particular location
        
        foreach(Monster mob in activeRoom.ActiveMonsters)
        {
            if(mob.gridPosition == incPos)
            {
                return mob;
            }
        }

        return null;
    }

    public Mercenary GetMercFromLoc(Vector2Int incPos)
    {//get a Mercenary at a particular location

        foreach (Mercenary merc in AllActiveMercenaries)
        {
            if (merc.gridPosition == incPos)
            {
                return merc;
            }
        }

        return null;
    }

    public void RunAttack(Mercenary merc, Monster mob, bool PlayerTurn)
    {
        if (PlayerTurn)
        {
            Attack holding = merc.GenerateAttack();
            mob.RecieveAttack(holding);
            merc.Movement -= 3;
        }
        else
        {
            Attack holding = mob.GenerateAttack();
            merc.RecieveAttack(holding);
            mob.Stamina -= 3;
        }
    }
}
