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
            Debug.Log(EncounterController.Instance.ActiveMercenary);

            SceneSwitcher.Instance.DisableScene("Overworld");
            return true;
        }
    }
}
