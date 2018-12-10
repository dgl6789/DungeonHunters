using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Data {
    [Serializable]
    public class EncounterCheckAction : EncounterAction {
        public Skills Skill;
        public Difficulty CheckDifficulty;

        public override bool Success(StatBlock sb) {
            return RPGController.SkillCheck(sb, Skill, CheckDifficulty);
        }
    }
}
