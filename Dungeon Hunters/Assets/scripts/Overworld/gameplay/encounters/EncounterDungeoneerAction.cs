using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Data
{
    [Serializable]
    public class EncounterDungeoneerAction : EncounterAction
    {
        public override bool Success(StatBlock sb)
        {
            SceneSwitcher.Instance.EnableScene("Dungeon");

            // call that method we made in WholeDungeon.cs
            List<MercenaryData> mercList = new List<MercenaryData>();
            mercList.Add(EncounterController.Instance.ActiveMercenary);                    

            SceneSwitcher.Instance.dungeon.gameObject.GetComponentInChildren<WholeDungeon>().TakeDataFromOverworld(mercList);
            

            SceneSwitcher.Instance.DisableScene("Overworld");
            return true;
        }
    }
}
