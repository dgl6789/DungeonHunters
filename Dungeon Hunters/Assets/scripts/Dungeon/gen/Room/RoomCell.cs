using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCell : MonoBehaviour {
    public int Height; // Displays how high the floor is from the cieling, for use in relative terraign generation.
    public bool Passable;// used for navigation
    public int Decoration; // used for display/World Gen
    public int FluidType = 0;
    public int LootType = 0;    
    public Vector2Int Gridlocation; // Simply its absolute coordinates within grid/hex system.
    public GameObject cubeTemp;
    public DungeonCamera DGcam;
    [SerializeField] private BoxCollider tileCollider;

	// Use this for initialization
	void Start () {
        tileCollider = gameObject.GetComponent<BoxCollider>();
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseDown()
    {
        //Add enums here for movement and attack. This is gonna be the basis for selecting options.

        DGcam.SetTargetPosition(gameObject.transform.position);
    }

    public void IncrimentHieght(int deltaHeight, int incColor)
    {
        Height = deltaHeight;
        Passable = true;
        cubeTemp.transform.Translate(new Vector3(0, 0, deltaHeight));
        cubeTemp.GetComponent<Renderer>().material.color = new Color(1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1);
    }

    public void RaiseTo(int deltaHeight, int incColor)
    {
        if(Height < deltaHeight)
        {
            Passable = true;
            Height = deltaHeight;
            cubeTemp.GetComponent<Renderer>().material.color = new Color(1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1.0f / 5.0f * Height, 1);
            cubeTemp.transform.position = new Vector3(cubeTemp.transform.position.x, cubeTemp.transform.position.y, Height);
        }
    }

    public void Reset()
    {
        Passable = false;
        Height = 0;
        cubeTemp.transform.position = new Vector3(cubeTemp.transform.position.x, cubeTemp.transform.position.y, Height);
        cubeTemp.GetComponent<Renderer>().material.color = Color.black;
    }

    public void AssignOreValue(int incValue){
        switch (incValue)
        {
            case 1://Copper?
                cubeTemp.GetComponent<Renderer>().material.color = Color.green;
                break;
            case 2://Tin?
                cubeTemp.GetComponent<Renderer>().material.color = Color.white;
                break;
            case 3://Strengthening Agent?
                cubeTemp.GetComponent<Renderer>().material.color = Color.magenta;
                break;
            case 4://Hardening Agent?
                cubeTemp.GetComponent<Renderer>().material.color = Color.magenta;
                break;
            case 5://Firmening Agent?
                cubeTemp.GetComponent<Renderer>().material.color = Color.magenta;
                break;
            case 6://Iron
                cubeTemp.GetComponent<Renderer>().material.color = Color.grey;
                break; 
            case 7://Silver
                cubeTemp.GetComponent<Renderer>().material.color = Color.clear;
                break;
            case 8://Gold
                cubeTemp.GetComponent<Renderer>().material.color = Color.yellow;
                break;
            case 9://Gem
                cubeTemp.GetComponent<Renderer>().material.color = Color.cyan;
                break;
            case 10://Aluminum?
                cubeTemp.GetComponent<Renderer>().material.color = Color.red;
                break;
        }
    }
}
