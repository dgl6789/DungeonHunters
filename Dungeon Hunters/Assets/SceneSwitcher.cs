using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour {

    public static SceneSwitcher Instance;
    [HideInInspector] public bool overWorldActive;
    
    [SerializeField] Transform overworld;
    [SerializeField] Transform dungeon;


	// Use this for initialization
	void Awake () {
        if (Instance == null) Instance = this;
        else Destroy(this);
	}

    public void EnableScene(string scene)
    {
        switch(scene)
        {
            case "Overworld": overworld.gameObject.SetActive(true); overWorldActive = true; return;
            case "Dungeon": dungeon.gameObject.SetActive(true); return;
        }
    }

    public void DisableScene(string scene)
    {
        switch (scene)
        {
            case "Overworld": overworld.gameObject.SetActive(false); overWorldActive = false; return;
            case "Dungeon": dungeon.gameObject.SetActive(false); return;
        }
    }
}
