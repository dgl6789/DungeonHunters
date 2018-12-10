using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App.Data;

namespace App {
    public enum Difficulty { Trivial, Normal, Hard, Impossible }
    public enum Attributes { Body, Mind, Spirit }
    public enum Skills { Constitution, Endurance, Stealth, Medicine, Athletics, Knowledge, Smithing, Magic, Survival, Insight, Coercion, Deception, Intimidation, Performance, Spot }

    [System.Serializable]
    public class Skill : ScriptableObject {
        [SerializeField] public Skills Name;
        [SerializeField] private int bonus;
        public int Bonus {
            get { return bonus; }
            set { bonus = value; }
        }

        public Skill(Skills pName, int pBonus) {
            Name = pName;
            bonus = pBonus;
        }
    }

    [System.Serializable]
    public class Attribute : ScriptableObject {
        [SerializeField] public Attributes Name;
        [SerializeField] public int score;
        public int Score {
            get { return score; }
            set { score = value; }
        }

        public Attribute(Attributes pName, int pScore) {
            Name = pName;
            Score = pScore;
        }
    }

    public static class RPGController {

        /// <summary>
        /// Simulate a skill-based check against a default difficulty class.
        /// </summary>
        /// <param name="pStats">Stat block of the character making the check.</param>
        /// <param name="skill">Skill to make a check with.</param>
        /// <param name="difficulty">Difficulty classification of the check.</param>
        /// <returns>True if the check was greater than or equal to the difficulty class, and false otherwise.</returns>
        public static bool SkillCheck(StatBlock pStats, Skills skill, Difficulty difficulty) {
            return SkillCheck(pStats, skill, GetDifficultyClass(difficulty));
        }

        /// <summary>
        /// Simulate a skill-based check against a flat difficulty.
        /// </summary>
        /// <param name="pStats">Stat block of the character making the check.</param>
        /// <param name="skill">Skill to make a check with.</param>
        /// <param name="difficulty">Difficulty of the check.</param>
        /// <returns>True if the check was greater than or equal to the difficulty class, and false otherwise.</returns>
        public static bool SkillCheck(StatBlock pStats, Skills skill, int difficulty) {
            return Roll(pStats, skill) >= difficulty;
        }

        /// <summary>
        /// Simulate a skill-based contest between two characters with statblocks.
        /// </summary>
        /// <param name="a">Statblock of character A</param>
        /// <param name="b">Statblock of character B</param>
        /// <param name="skill">Skill to perform the contest with</param>
        /// <returns>True if character A wins, and false otherwise.</returns>
        public static bool Contest(StatBlock a, StatBlock b, Skills skill) {
            int totalA = 0;
            int totalB = 0;

            do {
                totalA = Roll(a, skill);
                totalB = Roll(b, skill);
            } while (totalA == totalB);

            return totalA > totalB;
        }

        static int Roll(StatBlock stats, Skills skill) {
            int total = 0;

            // Roll a number of dice equal to the derivative attribute of the skill.
            // Replace a number of d4 rolls in the total with d8 rolls.
            // Avg of 4d4 = 10, Avg of 2d4 + 2d8 = 14, for example

            int attributeScore = stats.GetAttributeScoreFor(GetSkillBaseAttribute(skill));

            for (int i = 0; i < attributeScore; i++) {
                if(i <= stats.GetSkillScore(skill)) {
                    total += Random.Range(1, 9);
                } else {
                    total += Random.Range(1, 5);
                }
            }

            return total;
        }

        public static List<Skills> BodySkills {
            get {
                return new List<Skills>() {
                    Skills.Constitution, Skills.Endurance, Skills.Stealth, Skills.Medicine, Skills.Athletics };
            }
        }

        public static List<Skills> MindSkills {
            get {
                 return new List<Skills>() {
                     Skills.Knowledge, Skills.Smithing, Skills.Magic, Skills.Survival, Skills.Insight };
            }
        }

        public static List<Skills> SpiritSkills {
            get {
                return new List<Skills>() {
                Skills.Coercion, Skills.Deception, Skills.Intimidation, Skills.Performance, Skills.Spot };
            }
        }

        public static Attributes GetSkillBaseAttribute(Skills pSkill) {
            if (BodySkills.Contains(pSkill)) return Attributes.Body;
            if (MindSkills.Contains(pSkill)) return Attributes.Mind;
            return Attributes.Spirit;
        }

        public static int GetDifficultyClass(Difficulty difficulty) {
            switch(difficulty) {
                case Difficulty.Trivial: return 5;
                default:
                case Difficulty.Normal: return 10;
                case Difficulty.Hard: return 15;
                case Difficulty.Impossible: return 20;
            }
        }
    }
}
