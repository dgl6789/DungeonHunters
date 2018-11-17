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
        public GameObject MercenaryLocationPin;

        public Transform MapOverlayParent;
        
        [SerializeField] RectTransform[] Pages;
        [HideInInspector] public int lastPageOpened;
        
        [SerializeField] RectTransform LeftMenuPanel;
        [SerializeField] RectTransform MercenaryInventoryPanel;
        [SerializeField] RectTransform StrongholdInventoryPanel;

        [HideInInspector] public bool leftPanelOpen;

        [SerializeField, ReadOnly] bool AcceptingKeyInput = true;

        public static AppUI Instance;

        private MercenaryData selectedMercenary;
        public MercenaryData SelectedMercenary {
            get { return selectedMercenary; }
        }

        [Header("Text Fields")]

        public TextMeshProUGUI DateText;
        public TextMeshProUGUI SelectTile;
        public TextMeshProUGUI NotificationBadge;
        public TextMeshProUGUI NotificationCount;

        public TextMeshProUGUI MercenaryName;
        public TextMeshProUGUI MercenaryRank;
        public TextMeshProUGUI MercenaryStats;
        public TextMeshProUGUI MercenarySkills;

        [Header("Buttons")]

        public Button SetMissionButton;
        public Button FocusOnMercButton;
        public Button AdvanceDayButton;

        [Header("Animators")]

        [SerializeField] Animator TopInventoryPanel;
        [SerializeField] Animator BottomInventoryPanel;

        bool topInventoryOpen, bottomInventoryOpen;
        MercenaryData lastLoadedMercenaryInventory;

        [Header("Sprites")]

        [SerializeField] Sprite[] inventoryButtonSprites;

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

            if (!leftPanelOpen)
            {
                BottomInventoryPanel.SetBool("Open", false);
                TopInventoryPanel.SetBool("Open", false);
            } else {
                BottomInventoryPanel.SetBool("Open", bottomInventoryOpen);
                TopInventoryPanel.SetBool("Open", topInventoryOpen);
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

        public void SetSelectedMercenary(MercenaryData pMercenary) {
            if (selectedMercenary != pMercenary)
            {
                selectedMercenary = pMercenary;

                UpdateMercInteractionButton(pMercenary);

                if (bottomInventoryOpen)
                {
                    BottomInventoryPanel.SetTrigger("SwapSelected");

                    // Load the selected mercenary's data into the inventory panel while it's behind the left panel ui
                    Invoke("LoadMercenaryInventory", 0.075f);
                }
            }
        }

        public void UpdateMercInteractionButton(MercenaryData pMercenary) {
            bool mercIsOnMission = false;

            foreach (Notification n in NotificationController.Instance.Notifications) {
                if (n.Mercenary == pMercenary) {
                    mercIsOnMission = true;
                    break;
                }
            }
            
            SetMissionButton.interactable = !mercIsOnMission;
        }

        public void ClickFocusMercButton() {
            TileSelector.Instance.SetTarget(SelectedMercenary.Location);
        }

        public void ClickShowStrongholdInventoryButton()
        {
            topInventoryOpen = !topInventoryOpen;
            TopInventoryPanel.SetBool("Open", topInventoryOpen);

            if(topInventoryOpen) LoadStrongholdInventory();
        }

        public void ClickShowMercenaryInventoryButton() {
            bottomInventoryOpen = !bottomInventoryOpen;
            if (bottomInventoryOpen)
            {
                    BottomInventoryPanel.SetTrigger("SwapSelected");

                    // Load the selected mercenary's data into the inventory panel while it's behind the left panel ui
                    Invoke("LoadMercenaryInventory", 0.075f);
            }

            // Toggle the inventory panel's visibility
            BottomInventoryPanel.SetBool("Open", bottomInventoryOpen);
        }

        public void LoadMercenaryInventory() {
            Debug.LogFormat("Loaded {0}'s inventory.", selectedMercenary.Name);
        }

        public void LoadStrongholdInventory()
        {
            Debug.Log("Loaded the Stronghold inventory.");
        }

        public void SwitchPage(int index) {

            if (leftPanelOpen && index == lastPageOpened) {
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