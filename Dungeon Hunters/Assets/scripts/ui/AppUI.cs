using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Numeralien.Utilities;
using TMPro;

namespace App.UI {
    public class AppUI : MonoBehaviour {

        [SerializeField] Image BookImage;
        public GameObject NotificationObject;
        
        [SerializeField] RectTransform[] Pages;
        [HideInInspector] public int lastPageOpened;
        
        [SerializeField] RectTransform LeftMenuPanel;

        [HideInInspector] public bool leftPanelOpen;

        [SerializeField, ReadOnly] bool AcceptingKeyInput = true;

        public static AppUI Instance;

        [HideInInspector] public MercenaryData SelectedMercenary;

        [Header("Text Fields")]

        public TextMeshProUGUI DateText;
        public TextMeshProUGUI SelectTile;
        public TextMeshProUGUI NotificationBadge;
        public TextMeshProUGUI NotificationCount;

        public TextMeshProUGUI MercenaryName;
        public TextMeshProUGUI MercenaryRank;
        public TextMeshProUGUI MercenaryStats;
        public TextMeshProUGUI MercenarySkills;

        public void Awake() {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            AcceptingKeyInput = true;
        }

        private void Update()
        {
            if(AcceptingKeyInput)
            {
                if (Input.GetButtonDown("LeftMenu")) ToggleLeftMenu();

                if (Input.GetButtonDown("NotificationMenu")) SwitchPage(0);
                if (Input.GetButtonDown("AreaReportMenu")) SwitchPage(1);
                if (Input.GetButtonDown("MercenaryManagementMenu")) SwitchPage(2);
                if (Input.GetButtonDown("StrongholdManagementMenu")) SwitchPage(3);
                if (Input.GetButtonDown("OptionsMenu")) SwitchPage(4);
            }
        }

        public void ToggleLeftMenu() {
            if (LeftMenuPanel) {
                leftPanelOpen = !leftPanelOpen;
                LeftMenuPanel.GetComponent<Animator>().SetBool("Open", leftPanelOpen);
            }
        }

        public void ToggleSelectTileText(string mercenaryName = "") {
            if (!SelectTile.gameObject.activeSelf) {
                SelectTile.text = "Select the tile to send " + mercenaryName + " to.\nPress ENTER to confirm.";
                SelectTile.gameObject.SetActive(true);
            } else {
                SelectTile.gameObject.SetActive(false);
            }
        }

        public void SwitchPage(int index) {
            if(leftPanelOpen && index == lastPageOpened) {
                ToggleLeftMenu();
                return;
            }

            for (int p = 0; p < Pages.Length; p++) {
                if (p == index) {
                    Pages[p].gameObject.SetActive(true);
                } else {
                    Pages[p].gameObject.SetActive(false);
                }
            }

            if (!leftPanelOpen) ToggleLeftMenu();

            lastPageOpened = index;
        }
    }
}