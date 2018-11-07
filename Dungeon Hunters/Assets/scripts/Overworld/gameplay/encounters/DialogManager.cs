using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace App.UI {
    public enum DialogType { NONE, TASK_SELECT, ENCOUNTER }

    public class DialogManager : MonoBehaviour {

        public static DialogManager Instance;

        public bool DialogIsOpen;

        [SerializeField] RectTransform DialogObjectCommon;
        [SerializeField] RectTransform[] DialogObjects;

        // Use this for initialization
        void Awake() {
            // Put the single in singleton
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            DialogIsOpen = false;
        }

        public RectTransform ShowDialog(DialogType pType) {
            RectTransform activeDialog = null;

            for(int i = 1; i < DialogObjects.Length; i++) {
                if(((DialogType)i).Equals(pType)) {
                    DialogObjectCommon.gameObject.SetActive(true);
                    DialogObjects[i].gameObject.SetActive(true);
                    activeDialog = DialogObjects[i];
                }
            }

            return activeDialog;
        }

        public void CloseAllDialogs() {
            DialogObjectCommon.gameObject.SetActive(false);
            ShowDialog(DialogType.NONE);
            DialogIsOpen = false;
        }
    }
}
