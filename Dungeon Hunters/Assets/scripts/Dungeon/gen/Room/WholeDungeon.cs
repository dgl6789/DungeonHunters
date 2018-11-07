﻿using System.Collections;
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
            merc.Stamina += merc.Movement; //decrease stamina for "Extra" moves
            merc.Stamina = Mathf.Min(merc.Stamina, merc.MaxStamina);
            merc.Movement = 5;
        }
        foreach(Monster mob in activeRoom.ActiveMonsters)
        {
            mob.Stamina += mob.Movement; //decrease stamina for "Extra" moves
            mob.Stamina = Mathf.Min(mob.Stamina, mob.MaxStamina);
            mob.Movement = 5;
        }
        UndrawTick();
        activeRoom.EnemyUpdate(AllActiveMercenaries);
        DrawTick();
    }

    public void TraverseRooms(bool isFoward)
    {//Code to move from room to room
        //REQUIRES CAMERA CODE TO INDICATE CHANGE OR MOVEMENT
        //Not sure how to do it though, probably for DOM

        if (isFoward)
        {// if we are moving forward
            if(activeRoom.nextRoom != null)
            {//if we have forward to move to
                UndrawTick();                
                activeRoom.ClearRoom();
                activeRoom.ExtendCave(AllActiveMonsters);                
                activeRoom = activeRoom.nextRoom;                
                activeRoom.OnRoomSwitch(true);
                activeRoom.PlaceMercs(true, true);
                forwardBack = true;
            }
        }
        else{
            if (activeRoom.previousRoom != null)
            {//if we have forward to move to
                UndrawTick();
                activeRoom.ClearRoom();                
                activeRoom = activeRoom.previousRoom;
                activeRoom.RebuildCave();
                activeRoom.OnRoomSwitch(false);
                activeRoom.PlaceMercs(false, true);
                forwardBack = false;

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
        if (activeRoom.ActiveMonsters.Count <= 0)
        {
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

    void CharacterTick(bool activating)
    {
        if (activating)
        {
            foreach (Mercenary merc in AllActiveMercenaries)
            {
                activeRoom.HighLightZones(1, merc.gridPosition, 0, 0);
            }
        }
        else
        {
            foreach (Mercenary merc in AllActiveMercenaries)
            {
                activeRoom.HighLightZones(0, merc.gridPosition, 0, 0);
            }
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
            if(AllActiveMercenaries[ActiveMerc].Movement> 0)//can't attack with 0 movement.
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
            CharacterTick(true);
            MobTick(true);
        }
    }
    public void PlaceActiveMerc(Vector2Int index)
    {
        if (ActiveMerc != -1)
        {//ensure that we aren't moving a character that can't be moved           
            AllActiveMercenaries[ActiveMerc].gridPosition = index;
            CharacterTick(ActiveMerc, true);
            ActiveMerc++;
            if (ActiveMerc == AllActiveMercenaries.Count)
            {
                ActiveMerc = ActiveMerc % AllActiveMercenaries.Count;
                activeRoom.PlaceMercs(forwardBack, false);
                dirty = true;
            }
        }
    }

    public void MobTick(bool activating)
    {
        if (activating)
        {
            foreach (Monster mob in activeRoom.ActiveMonsters)
            {
                activeRoom.HighLightZones(4, mob.gridPosition, 0, 0);
            }
        }
        else
        {
            foreach (Monster mob in activeRoom.ActiveMonsters)
            {
                activeRoom.HighLightZones(0, mob.gridPosition, 0, 0);
            }

        }
    }

    void DrawTick()
    {
        switch (myDisplayState)
        {
            case DisplayState.None:
                CharacterTick(true);
                MobTick(true);
                break;
            case DisplayState.Walk:
                MovementTick(true);
                CharacterTick(true);
                MobTick(true);
                break;
            case DisplayState.Attack:
                CharacterTick(true);
                MobTick(true);
                if (AllActiveMercenaries[ActiveMerc].Movement > 0)//can't attack with 0 movement.
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
                CharacterTick(true);
                MobTick(true);
                break;
            default:
               
                MobTick(true);
                break;
        }
        CharacterTick(true);
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
                break;
        }
        MobTick(false);
        CharacterTick(false);
        
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
        {//This is an attack on an enemy
            Attack holding = merc.GenerateAttack();
            if (mob.RecieveAttack(holding))
            {//if the attack hits, lower enemy morale, and raise mine
                mob.Morale -= 2;
                merc.Morale += 1;

            }
            merc.Movement -= 3;

            //See if the enemy is dead
            if(mob.Health <= 0)
            {
                UndrawTick();
                activeRoom.ActiveMonsters.Remove(mob);
                Destroy(mob.gameObject);
                foreach(Monster struckByFear in activeRoom.ActiveMonsters)
                {
                    struckByFear.Morale -= 2;
                }
                merc.Morale += 1;
                foreach(Mercenary encouraged in AllActiveMercenaries)
                {
                    encouraged.Morale += 1;
                }
                CalcDirections();
                DrawTick();
            }
            else
            {
                MobTick(true);
            }
        }
        else
        {//This is not an attack on an enemy
            Attack holding = mob.GenerateAttack();
            if (merc.RecieveAttack(holding))
            {//if a player character is hit
                mob.Morale++;
                merc.Morale -= 2;                
            }
            mob.Stamina -= 3;

            if (merc.Health <= 0)
            {//if a merc dies
                UndrawTick();//undraw mercs
                AllActiveMercenaries.Remove(merc);       // remove the relevant merc from the list          
                foreach (Monster OvertakenByBloodlust in activeRoom.ActiveMonsters)
                {
                    OvertakenByBloodlust.Morale += 1;
                }
                mob.Morale += 1;
                foreach (Mercenary discouraged in AllActiveMercenaries)
                {
                    discouraged.Morale -= 1;
                }
                DrawTick();
            }
            else
            {
                MobTick(true);
            }
        }

    }
}
