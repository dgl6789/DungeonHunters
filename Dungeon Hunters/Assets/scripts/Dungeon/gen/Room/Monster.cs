using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Attack
{
    public int Rating, Cohesion, baseDamage;
    public Vector3Int WeaponStats;
    public DamageType attackType;
    
}
public enum DamageType { Slash, Pierce, Crush}
public enum Stance { Offensive, Standard, Parry}

public class Monster : MonoBehaviour {
    public Vector2Int gridPosition;//Position in the grid
    public int Health, Stamina, Morale; // Basic stats- Health bounds the other 2, low stamina reduces combat effectiveness, and Low Morale leads to unit control loss
    public int pointBuy, MaxHealth=100, MaxStamina=50, MaxMorale=20, WeaponCohesion=100, ArmourCohesion=100;
    public Vector3Int defenseQuality, offenseQuality, Skills;//Quality of Defensive Armaments, Offensive Armaments, and skill therein    
    public bool isTemplate;//if this is a template creature, generate it, and if its not
    public bool CombatTest;
    public DamageType weaponType;
    public int MinRange, MaxRange;
    public Stance Style;
    GameObject model;
	// Use this for initialization
	void Start () {
        if (isTemplate)
        {//If we are to generate 
            int weaponTier =0, armourTier=0, highestSkill=0, temp;
            while(pointBuy > 3)
            {//while we still have points to spend
                temp = Random.Range(0, Mathf.Min(3, pointBuy));//pick a random number between 0 and either 3, or the points left
                weaponTier += temp;//Add it to the relevant 
                pointBuy -= temp;//decriment the amout of points left, and then do this process again for the other 2 stats
                MaxHealth += temp * 10;

                temp = Random.Range(0, Mathf.Min(3, pointBuy));
                armourTier += temp;
                pointBuy -= temp;
                MaxStamina += temp * 5;

                temp = Random.Range(0, Mathf.Min(3, pointBuy));
                highestSkill += temp;
                pointBuy -= temp;
                MaxMorale += temp * 2;
            }
            //All of our points have been allocated, time to interprit them.

            if (weaponTier > armourTier)
            {
                weaponType = DamageType.Slash;
                Style = Stance.Offensive;
            }
            else if (weaponTier % 2 == 0) {
                weaponType = DamageType.Pierce;
                Style = Stance.Parry;
            }
            else
            {
                weaponType = DamageType.Crush;
                Style = Stance.Standard;
            }                               

            switch (weaponTier)
            {//figure out what our weapons are made of, and assign their stats
                case 0://Weak Creature- Tooth and Nail
                    offenseQuality.x = 50; //Hardness, or ability to damage enemy armour
                    offenseQuality.y = 50; //Stength- ability to maintain form (not bend, or bend and then reform its shape)
                    offenseQuality.z = 75; // Toughness- ability to not shatter
                    break;
                case 1://medium Creature- Fang and Claw 
                    offenseQuality.x = 75;
                    offenseQuality.y = 50;
                    offenseQuality.z = 60;
                    break;
                case 2://Large Creature- Replacable Teeth and Long Claws 
                    offenseQuality.x = 85;
                    offenseQuality.y = 45;
                    offenseQuality.z = 60;
                    break;
                case 3://Copper - annealed?
                    offenseQuality.x = 100; //Copper is denser than natural weapons, but deforms easily
                    offenseQuality.y = 50;
                    offenseQuality.z = 90; //as a single metal that has been annealed, it is shatter-resistant
                    break;
                case 4://Bronze - Hard
                    offenseQuality.x = 150; //Hardened, but more brittle
                    offenseQuality.y = 70;
                    offenseQuality.z = 60; 
                    break;
                case 5://Bronze - Hard and Tough
                    offenseQuality.x = 145; //Hardened, but not brittle - but has a hard time keeping edge
                    offenseQuality.y = 60;
                    offenseQuality.z = 100;
                    break;
                case 6://Bronze - Superior
                    offenseQuality.x = 145; 
                    offenseQuality.y = 95;
                    offenseQuality.z = 95;
                    break;
                case 7://Wrought Iron - Quenched
                    offenseQuality.x = 175; 
                    offenseQuality.y = 100;
                    offenseQuality.z = 80;
                    break;
                case 8://Low Carbon Steel 
                    offenseQuality.x = 200;
                    offenseQuality.y = 120;
                    offenseQuality.z = 120;
                    break;
                default://Carbon Steel
                    offenseQuality.x = 225;
                    offenseQuality.y = 150;
                    offenseQuality.z = 150;
                    break;
            }

            switch (armourTier)
            {//figure out what our armour is made of, and assign their stats
                case 0://Weak Creature- Skin
                    defenseQuality.x = 0; //Hardness, or ability to damage enemy weapons
                    defenseQuality.y = 25; //Stength- ability to maintain form (not bend, or bend and then reform its shape)
                    defenseQuality.z = -1; // Toughness- ability to not shatter
                    break;
                case 1://medium Creature- Fur 
                    defenseQuality.x = 0;
                    defenseQuality.y = 50;
                    defenseQuality.z = -1;//Cannot be removed or destroyed
                    break;
                case 2://Large Creature- Scale
                    defenseQuality.x = 20;
                    defenseQuality.y = 50;
                    defenseQuality.z = 25;
                    break;
                case 3://Copper - annealed?
                    defenseQuality.x = 100; //Copper is denser than natural weapons, but deforms easily
                    defenseQuality.y = 50;
                    defenseQuality.z = 90; //as a single metal that has been annealed, it is shatter-resistant
                    break;
                case 4://Bronze - Hard
                    defenseQuality.x = 150; //Hardened, but more brittle
                    defenseQuality.y = 70;
                    defenseQuality.z = 60;
                    break;
                case 5://Bronze - Hard and Tough
                    defenseQuality.x = 145; //Hardened, but not brittle - but has a hard time keeping edge
                    defenseQuality.y = 60;
                    defenseQuality.z = 100;
                    break;
                case 6://Bronze - Superior
                    defenseQuality.x = 145;
                    defenseQuality.y = 95;
                    defenseQuality.z = 95;
                    break;
                case 7://Wrought Iron - Quenched
                    defenseQuality.x = 175;
                    defenseQuality.y = 100;
                    defenseQuality.z = 80;
                    break;
                case 8://Low Carbon Steel 
                    defenseQuality.x = 200;
                    defenseQuality.y = 120;
                    defenseQuality.z = 120;
                    break;
                default://Carbon Steel
                    defenseQuality.x = 225;
                    defenseQuality.y = 150;
                    defenseQuality.z = 150;
                    break;
            }

            Skills = new Vector3Int(highestSkill * 10, highestSkill * 10, highestSkill * 10);//Attck or Offense, Attack Negation, Reposte
            switch (Style)
            {
                case Stance.Offensive:
                    Skills.x = Skills.x * 2;//Twice the attack
                    Skills.y = Skills.y / 2;//Half the Defense, reposte Unchanged
                    break;
                case Stance.Standard:
                    Skills.x = (int)(Skills.x * 1.5f);//Attack and defense increased, reposte decreased
                    Skills.y = (int)(Skills.y * 1.5f);
                    Skills.z = Skills.z / 2;
                    break;
                case Stance.Parry:
                    Skills.x = Skills.x / 2;//half the attack
                    Skills.y = (int)(Skills.y * 1.5f);
                    Skills.z = Skills.z * 2;//twice the reposte
                    break;
            }


        }
        Health = MaxHealth;
        Stamina = MaxStamina;
        Morale = MaxMorale;
	}
	
	// Update is called once per frame
	void Update () {
        if (CombatTest)
        {
            CombatTest = false;
            Attack temp = GenerateAttack();
            Debug.Log(temp);
            RecieveAttack(temp);
        }

	}

    public Attack GenerateAttack()
    {
        Attack temp;
        temp.Rating = Skills.x + (Stamina-25);//Combine offensive skill with basic Stamina Bonus
        temp.WeaponStats = offenseQuality;
        temp.attackType = weaponType;
        temp.Cohesion = WeaponCohesion;
        temp.baseDamage = 5;
        return temp;
    }

    public void RecieveAttack(Attack incAttack)
    {//This is being kept exceedingly simple for now - but realistically there should be a quality modifier on weapons and armour.
        //differences in these ratings should decrease damage - regardless of what deforms - but decrease the quality of the artifact for its subsequent uses
        int IncDamage = incAttack.Rating - (Skills.y + (Stamina - 25));//Decrease the attack by our defense rating and stamina bonus
        IncDamage += (incAttack.WeaponStats.x - defenseQuality.x);//hardness difference- ability to not erode
        IncDamage += (incAttack.WeaponStats.y - defenseQuality.y);//Strength difference- ability for artifacts to not deform
        IncDamage += (incAttack.WeaponStats.z - defenseQuality.z);//toughness difference - ability for artifacts to not shatter
        Debug.Log(IncDamage);

        if (IncDamage > 0)
        {
            IncDamage = (int)(incAttack.baseDamage * (incAttack.Cohesion / 100.0f));//should probably switch to parabolic method later

            Debug.Log(IncDamage);
            if (IncDamage > 0)
            {
                Health -= IncDamage;
            }
        }
    }

}
