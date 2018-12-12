using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;

public enum TileState {None, Ally, MovementSelector, AttackSelector, Enemy , AttackRange, ThreatenedByFoe, ThreatenedByFriend, PlacementSelector, MoveAndTriggerAttack }

public class RoomCell : MonoBehaviour {
    public int Height; // Displays how high the floor is from the cieling, for use in relative terraign generation.
    public bool Passable;// used for navigation
    public int Decoration, River, RiverIndex, RiverDistance; // used for display/World Gen
    public int MonsterThreatening;
    public Vector2Int Gridlocation; // Simply its absolute coordinates within grid/hex system.
    public GameObject cubeTemp;
    [SerializeField] private WholeDungeon myDungeon;
    public TileState Mystate;
    [SerializeField] private BoxCollider2D tileCollider;
    [SerializeField] private Sprite OreSpritePrefab;
    private GameObject OreTile;

    // Use this for initialization
    void Start () {
        tileCollider = gameObject.GetComponent<BoxCollider2D>();
        MonsterThreatening = -1;
        

    }
	
	// Update is called once per frame
	void Update () {
        switch (Mystate)
        {
            case TileState.None:
                cubeTemp.GetComponent<SpriteRenderer>().color = new Color(1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1);
                break;
            case TileState.Enemy:
                cubeTemp.GetComponent<SpriteRenderer>().color = Color.magenta;
                break;
            case TileState.Ally:
                cubeTemp.GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case TileState.MovementSelector:
            case TileState.PlacementSelector:
                cubeTemp.GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case TileState.AttackSelector:
                cubeTemp.GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case TileState.AttackRange:
                cubeTemp.GetComponent<SpriteRenderer>().color = new Color(1,.45f,.45f);
                break;
            case TileState.ThreatenedByFoe:
                cubeTemp.GetComponent<SpriteRenderer>().color = new Color(.70f, .4f, 1);
                break;
            case TileState.ThreatenedByFriend:
                cubeTemp.GetComponent<SpriteRenderer>().color = new Color(.4f, .76f, 1);
                break;
            case TileState.MoveAndTriggerAttack:
                cubeTemp.GetComponent<SpriteRenderer>().color = Color.red;
                break;
        }
		
	}

    private void OnMouseDown()
    {
        //Add enums here for movement and attack. This is gonna be the basis for selecting options.

        Camera.main.GetComponent<OverworldCamera>().SetTargetPosition(gameObject.transform.position);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {//If there is a right click
            switch (Mystate)
            {
                case TileState.MovementSelector://if we are moving, run the code to move stuff
                    myDungeon.MoveActiveMerc(Gridlocation);
                    break;
                case TileState.AttackSelector://if we are doing an attack, first turn off the color for here, and then call the attack script, then redraw all monsters.
                    Mystate = TileState.None;
                    Monster temp = myDungeon.GetMobFromLoc(Gridlocation);
                    myDungeon.RunAttack(myDungeon.AllActiveMercenaries[myDungeon.ActiveMerc], temp, true);                                     
                    break;
                case TileState.PlacementSelector://if we are Placeing mercs, place em down. This is different from moving, becasue there are no penalties.
                    myDungeon.PlaceActiveMerc(Gridlocation);
                    break;              
                case TileState.MoveAndTriggerAttack://if we are moving, run the code to move stuff
                    myDungeon.MoveActiveMerc(Gridlocation);
                    Debug.Log("Attack of Opportunity launched");
                    break;
                default:
                    break;
            }
        }
    }
    

    public void IncrimentHeight(int deltaHeight, int incRiver, int incRiverIndex, int incRiverDistance)
    {
        River = incRiver;
        RiverIndex = incRiverIndex;
        RiverDistance = incRiverDistance;
        Height = deltaHeight;
        Passable = true;
        cubeTemp.transform.Translate(new Vector3(0, 0, deltaHeight));
        cubeTemp.GetComponent<SpriteRenderer>().material.color = new Color(1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1);
    }

    public bool RaiseTo(int deltaHeight, int incRiver, int incRiverIndex, int incRiverDistance)
    {
        if (Height < deltaHeight)
        {
           
                River = incRiver;
                RiverIndex = incRiverIndex;
                RiverDistance = incRiverDistance;
                Passable = true;
                Height = deltaHeight;
                cubeTemp.GetComponent<SpriteRenderer>().material.color = new Color(1.5f + 1.0f / 20.0f * Height, 1.5f + 1.0f / 20.0f * Height, 1.5f + 1.0f / 20.0f * Height, 1);
                cubeTemp.transform.position = new Vector3(cubeTemp.transform.position.x, cubeTemp.transform.position.y, 5+Height);
                return true;
        }
        else if (Height == deltaHeight)
        {
            if (incRiverDistance < RiverDistance)
            {
                River = incRiver;
                RiverIndex = incRiverIndex;
                RiverDistance = incRiverDistance;
                Passable = true;
                Height = deltaHeight;
                cubeTemp.GetComponent<SpriteRenderer>().material.color = new Color(1.5f +  1.0f / 20.0f * Height, 1.5f + 1.0f / 20.0f * Height, 1.5f + 1.0f / 20.0f * Height, 1);
                cubeTemp.transform.position = new Vector3(cubeTemp.transform.position.x, cubeTemp.transform.position.y, 5+Height);
                return true;
            }
        }
        return false;
    }

    public void AddMode(int IncMod)
    {
        //if(!(Mystate == TileState.ThreatenedByFoe && IncMod == (int)TileState.MovementSelector ) && !(Mystate == TileState.MovementSelector && IncMod == (int)TileState.ThreatenedByFoe))
        {
            Mystate = (TileState)IncMod;
        }
       // else
        {
       //     Mystate = TileState.MoveAndTriggerAttack;
        }
    }

    public void Reset()
    {
        Passable = false;
        Height = 0;
        cubeTemp.transform.position = new Vector3(cubeTemp.transform.position.x, cubeTemp.transform.position.y, Height);
        cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.black;
    }

    //Temporarily disabled for new sprites
    public void AssignOreValue(int incValue){
        if (incValue >= 1)
        {
            if(OreTile == null)
            {
                
                OreTile = new GameObject();
                OreTile.transform.localScale = transform.localScale;
                OreTile.transform.parent = gameObject.transform;
                OreTile.transform.localPosition = new Vector3(0, 0, -.2f);
                OreTile.AddComponent<SpriteRenderer>();
                OreTile.GetComponent<SpriteRenderer>().sprite = OreSpritePrefab;

            }
            switch (incValue)
            {
                case 1://Copper?
                    OreTile.GetComponent<SpriteRenderer>().material.color = Color.green;
                    break;
                case 2://Tin?
                    OreTile.GetComponent<SpriteRenderer>().material.color = Color.white;
                    break;
                case 3://Strengthening Agent?
                    OreTile.GetComponent<SpriteRenderer>().material.color = Color.magenta;
                    break;
                case 4://Hardening Agent?
                    OreTile.GetComponent<SpriteRenderer>().material.color = Color.magenta;
                    break;
                case 5://Firmening Agent?
                    OreTile.GetComponent<SpriteRenderer>().material.color = Color.magenta;
                    break;
                case 6://Iron
                    OreTile.GetComponent<SpriteRenderer>().material.color = Color.grey;
                    break;
                case 7://Silver
                    OreTile.GetComponent<SpriteRenderer>().material.color = Color.clear;
                    break;
                case 8://Gold
                    OreTile.GetComponent<SpriteRenderer>().material.color = Color.yellow;
                    break;
                case 9://Gem
                    OreTile.GetComponent<SpriteRenderer>().material.color = Color.cyan;
                    break;
                case 10://Aluminum?
                    OreTile.GetComponent<SpriteRenderer>().material.color = Color.red;
                    break;
            }
        }
        else
        {
            if(OreTile != null)
            {
                Destroy(OreTile);
            }
        }
    }
}
