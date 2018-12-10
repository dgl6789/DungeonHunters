using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Data {
    [Serializable]
    public class EncounterContestAction : EncounterAction {
        public Skills Skill;

        public StatBlock Adversary;

        public override bool Success(StatBlock sb) {
            return RPGController.Contest(sb, Adversary, Skill);
        }
    }
}
