using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;
using App.UI;

namespace App {
    public enum TaskType { NONE, DUNGEONEER, SCOUT, FORAGE, TRAIN }

    public class TaskController : MonoBehaviour {

        public static TaskController Instance;

        public List<MercenaryData> Mercenaries;

        private TaskType selectedTask;

        // Use this for initialization
        void Awake() {
            // Put the single in singleton
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        public void SetMercenaryTask(MercenaryData pMercenary, TaskType pTaskType, HexTile pTile) {

        }

        public void RefreshMercenaryLocations() {
            // Set the mercenary's location one days' travel closer to their goal.
            // A day's travel is one hex divided by the rough terrain factor for that tile, which depends on its tile type.
        }

        public void ClickSetMissionButton() {
            StartCoroutine(SetMission());
        }

        private IEnumerator SetMission() {
            MercenaryData m = AppUI.Instance.selectedMercenary;

            // Collapse the left menu

            AppUI.Instance.ToggleLeftMenu();

            // Show the "Select a Tile" text

            AppUI.Instance.ToggleSelectTileText(m.Name);

            // Wait for a tile to be selected and confirmed

            yield return new WaitUntil(() => (Input.GetButtonDown("Confirm") && TileSelector.Instance.TargetTile != null));

            // Pop up the task type selection dialog

            // set selected task

            // Generate a corresponding notification and push it to the panel

            // reset selectedtask

            yield return null;
        }
    }
}