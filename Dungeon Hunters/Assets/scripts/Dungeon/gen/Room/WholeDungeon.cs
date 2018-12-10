using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dungeon;
using App;
using App.Data;


public class WholeDungeon : MonoBehaviour {
    enum DisplayState {None, Attack, Walk, Stance, Data}
    public bool interactablePhase = false;
    private DisplayState myDisplayState = 0;
    public int foodLeft;
    public GameObject RoomPrefab;
    public List<List<GameObject>> allRooms; // Pontentially not needed. Possibly a good idea to have though.
    public Room activeRoom;
    public DungeonHead landingRoom;
    bool inLandingPhase = true;
    public bool forTesting;
    public bool forwardBack;
    public Button [] NavigationButtons;
    public Button[] RoomEndButtons;
    public Button[] CampButtons;
    public Button NextTurn;
    public Text foodDisplay;
    public GameObject CharacterIndicator;
    public List<Mercenary> AllActiveMercenaries;
    public List<Monster> AllActiveMonsters;
    private List<GameObject> MercenaryDisplay;
    private List<GameObject> MonsterDisplay;
    public CharPortrait portrait;
    public GameObject MercenaryPrefab;
    public int ActiveMerc=0;
    public bool dirty= true;

	// Use this for initialization
	void Start () {
        CalcDirections();
        MercenaryDisplay = new List<GameObject>();
        MonsterDisplay = new List<GameObject>();
        for (int i = 4; i < 8; i++)
        {
            NavigationButtons[i].gameObject.SetActive(false);
        }       
        foreach(Mercenary merc in AllActiveMercenaries)
        {//ensure Mercenary stats are appropriately set.
            if (merc.MaxHealth < 10)
                merc.MaxHealth = 10;
            merc.Health = merc.MaxHealth;
            if (merc.MaxStamina < 10)
                merc.MaxStamina = 10;
            merc.Stamina = merc.MaxStamina;
            if (merc.MaxStamina < 10)
                merc.MaxStamina = 10;
            merc.Morale = merc.MaxMorale;
            MercenaryDisplay.Add(Instantiate(MercenaryPrefab));           
        }
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


        if (Input.GetKeyDown(KeyCode.O)){
            TakeDataFromOverworld();
        }

        if (Input.GetKeyDown(KeyCode.E) && interactablePhase)
        {
            UndrawTick();
            ActiveMerc--;
            if (ActiveMerc < 0)
                ActiveMerc = AllActiveMercenaries.Count - 1;
            dirty = true;
            portrait.Activate(ActiveMerc, AllActiveMercenaries[ActiveMerc]);
        }
        else if (Input.GetKeyDown(KeyCode.Q) && interactablePhase)
        {
            UndrawTick();
            ActiveMerc++;
            ActiveMerc = ActiveMerc % AllActiveMercenaries.Count;
            dirty = true;
            portrait.Activate(ActiveMerc, AllActiveMercenaries[ActiveMerc]);
        }
        else if (Input.GetKeyDown(KeyCode.W) && interactablePhase)
        {
            UndrawTick();
            myDisplayState = DisplayState.Walk;
            dirty = true;
        }
        else if (Input.GetKeyDown(KeyCode.A) && interactablePhase)
        {
            UndrawTick();
            myDisplayState = DisplayState.Attack;
            dirty = true;
        }
        else if (Input.GetKeyDown(KeyCode.S) && interactablePhase)
        {
            UndrawTick();
            myDisplayState = DisplayState.Stance;
            dirty = true;
        }
        else if (Input.GetKeyDown(KeyCode.D) && interactablePhase)
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

    public void TraverseRooms(bool isFoward,int Dir)
    {//Code to move from room to room    
        portrait.Activate(ActiveMerc, AllActiveMercenaries[ActiveMerc]);

        if (inLandingPhase) {            
            landingRoom.ClearRoom();
            inLandingPhase = false;

            switch (Dir)
            {                
                case 0://Navigate to the proper room if it exists, if it doesn't, create it then move to it.
                    if(landingRoom.NorthBranch != null)
                    {
                        activeRoom = landingRoom.NorthBranch;
                        activeRoom.RebuildCave();                        
                    }
                    else
                    {      
                        landingRoom.North = true;                  
                        Room Temp = Instantiate(RoomPrefab).GetComponent<Room>();
                        Temp.transform.SetParent(gameObject.transform);
                        Temp.CounterToEnd = Random.Range(3,3);
                        landingRoom.ExtendCave(AllActiveMonsters, 0,Temp); 
                        activeRoom = landingRoom.NorthBranch;
                    }
                    break;
                case 1:
                    if (landingRoom.EastBranch != null)
                    {
                        activeRoom = landingRoom.EastBranch;
                        activeRoom.RebuildCave();
                    }
                    else
                    {
                        landingRoom.East = true;
                        Room Temp = Instantiate(RoomPrefab).GetComponent<Room>();
                        Temp.transform.SetParent(gameObject.transform);
                        Temp.CounterToEnd = Random.Range(3,3);
                        landingRoom.ExtendCave(AllActiveMonsters, 1, Temp);
                        activeRoom = landingRoom.EastBranch;
                    }
                    break;
                case 2:
                    if (landingRoom.SouthBranch != null)
                    {
                        activeRoom = landingRoom.SouthBranch;
                        activeRoom.RebuildCave();
                    }
                    else
                    {
                        landingRoom.South = true;
                        Room Temp = Instantiate(RoomPrefab).GetComponent<Room>();
                        Temp.transform.SetParent(gameObject.transform);
                        Temp.CounterToEnd = Random.Range(3,3);
                        landingRoom.ExtendCave(AllActiveMonsters, 2, Temp);
                        activeRoom = landingRoom.SouthBranch;
                    }
                    break;
                case 3:
                    if (landingRoom.WestBranch != null)
                    {
                        activeRoom = landingRoom.WestBranch;
                        activeRoom.RebuildCave();
                    }
                    else
                    {
                        landingRoom.West = true;
                        Room Temp = Instantiate(RoomPrefab).GetComponent<Room>();
                        Temp.transform.SetParent(gameObject.transform);
                        Temp.CounterToEnd = Random.Range(3,3);
                        landingRoom.ExtendCave(AllActiveMonsters, 3, Temp);
                        activeRoom = landingRoom.WestBranch;
                    }
                    break;
            }
            activeRoom.OnRoomSwitch(true);
            activeRoom.PlaceMercs(true, true);
            forwardBack = true;

        }
        else
        {
            if (isFoward)
            {// if we are moving forward 
                UndrawTick();
                activeRoom.ClearRoom();
                if (activeRoom.nextRoom == null && activeRoom.CounterToEnd > 1)
                {//Regular forward room, new
                    Room Temp = Instantiate(RoomPrefab).GetComponent<Room>();
                    Temp.transform.SetParent(gameObject.transform);
                    Temp.CounterToEnd = activeRoom.CounterToEnd - 1;
                    Temp.DeclareToPreventErrors();
                    activeRoom.nextRoom = Temp;
                    activeRoom.ExtendCave(AllActiveMonsters, false);
                    activeRoom = activeRoom.nextRoom;
                    activeRoom.OnRoomSwitch(true);
                    activeRoom.PlaceMercs(true, true);
                    forwardBack = true;
                }
                else if (activeRoom.nextRoom == null && activeRoom.CounterToEnd == 1)
                {//last room in a branch
                    Room Temp = Instantiate(RoomPrefab).GetComponent<Room>();
                    Temp.transform.SetParent(gameObject.transform);
                    Temp.CounterToEnd = activeRoom.CounterToEnd - 1;
                    Temp.DeclareToPreventErrors();
                    activeRoom.nextRoom = Temp;
                    activeRoom.ExtendCave(AllActiveMonsters, true); activeRoom = activeRoom.nextRoom;
                    activeRoom.OnRoomSwitch(true);
                    activeRoom.PlaceMercs(true, true);
                    forwardBack = true;
                }
                else if (activeRoom.nextRoom != null)
                {//room already generated
                    activeRoom = activeRoom.nextRoom;
                    activeRoom.OnRoomSwitch(true);
                    activeRoom.PlaceMercs(true, true);
                    forwardBack = true;
                }
                else
                {//retrun to landing after clearing final room.
                    UndrawTick();
                    activeRoom.ClearRoom();
                    activeRoom = null;
                    landingRoom.RebuildCave();
                    inLandingPhase = true;
                    forwardBack = false;
                }
            }
            else
            {
                if (activeRoom.previousRoom != null)
                {//if we have  backward to move to.
                    UndrawTick();
                    activeRoom.ClearRoom();
                    activeRoom = activeRoom.previousRoom;
                    activeRoom.RebuildCave();
                    activeRoom.OnRoomSwitch(false);
                    activeRoom.PlaceMercs(false, true);
                    forwardBack = false;

                }
                else {//if we are returning to the landing zone, do so.
                    UndrawTick();
                    activeRoom.ClearRoom();
                    activeRoom = null;
                    landingRoom.RebuildCave();
                    inLandingPhase = true;
                    forwardBack = false;
                }
            }
        }
        UndrawTick();
        CalcDirections();
    }

    void CalcDirections()
    {
        if (inLandingPhase)
        {
            foreach (Button Butt in RoomEndButtons)
            {
                Butt.gameObject.SetActive(false);
                NextTurn.gameObject.SetActive(false);
                foodDisplay.text = foodLeft.ToString();                
            }
            for (int i = 4; i < 8; i++)
            {
                NavigationButtons[i].gameObject.SetActive(false);
            }
            if (!landingRoom.North)
            {
                NavigationButtons[0].gameObject.SetActive(true);
            }
            if (!landingRoom.East)
            {
                NavigationButtons[1].gameObject.SetActive(true);
            }
            if (!landingRoom.South)
            {
                NavigationButtons[2].gameObject.SetActive(true);
            }
            if (!landingRoom.West)
            {
                NavigationButtons[3].gameObject.SetActive(true);
            }

            if(landingRoom.West && landingRoom.East && landingRoom.North && landingRoom.South)
            {//Dungeon Completed.

                SceneSwitcher.Instance.EnableScene("Overworld");
                Debug.Log("Dungeon Completed");
                SceneSwitcher.Instance.DisableScene("Dungeon");
            }
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                NavigationButtons[i].gameObject.SetActive(false);
            }            
            if (activeRoom.ActiveMonsters.Count <= 0)//If we are Entering Next Turn.
            {
                CharacterIndicator.gameObject.SetActive(false);
                foreach(GameObject obby in MercenaryDisplay)
                {
                    obby.SetActive(false);
                }
                ActiveMerc = 0;
                NextTurn.gameObject.SetActive(false);
                foreach (Button Butt in RoomEndButtons)
                {
                    Butt.gameObject.SetActive(true);
                    foodDisplay.text = foodLeft.ToString();
                    portrait.Deactivate();
                    interactablePhase = false;
                    
                }
            }
            else
            {
                NextTurn.gameObject.SetActive(true);                
                foreach (Button Butt in RoomEndButtons)
                {
                    Butt.gameObject.SetActive(false);
                    foodDisplay.text = foodLeft.ToString();
                    
                }
            }
        }
    }

    void CharacterTick(bool activating)
    {
        if (activeRoom != null)
        {
            if (activating)
            {
                foreach (Mercenary merc in AllActiveMercenaries)
                {
                    activeRoom.HighLightZones(1, merc.gridPosition, 0, 0, 0);
                }
            }
            else
            {
                foreach (Mercenary merc in AllActiveMercenaries)
                {
                    activeRoom.HighLightZones(0, merc.gridPosition, 0, 0, 0);
                }
            }
        }
    }

    void CharacterTick(int Index, bool Active)
    {//Draws or undraws a single character
        if (Active)
        {
            activeRoom.HighLightZones(1, AllActiveMercenaries[Index].gridPosition, 0, 0,0);
        }
        else
        {
            activeRoom.HighLightZones(0, AllActiveMercenaries[Index].gridPosition, 0, 0,0);
        }
    }

    void MovementTick(bool activating)
    {//Draws or removes the movement selector
        if (activating)
        {
            activeRoom.HighLightZones(2, AllActiveMercenaries[ActiveMerc].gridPosition, 1, AllActiveMercenaries[ActiveMerc].Movement,0);           
        }
        else
        {            
           activeRoom.HighLightZones(0, AllActiveMercenaries[ActiveMerc].gridPosition, 0, AllActiveMercenaries[ActiveMerc].Movement,0);            
        }
    }
    
    void RangeTick(bool activating)
    {//draws a character's weapon range- to be used to select targets.
        if (activating)
        {
            if(AllActiveMercenaries[ActiveMerc].Movement> 0)//can't attack with 0 movement.
                activeRoom.HighLightZones(3, AllActiveMercenaries[ActiveMerc].gridPosition, 3, AllActiveMercenaries[ActiveMerc].Movement,0);
        }
        else
        {
            activeRoom.HighLightZones(0, AllActiveMercenaries[ActiveMerc].gridPosition, 3, AllActiveMercenaries[ActiveMerc].Movement,0);
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
            MercenaryDisplay[ActiveMerc].transform.position = new Vector3(index.x / 2.0f, index.y / 2.0f, 1f);
            AllActiveMercenaries[ActiveMerc].Movement -= totalMovement;
            DrawTick();
        }
    }
    public void PlaceActiveMerc(Vector2Int index)
    {
        if (ActiveMerc != -1)
        {//ensure that we aren't moving a character that can't be moved           
            AllActiveMercenaries[ActiveMerc].gridPosition = index;
            MercenaryDisplay[ActiveMerc].transform.position = new Vector3(index.x / 2.0f, index.y / 2.0f, 1f);
            CharacterTick(ActiveMerc, true);
            MercenaryDisplay[ActiveMerc].SetActive(true);
            ActiveMerc++;
            if (ActiveMerc == AllActiveMercenaries.Count)//if we are done moving mercenaries
            {
                ActiveMerc = 0;
                activeRoom.PlaceMercs(forwardBack, false);
                dirty = true;
                interactablePhase = true;
                MercenaryDisplay[ActiveMerc].SetActive(true);
                CharacterIndicator.gameObject.SetActive(true);
            }
        }
    }

    public void MobTick(bool activating)
    {
        if (activeRoom != null)
        {
            if (activating)
            {
                foreach (Monster mob in activeRoom.ActiveMonsters)
                {
                    activeRoom.HighLightZones(4, mob.gridPosition, 0, 0, 0);
                }
            }
            else
            {
                foreach (Monster mob in activeRoom.ActiveMonsters)
                {
                    activeRoom.HighLightZones(0, mob.gridPosition, 0, 0, 0);
                }
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
                int i = 0;
                MovementTick(true);
                foreach (Monster Mob in activeRoom.ActiveMonsters)
                {
                    activeRoom.HighLightZones(6, Mob.gridPosition, 1, 2, i);
                    i++;
                }
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
                    activeRoom.HighLightZones(7, Merc.gridPosition, 1, 2,0);
                }
                int j = 0;
                foreach (Monster Mob in activeRoom.ActiveMonsters)
                {
                    activeRoom.HighLightZones(6, Mob.gridPosition, 1, 2, j);
                    j++;
                }
                CharacterTick(true);
                MobTick(true);
                break;
            default:               
                MobTick(true);
                break;
        }
        CharacterTick(true);
        CharacterIndicator.transform.position = new Vector3(AllActiveMercenaries[ActiveMerc].gridPosition.x/2.0f, AllActiveMercenaries[ActiveMerc].gridPosition.y/2.0f, 1);
        portrait.Tick();
    }

    void UndrawTick()
    {
        switch (myDisplayState)
        {
            case DisplayState.None:
                break;
            case DisplayState.Walk:
                if (activeRoom != null)
                {
                    MovementTick(false);
                    foreach (Monster Mob in activeRoom.ActiveMonsters)
                    {
                        activeRoom.HighLightZones(0, Mob.gridPosition, 0, 2,-1);
                    }
                }
                break;
            case DisplayState.Attack:
                if(activeRoom!= null)
                activeRoom.HighLightTargets(AllActiveMercenaries[ActiveMerc].gridPosition, 2, 5, false);
                
                break;
            case DisplayState.Stance:
                if (activeRoom != null)
                {
                    foreach (Mercenary Merc in AllActiveMercenaries)
                    {
                        activeRoom.HighLightZones(0, Merc.gridPosition, 1, 2,-1);
                    }
                    foreach (Monster Mob in activeRoom.ActiveMonsters)
                    {
                        activeRoom.HighLightZones(0, Mob.gridPosition, 1, 2,-1);
                    }
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
                if(mob.pointBuy > 5)
                {
                    foodLeft += (mob.pointBuy / 5);
                }
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
                Destroy( MercenaryDisplay[AllActiveMercenaries.IndexOf(merc)]);
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

    public void MoveRooms( int Index) {//Just a linkup for the scene editor, which decided to not be co-operative.
        if(Index < 0)
        {
            TraverseRooms(false, Mathf.Abs(Index));
        }
        else
        {
            TraverseRooms(true, Index);
        }
       

    }

    public void LootAndLeave()
    {
       
        foreach (Button Butt in RoomEndButtons)
        {
            Butt.gameObject.SetActive(false);
            foodDisplay.text = foodLeft.ToString();
            
        }
        SceneSwitcher.Instance.EnableScene("Overworld");
        Debug.Log("Left With Loot");
        SceneSwitcher.Instance.DisableScene("Dungeon");
        
    }

    public void CampPhase()
    {
        Debug.Log("Entering Camp Phase");
        portrait.Deactivate();
        foreach (Button Butt in RoomEndButtons)
        {//Disable previous set of buttons
            Butt.gameObject.SetActive(false);
            foodDisplay.text = foodLeft.ToString();            
        }
        Text[] Stats = new Text[3];
        for (int i=0; i < AllActiveMercenaries.Count; i++)
        {//Enable new set of buttons, based on number of mercs active. (or alive) 

            CampButtons[i].gameObject.SetActive(true);
            Stats = CampButtons[i].GetComponentsInChildren<Text>();
            Stats[0].text = (AllActiveMercenaries[i].Health + "/" + AllActiveMercenaries[i].MaxHealth);
            Stats[1].text = (AllActiveMercenaries[i].Stamina + "/" + AllActiveMercenaries[i].MaxStamina);
            Stats[2].text = (AllActiveMercenaries[i].Morale + "/" + AllActiveMercenaries[i].MaxMorale);

        }


    }
    public void LeaveCampPhase()
    {
        foreach(Button butt in CampButtons)
        {
            butt.gameObject.SetActive(false);
        }
        ActiveMerc = 0;
        MoveRooms(1);
    }

    public void DesignateWatchman(int index)
    {
        foodLeft -= AllActiveMercenaries.Count;
        foodDisplay.text = foodLeft.ToString();        
        AllActiveMercenaries[index].Stamina -= 10;//reduce stamina for guard merc
        Text[] Stats = new Text[3];
        foreach (Mercenary merc in AllActiveMercenaries)//Increase resting stats for all resting mercs, and the guard merc.
        {
            merc.Health += merc.MaxHealth / 10;          
            merc.Stamina += 5;          
            merc.Morale += 1;

            merc.Health = Mathf.Min(merc.Health, merc.MaxHealth);
            merc.Stamina = Mathf.Min(merc.Stamina, merc.MaxStamina);
            merc.Morale = Mathf.Min(merc.Morale, merc.MaxMorale);
        }
        for (int i = 0; i < AllActiveMercenaries.Count; i++)        {//Enable new set of buttons, based on number of mercs active. (or alive) 

          
            Stats = CampButtons[i].GetComponentsInChildren<Text>();
            Stats[0].text = (AllActiveMercenaries[i].Health + "/" + AllActiveMercenaries[i].MaxHealth);
            Stats[1].text = (AllActiveMercenaries[i].Stamina + "/" + AllActiveMercenaries[i].MaxStamina);
            Stats[2].text = (AllActiveMercenaries[i].Morale + "/" + AllActiveMercenaries[i].MaxMorale);

        }
        if (foodLeft < AllActiveMercenaries.Count)
        {
            LeaveCampPhase();
            MoveRooms(1);            
        }
    }

    public void StanceSwitch(int incStyle)
    {
        if (AllActiveMercenaries[ActiveMerc].Movement > 0 && (int)AllActiveMercenaries[ActiveMerc].Style != incStyle)
        {//if we have any movement, and we are actually changing, subtract a movement, and change that style.
            UndrawTick();
            AllActiveMercenaries[ActiveMerc].Style = (Stance)incStyle;
            Debug.Log("Switching to stance " + (Stance)incStyle);
            AllActiveMercenaries[ActiveMerc].Movement -= 1;
            DrawTick();
        }
    }
    public void HUDInput(int buttonNumber)
    {
        switch (buttonNumber)
        {
            case 1:
                UndrawTick();
                ActiveMerc--;
                if (ActiveMerc < 0)
                    ActiveMerc = AllActiveMercenaries.Count - 1;
                dirty = true;
                portrait.Activate(ActiveMerc, AllActiveMercenaries[ActiveMerc]);
                break;
            case 2:
                UndrawTick();
                myDisplayState = DisplayState.Attack;
                dirty = true;
                break;
            case 3:
                UndrawTick();
                myDisplayState = DisplayState.Walk;
                dirty = true;
                break;
            case 4:
                UndrawTick();
                ActiveMerc++;
                ActiveMerc = ActiveMerc % AllActiveMercenaries.Count;
                dirty = true;
                portrait.Activate(ActiveMerc, AllActiveMercenaries[ActiveMerc]);
                break;

        }
    }

    public void TakeDataFromOverworld()
    {
        MercenaryData temp = EncounterController.Instance.ActiveMercenary;
        Debug.Log(temp.Stats.Body);
    }
}
