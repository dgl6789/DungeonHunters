using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using App.UI;
using System.Linq;
using App.Data;
using UnityEngine.EventSystems;
using Overworld;

namespace App {
    public class MercenaryController : MonoBehaviour {

        public static MercenaryController Instance;
        [SerializeField] MercenaryManifest manifest;

        [HideInInspector] public List<MercenaryData> Mercenaries;
        List<MercenaryData> orderedMercenaryList;

        [SerializeField] GameObject mercenaryUIObject;
        [SerializeField] RectTransform mercenaryUIContainer;
        [SerializeField] Sprite[] mercenaryTabSprites;

        // Use this for initialization
        void Awake() {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            Mercenaries = new List<MercenaryData>();
            orderedMercenaryList = new List<MercenaryData>();
        }

        private void Start() {
            // Add a starter mercenary to the list.
            Mercenaries.Add(RandomMercenary());
            Mercenaries.Add(RandomMercenary());
            UpdateMercenaryUI();
            SetSelectedMercenary(Mercenaries[0]);
        }

        public void SetSelectedMercenary(MercenaryData pMercenary) {
            AppUI.Instance.SetSelectedMercenary(pMercenary);

            // Set the tab sprites to indicate the selected mercenary
            foreach (RectTransform tab in mercenaryUIContainer.GetComponentsInChildren<RectTransform>()) {
                if (tab != mercenaryUIContainer && tab.GetComponent<MercenaryUI>()) {
                    if(tab.GetComponent<MercenaryUI>().Data == AppUI.Instance.SelectedMercenary) {
                        tab.GetComponent<Image>().sprite = mercenaryTabSprites[1];
                    } else {
                        tab.GetComponent<Image>().sprite = mercenaryTabSprites[0];
                    }
                }
            }

            // Update the text in the right side of the mercenary panel
            AppUI.Instance.MercenaryName.text = AppUI.Instance.SelectedMercenary.Name;
            AppUI.Instance.MercenaryRank.text = "Rank " + AppUI.Instance.SelectedMercenary.Rank;
            AppUI.Instance.MercenarySkills.text = ParseSkillsList(AppUI.Instance.SelectedMercenary);
            AppUI.Instance.MercenaryStats.text = ParseStats(AppUI.Instance.SelectedMercenary);
        }
        
        public void UpdateMercenaryUI() {
            // Remove all the existing merc tabs from the UI
            foreach(RectTransform tab in mercenaryUIContainer.GetComponentsInChildren<RectTransform>()) {
                if (tab != mercenaryUIContainer) Destroy(tab.gameObject);
            }

            // Iterate through the list and recreate the mercenary tabs, sorted by rank
            orderedMercenaryList = Mercenaries.OrderByDescending(m => m.Rank).ToList();

            for (int i = 0; i < orderedMercenaryList.Count; i++) {
                // Instantiate a mercenary ui object in the container with the data at i.
                GameObject n = Instantiate(mercenaryUIObject, mercenaryUIContainer.transform);
                n.GetComponent<MercenaryUI>().AssignData(orderedMercenaryList[i]);
                RectTransform rect = n.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(rect.localPosition.x, 
                    (mercenaryUIContainer.rect.yMax - mercenaryUIObject.GetComponent<RectTransform>().rect.height / 2) - i * (rect.rect.height + 3));

                n.GetComponent<Button>().onClick.AddListener(() => { SetSelectedMercenary(n.GetComponent<MercenaryUI>().Data); });
            }
        }

        /// <summary>
        /// Compute the path that a mercenary will follow to reach a destination.
        /// </summary>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <returns></returns>
        public List<HexTile> GetTaskPath(HexTile pStart, HexTile pEnd) {
            List<HexTile> path = new List<HexTile>();
            HexAddress currentAddress = pStart.Address;
            HexAddress targetAddress = pEnd.Address;

            path.Add(pStart);

            // While the range is not complete...
            while (currentAddress != targetAddress) {
                // Get the desired direction toward the address of currentRange[1].
                Vector2Int d = new Vector2Int();

                // If the end tile is above this one, add 1 to y, if it is below subtract 1 from y, else do nothing.
                if (targetAddress.X > currentAddress.X) d.x++;
                else if (targetAddress.X < currentAddress.X) d.x--;

                // If the end tile is to the right of this one, add 1 to x, if it is to the left subrtact 1, else do nothing.
                if (targetAddress.Y > currentAddress.Y) d.y++;
                else if (targetAddress.Y < currentAddress.Y) d.y--;

                // Make sure we stay adjacent to the current address
                if (d.y == d.x && d.x + d.y != 0) {
                    if (Random.Range(0f, 1.0f) >= 0.5f) d.y = 0;
                    else d.x = 0;
                }
                
                currentAddress += new HexAddress(d.x, d.y);

                if (currentAddress.Equals(targetAddress)) break;
                
                // Add the tile to the path.
                path.Add(HexMap.Instance.Tiles[currentAddress]);
            }

            path.Add(pEnd);

            return path;
        }

        public void UpdateLocationPins() {
            foreach(MercenaryData mercenary in Mercenaries) {
                GameObject pin;

                if(mercenary.LocationMarker == null) {
                    // Instantiate a marker and set the reference in the mercenary data
                    pin = Instantiate(AppUI.Instance.MercenaryLocationPin, AppUI.Instance.MapOverlayParent);
                    mercenary.LocationMarker = pin;
                } else {
                    pin = mercenary.LocationMarker;
                }

                pin.transform.parent = mercenary.Location.transform;
                pin.transform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// Get a random mercenary data for a mercenary of the given power level.
        /// </summary>
        /// <param name="pRank">Power level mercenary to generate.</param>
        /// <returns>Mercenary data of the generated mercenary.</returns>
        public MercenaryData RandomMercenary(int pRank = 1) {
            int points = 5 + (pRank * 2);

            // Generate randomized weights for the three stats
            int[] stats = new int[3];
            stats[0] = Random.Range(1, points / 2);
            stats[1] = Random.Range(1, points - stats[0]);
            stats[2] = points - stats[0] - stats[1];

            // Assign the stats randomly
            List<int> s = stats.OrderBy(r => Random.Range(0f, 1f)).ToList();

            // Get some skills (one that keys off the main stat, and up to two other random ones).
            List<MercenarySkills> skills = new List<MercenarySkills>();

            // Main skill
            int maxStatIndex = s.IndexOf(Mathf.Max(s[0], s[1], s[2]));
            switch (maxStatIndex) {
                case 0: skills.Add(MindSkills()[Random.Range(0, 5)]); break;
                case 1: skills.Add(BodySkills()[Random.Range(0, 5)]); break;
                case 2: skills.Add(SpiritSkills()[Random.Range(0, 5)]); break;
                default: skills.Add((MercenarySkills)Random.Range(0, 15)); break;
            }

            // Other skills
            int numAdditionalSkills = Random.Range(1, 3);
            int n = 0;
            do {
                MercenarySkills skill = (MercenarySkills)Random.Range(0, 15);

                if(!skills.Contains(skill)) {
                    skills.Add(skill);
                    n++;
                }
            } while (n < numAdditionalSkills);

            // Get a name from the manifest
            // Create the object and return it
            return new MercenaryData(manifest.Names[Random.Range(0, manifest.Names.Length)], s[0], s[1], s[2], skills, pRank);
        }

        public string ParseSkillsList(MercenaryData pData) {
            string s = "";

            foreach(MercenarySkills skill in pData.Skills) {
                if(skill.ToString().Length < 8)
                    s += skill.ToString() + "\t\t" + CalculateSkill(pData, skill) + "\n";
                else
                    s += skill.ToString() + "\t" + CalculateSkill(pData, skill) + "\n";
            }

            if (s != "") return s;
            else return "None";
        }

        public string ParseStats(MercenaryData pData) {
            return "Mind\t" + pData.Mind + "\nBody\t" + pData.Body + "\nSpirit\t" + pData.Spirit;
        }

        public List<MercenarySkills> BodySkills() {
            return new List<MercenarySkills>() {
                MercenarySkills.Constitution, MercenarySkills.Endurance, MercenarySkills.Stealth, MercenarySkills.Medicine, MercenarySkills.Athletics };
        }

        public List<MercenarySkills> MindSkills() {
            return new List<MercenarySkills>() {
                MercenarySkills.Knowledge, MercenarySkills.Smithing, MercenarySkills.Magic, MercenarySkills.Survival, MercenarySkills.Insight };
        }

        public List<MercenarySkills> SpiritSkills() {
            return new List<MercenarySkills>() {
                MercenarySkills.Coercion, MercenarySkills.Deception, MercenarySkills.Intimidation, MercenarySkills.Performance, MercenarySkills.Spot };
        }

        public int CalculateSkill(MercenaryData pData, MercenarySkills pSkill) {
            int body = pData.Body;
            int mind = pData.Mind;
            int spirit = pData.Spirit;

            switch(pSkill) {
                default: return 0;
                case MercenarySkills.Athletics: return (int)Mathf.Floor(0.5f * body + 0.3f * spirit + 0.2f * mind);
                case MercenarySkills.Coercion: return spirit;
                case MercenarySkills.Constitution: return body;
                case MercenarySkills.Deception: return (int)Mathf.Floor(0.75f * spirit + 0.25f * mind);
                case MercenarySkills.Endurance: return (int)Mathf.Floor(0.75f * body + 0.25f * spirit);
                case MercenarySkills.Insight: return (int)Mathf.Floor(0.5f * mind + 0.3f * spirit + 0.2f * body);
                case MercenarySkills.Intimidation: return (int)Mathf.Floor(0.75f * spirit + 0.25f * body);
                case MercenarySkills.Knowledge: return mind;
                case MercenarySkills.Magic: return (int)Mathf.Floor(0.75f * mind + 0.25f * spirit);
                case MercenarySkills.Medicine: return (int)Mathf.Floor(0.5f * body + 0.3f * mind + 0.2f * spirit);
                case MercenarySkills.Performance: return (int)Mathf.Floor(0.5f * spirit + 0.3f * body + 0.2f * mind);
                case MercenarySkills.Smithing: return (int)Mathf.Floor(0.75f * mind + 0.25f * body);
                case MercenarySkills.Spot: return (int)Mathf.Floor(0.5f * spirit + 0.3f * mind + 0.2f * body);
                case MercenarySkills.Stealth: return (int)Mathf.Floor(0.75f * body + 0.25f * mind);
                case MercenarySkills.Survival: return (int)Mathf.Floor(0.5f * mind + 0.3f * body + 0.2f * spirit);
            }
        }
    }
}
