using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using App;

namespace App.Data {
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Statblock", menuName = "Data/Statblock", order = 6)]
    public class StatBlock : ScriptableObject {
        private int rank;
        public int Rank { get { return rank; } }

        public int Mind { get { return mind.Score; } }
        public int Body { get { return body.Score; } }
        public int Spirit { get { return spirit.Score; } }

        [SerializeField] private Attribute mind;
        [SerializeField] private Attribute body;
        [SerializeField] private Attribute spirit;

        [SerializeField] public List<Skill> Skills;

        public StatBlock(int pMind, int pBody, int pSpirit, List<Skill> pSkills) {
            Skills = new List<Skill>();

            mind = new Attribute(Attributes.Mind, pMind);
            body = new Attribute(Attributes.Body, pBody);
            spirit = new Attribute(Attributes.Spirit, pSpirit);

            Skills = pSkills;
        }

        public static StatBlock GenerateMercenaryStatBlock(int pRank = 1) {
            int points = 5 + (pRank * 2);
            int skillPoints = 2 + (pRank * 2);

            // Generate randomized weights for the three stats
            int[] stats = new int[3];
            stats[0] = Random.Range(1, points / 2);
            stats[1] = Random.Range(1, points - stats[0]);
            stats[2] = points - stats[0] - stats[1];

            // Assign the stats randomly
            List<int> s = stats.OrderBy(r => Random.Range(0f, 1f)).ToList();

            // Get some skills (one that keys off the main stat, and up to two other random ones).
            List<Skill> skills = new List<Skill>();

            // Main skill
            int maxStatIndex = s.IndexOf(Mathf.Max(s[0], s[1], s[2]));
            switch (maxStatIndex) {
                case 0: skills.Add(new Skill(RPGController.MindSkills[Random.Range(0, 5)], 0)); break;
                case 1: skills.Add(new Skill(RPGController.BodySkills[Random.Range(0, 5)], 0)); break;
                case 2: skills.Add(new Skill(RPGController.SpiritSkills[Random.Range(0, 5)], 0)); break;
            }

            // Other skills
            int numAdditionalSkills = Random.Range(1, 3);
            int n = 0;
            do {
                Skills skill = (Skills)Random.Range(0, System.Enum.GetNames(typeof(Skills)).Length);

                bool valid = true;

                foreach(Skill sk in skills) { if (sk.Name.Equals(skill)) valid = false; }
                if(valid) {
                    n++;
                    skills.Add(new Skill(skill, 0));
                }

            } while (n < numAdditionalSkills);

            // Distribute skill points
            for(int i = 0; i < skillPoints; i++) {
                Skill skill = skills[Random.Range(0, skills.Count)];
                skill.Bonus++;
            }

            // Create the object and return it
            return new StatBlock(stats[0], stats[1], stats[2], skills);
        }

        public int GetAttributeScoreFor(Attributes attribute) {
            switch(attribute) {
                default: return -1;
                case App.Attributes.Mind: return Mind;
                case App.Attributes.Body: return Body;
                case App.Attributes.Spirit: return Spirit;
            }
        }

        public bool HasSkill(Skills skill) {
            if (Skills == null) Skills = new List<Skill>();

            foreach(Skill s in Skills) {
                if (s.Name.Equals(skill)) return true;
            }

            return false;
        }

        public int GetSkillScore(Skills skill) {
            foreach(Skill s in Skills) {
                if (s.Name.Equals(skill)) return s.Bonus;
            }

            return 0;
        }

        public void RemoveSkill(Skills skill) {
            Skill sk = null;

            foreach (Skill s in Skills) {
                if (s.Name.Equals(skill)) {
                    sk = s;
                }
            }

            if (sk == null) return;

            DestroyImmediate(sk, true);
            Skills.Remove(sk);
        }

        public void AddSkill(Skills skill, int bonus = 0) {
            if (HasSkill(skill)) return;

            Skill sk = CreateInstance<Skill>();
            Skills.Add(new Skill(skill, bonus));
        }
    }
}
