using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileState {None, Ally, MovementSelector, AttackSelector, Enemy }

public class RoomCell : MonoBehaviour {
    public int Height; // Displays how high the floor is from the cieling, for use in relative terraign generation.
    public bool Passable;// used for navigation
    public int Decoration; // used for display/World Gen
    public int FluidType = 0;
    public int LootType = 0;    
    public Vector2Int Gridlocation; // Simply its absolute coordinates within grid/hex system.
    public GameObject cubeTemp;
    public DungeonCamera DGcam;
    [SerializeField] private WholeDungeon myDungeon;
    public TileState Mystate;
    [SerializeField] private BoxCollider2D tileCollider;

	// Use this for initialization
	void Start () {
        tileCollider = gameObject.GetComponent<BoxCollider2D>();
        

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
                cubeTemp.GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case TileState.AttackSelector:
                cubeTemp.GetComponent<SpriteRenderer>().color = Color.red;
                break;

        }
		
	}

    private void OnMouseDown()
    {
        //Add enums here for movement and attack. This is gonna be the basis for selecting options.

        DGcam.SetTargetPosition(gameObject.transform.position);
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
                    Debug.Log("This is where an attack should be");
                    myDungeon.MobTick();
                    break;
                default:
                    break;
            }
        }
    }
    

    public void IncrimentHieght(int deltaHeight, int incColor)
    {
        Height = deltaHeight;
        Passable = true;
        cubeTemp.transform.Translate(new Vector3(0, 0, deltaHeight));
        cubeTemp.GetComponent<SpriteRenderer>().material.color = new Color(1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1);
    }

    public void RaiseTo(int deltaHeight, int incColor)
    {
        if(Height < deltaHeight)
        {
            Passable = true;
            Height = deltaHeight;
            cubeTemp.GetComponent<SpriteRenderer>().material.color = new Color(1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1);
            cubeTemp.transform.position = new Vector3(cubeTemp.transform.position.x, cubeTemp.transform.position.y, Height);
        }
    }

    public void Reset()
    {
        Passable = false;
        Height = 0;
        cubeTemp.transform.position = new Vector3(cubeTemp.transform.position.x, cubeTemp.transform.position.y, Height);
        cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.black;
    }

    public void AssignOreValue(int incValue){
        switch (incValue)
        {
            case 1://Copper?
                cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.green;
                break;
            case 2://Tin?
                cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.white;
                break;
            case 3://Strengthening Agent?
                cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.magenta;
                break;
            case 4://Hardening Agent?
                cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.magenta;
                break;
            case 5://Firmening Agent?
                cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.magenta;
                break;
            case 6://Iron
                cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.grey;
                break; 
            case 7://Silver
                cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.clear;
                break;
            case 8://Gold
                cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.yellow;
                break;
            case 9://Gem
                cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.cyan;
                break;
            case 10://Aluminum?
                cubeTemp.GetComponent<SpriteRenderer>().material.color = Color.red;
                break;
        }
    }
}
