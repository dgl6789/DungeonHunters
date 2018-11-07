using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overworld;
using App.UI;
using App.Data;

namespace App {
    public enum TaskType { NONE, DUNGEONEER, SCOUT, FORAGE, TRAIN }

    public class TaskController : MonoBehaviour {

        public static TaskController Instance;

        public List<MercenaryData> Mercenaries;

        public EncounterManifest EncountersManifest;

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

            // Get the event from the manifest.
            EncounterEvent e = EncountersManifest.Encounters[Random.Range(0, EncountersManifest.Encounters.Length)];

            // Calculate the day limit based on how long it will take the mercenary to get there from its current tile.
            int lDayLimit = 0 + e.ExtraDayTimer;

            // Return the generated notification.
            return new Notification(pType, e, ParseNotificationLabel(pType, pMercenary), lDayLimit, e.IsRequired, pMercenary, pTile);
        }

        /// <summary>
        /// Creates a string that describes a task within a notification
        /// </summary>
        /// <returns>Label to be inserted for this notification in the canvas object.</returns>
        public string ParseNotificationLabel(TaskType pType, MercenaryData pMerc) {
            string s = pMerc.Name;

            switch(pType) {
                case TaskType.DUNGEONEER: s += " travels to a dungeon."; break;
                case TaskType.FORAGE: s += " forages for materials."; break;
                case TaskType.SCOUT: s += " scouts a new area."; break;
                case TaskType.TRAIN: s += " trains in the wild."; break;
            }

            return s;
        }

        public void SetSelectedTask(int type) {
            selectedTask = (TaskType)type;
        }

        public bool IsValidTaskForTile(TaskType pTask, HexTile pTile) {
            switch(pTask) {
                default:
                case TaskType.NONE: return false;

                case TaskType.DUNGEONEER:
                    switch(pTile.Type) {
                        default: return false;
                        case TileType.DUNGEON: return true;
                    }

                case TaskType.SCOUT:
                    if (pTile.Obscured) return true;
                    else return false;

                case TaskType.TRAIN:
                case TaskType.FORAGE:
                    if (pTile.Obscured) return false;
                    else return true;
            }
        }

        private IEnumerator SetMission() {
            // Get the selected mercenary
            MercenaryData m = AppUI.Instance.SelectedMercenary;

            if (m == null) yield break;

            // Collapse the left menu

            AppUI.Instance.ToggleLeftMenu();

            // Show the "Select a Tile" text

            AppUI.Instance.ToggleSelectTileText(m.Name);

            // Wait for a tile to be selected and confirmed

            yield return new WaitUntil(() => (Input.GetButtonDown("Submit") && TileSelector.Instance.TargetTile != null));

            HexTile tile = TileSelector.Instance.TargetTile;

            // Pop up the task type selection dialog
            // Perform logic here for choosing options to show based on tile type
            RectTransform d = DialogManager.Instance.ShowDialog(DialogType.TASK_SELECT);

            // If this breaks, ShowDialog failed.
            if(d == null) yield break;

            RectTransform[] options = d.gameObject.GetComponentsInChildren<RectTransform>();
            int it = 0;
            foreach(RectTransform r in options) {
                if (r == d) continue;

                r.gameObject.SetActive(IsValidTaskForTile((TaskType)it, tile));

                it++;
            }

            // wait for a task to be selected
            yield return new WaitUntil(() => (selectedTask != TaskType.NONE));

            // Close the selection dialog
            DialogManager.Instance.CloseAllDialogs();

            // Generate a corresponding notification and push it to the panel
            NotificationController.Instance.AddNotification(GenerateNotification(selectedTask, m, tile));

            // reset selectedtask
            selectedTask = TaskType.NONE;

            yield return null;
        }
    }
}