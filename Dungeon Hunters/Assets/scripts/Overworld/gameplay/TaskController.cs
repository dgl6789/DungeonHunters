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

        /// <summary>
        /// Given a task type, mercenary, and tile, generate a notification that can be parsed and sent to the UI.
        /// </summary>
        /// <returns>Notification object containing an event.</returns>
        public Notification GenerateNotification(TaskType pType, MercenaryData pMercenary, HexTile pTile) {
            // Get a random event object from the event manifest for the given type. It should contain information as to
            // whether the event is clear-required.

            // Right now, return an empty event.
            EncounterEvent e = new EncounterEvent();

            // Calculate the day limit based on how long it will take the mercenary to get there from its current tile.
            int lDayLimit = 0 + e.ExtraDayTimer;

            // Return the generated notification.
            return new Notification(pType, e, lDayLimit, e.IsRequired, pMercenary, pTile);
        }

        /// <summary>
        /// Creates a Notification object from the given Notification specs
        /// </summary>
        /// <returns>Notification object to be pushed to the Canvas.</returns>
        public GameObject ParseNotification(Notification pNotification) {
            return null;
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