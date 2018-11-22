using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharPortrait : MonoBehaviour {
    public Mercenary currentMerc;
    public Image[] baseImages;
    public Sprite[] baseSprites;
    public Text[] stats;
    public Button[] stanceButtons;
    WholeDungeon myDungeon;
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void Activate(int characterModel, Mercenary incMerc)
    {
        gameObject.SetActive(true);
        baseImages[0].sprite = baseSprites[characterModel];
        currentMerc = incMerc;
        Tick();
    }

    public void Tick() {
        stats[0].text = currentMerc.Health.ToString() + " / " + currentMerc.MaxHealth.ToString();
        stats[1].text = currentMerc.Stamina.ToString() + " / " + currentMerc.MaxStamina.ToString();
        stats[2].text = currentMerc.Morale.ToString() + " / " + currentMerc.MaxMorale.ToString();
    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
}
