using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class Mercenary : MonoBehaviour {

        /// Dungeon Members
        /// 

        public Vector2Int gridPosition;//Position in the grid
        public int Health, Stamina, Morale, WeaponCohesion, ArmourCohesion, Movement;
        public int MaxHealth, MaxStamina, MaxMorale, Body, Mind, Spirit, DTier, OTier;
        public Vector3Int defenseQuality, offenseQuality, Skills;//Quality of Defensive Armaments, Offensive Armaments, and skill therein
        public DamageType weaponType;
        public Stance Style;
        public bool Recalc;
        // Use this for initialization
        void Start() {
        RecalcStats();
        RecalcSkills();


            

        }

        // Update is called once per frame
        void Update() {
            if (Recalc) {
                Recalc = false;
                RecalcStats();
            }

        }

        public Attack GenerateAttack()
        {
            Attack temp;
            temp.Rating = Skills.x + (Stamina - 25);//Combine offensive skill with basic Stamina Bonus
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

        void RecalcStats()//Generate Stats on the 
        {
            //set base combat stats on char stats
            MaxHealth = 50 + Body * 10;
            MaxStamina = 25 + (Body + Spirit) * 5;
            MaxMorale = 10 + (Spirit + Mind) * 2;

            //assign weapon and armour materials based on the tier of the weapons and armour
            switch (DTier) {//figure out what our armour is made of, and assign their stats
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
            switch (OTier) {//figure out what our weapons are made of, and assign their stats
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

            RecalcSkills();
        }
        void RecalcSkills() {
            Skills = new Vector3Int((Body * 2 + Mind) * 10, (Body + Mind) * 20, (Body + Mind * 2) * 10);
            switch (Style) {
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
    }

