using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using App.Data;
using App.UI;
using UnityEngine.UI;

namespace App {
    public class EncounterController : MonoBehaviour {

        public static EncounterController Instance;

        [SerializeField] EncounterManifest Manifest;

        [SerializeField] TextMeshProUGUI MainText;
        [SerializeField] GameObject ActionUIObject;

        [SerializeField] RectTransform ActionUIParentRect;

        EncounterEvent activeEncounter;
        [HideInInspector] public MercenaryData ActiveMercenary;
        int currentEncounterStep;

        public EncounterEvent DungeoneeringEvent;

        private void Awake() {
            if (Instance == null) Instance = this;
            else Destroy(this);
        }

        public EncounterEvent GetRandomEncounter() {
            return Manifest.Encounters[Random.Range(0, Manifest.Encounters.Length)];
        }

        public void InitializeNewEncounter(EncounterEvent e, MercenaryData m) {
            // Swap to the Encounter ui page
            AppUI.Instance.SwitchPage(5);

            // Set up the encounter UI
            SetActiveEncounter(e);
            SetActiveMercenary(m);

            currentEncounterStep = 0;
            LoadEncounterStep(currentEncounterStep);
        }

        public void IncrementEncounterStep() {
            currentEncounterStep++;

            LoadEncounterStep(currentEncounterStep);
        }

        public void SetActiveEncounter(EncounterEvent pEncounter) {
            // Do any necessary cleanup
            if (pEncounter == null) EndActiveEncounter();

            activeEncounter = Instantiate(pEncounter);
        }

        public void SetActiveMercenary(MercenaryData pMercenary) {
            ActiveMercenary = pMercenary;
        }

        public void EndActiveEncounter() {
            // Clear the old options
            foreach (RectTransform r in ActionUIParentRect.GetComponentsInChildren<RectTransform>()) {
                if (r != ActionUIParentRect) Destroy(r.gameObject);
            }

            AppUI.Instance.SwitchPage(0);

            activeEncounter = null;
        }

        public void LoadEncounterStep(int pStep) {
            if (pStep == -1) EndActiveEncounter();
            if (activeEncounter == null) return;

            // Clear the old options
            foreach (RectTransform r in ActionUIParentRect.GetComponentsInChildren<RectTransform>()) {
                if (r != ActionUIParentRect) Destroy(r.gameObject);
            }

            MainText.text = activeEncounter.Steps[pStep].Text;

            for (int i = 0; i < activeEncounter.Steps[pStep].Options.Count; i++) {
                EncounterOption option = activeEncounter.Steps[pStep].Options[i];

                // spawn the option object
                // Instantiate a mercenary ui object in the container with the data at i.
                GameObject n = Instantiate(ActionUIObject, ActionUIParentRect.transform);
                n.GetComponent<EncounterOptionUIObject>().Initialize(option);
                RectTransform rect = n.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(rect.localPosition.x,
                    (ActionUIParentRect.rect.yMax - ActionUIObject.GetComponent<RectTransform>().rect.height / 2) - i * (rect.rect.height + 3));

                // Set up click listener
                n.GetComponent<Button>().onClick.AddListener(() => {
                    
                    LoadEncounterStep(option.Action.Success(ActiveMercenary.Stats) ? option.SuccessStepIndex : option.FailureStepIndex);

                });
            }
        }
    }
}
