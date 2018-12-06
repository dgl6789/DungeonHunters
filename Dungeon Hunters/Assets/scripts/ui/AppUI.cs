using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Numeralien.Utilities;
using TMPro;

namespace App.UI {
    public class AppUI : MonoBehaviour {

        [SerializeField] Image BookImage;

        [HideInInspector] public bool leftPanelOpen;

        [SerializeField, ReadOnly] bool AcceptingKeyInput = true;

        public static AppUI Instance;

        private MercenaryData selectedMercenary;
        public MercenaryData SelectedMercenary {
            get { return selectedMercenary; }
        }

        public Canvas UICanvas;

        [Header("Text Fields")]

        public TextMeshProUGUI DateText;
        public TextMeshProUGUI SelectTile;
        public TextMeshProUGUI InventoryInstructions;
        public TextMeshProUGUI NotificationBadge;
        public TextMeshProUGUI NotificationCount;

        public TextMeshProUGUI MercenaryName;
        public TextMeshProUGUI MercenaryRank;
        public TextMeshProUGUI MercenaryStats;
        public TextMeshProUGUI MercenarySkills;

        public TextMeshProUGUI MercenaryNameInventory;
        public TextMeshProUGUI MercenaryPowerLevelInventory;
        public TextMeshProUGUI MercenaryEquipPowerInventory;
        public TextMeshProUGUI MercenaryRankInventory;

        [Header("Buttons")]

        public Button SetMissionButton;
        public Button FocusOnMercButton;
        public Button AdvanceDayButton;

        public Image LockTopInventoryButton;
        public Image LockBottomInventoryButton;

        [Header("Animators")]

        public Animator TopInventoryPanel;
        public Animator BottomInventoryPanel;

        [Header("Sprites")]

        public Sprite[] inventoryButtonSprites;
        public Sprite[] itemTypeSprites;
        public Sprite enchantmentLockedSprite;
        public Sprite enchantmentEmptySprite;

        [Header("Transforms")]

        public RectTransform MercInventoryGridParent;
        public RectTransform StrongholdInventoryGridParent;
        public RectTransform LootInventoryGridParent;

        public Transform MapOverlayParent;
        public RectTransform CommonUIParent;

        [SerializeField] RectTransform[] Pages;
        [HideInInspector] public int lastPageOpened;

        [SerializeField] RectTransform LeftMenuPanel;
        public RectTransform MercenaryInventoryPanel;
        public RectTransform StrongholdInventoryPanel;

        [Header("Objects")]

        public GameObject NotificationObject;
        public GameObject MercenaryLocationPin;
        public GameObject ItemObject;
        public GameObject ItemTooltipObject;
        public GameObject infoTooltipObject;

        public Item towershield;
        public Item armor;
        public Item berryswitch;
        public Item meat;

        private List<GameObject> informationalTooltips;

        public void Awake() {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);

            AcceptingKeyInput = true;
        }

        public void Start() {
            informationalTooltips = new List<GameObject>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z)) {
                int rot = Random.Range(0, 4);

                Item m = Item.Initialize(meat, 0, 0, rot, true, SelectedMercenary.Inventory);
                m.StackSize = Random.Range(3, 27);
                SelectedMercenary.Inventory.AddItem(m, 0, 0);
                SelectedMercenary.Inventory.AddItem(Item.Initialize(towershield, 3, 4, rot, true, SelectedMercenary.Inventory), 3, 4);
                SelectedMercenary.Inventory.AddItem(Item.Initialize(armor, 3, 1, rot, true, SelectedMercenary.Inventory), 3, 1);
                SelectedMercenary.Inventory.AddItem(Item.Initialize(berryswitch, 2, 2, rot, true, SelectedMercenary.Inventory), 2, 2);
            }

            if(AcceptingKeyInput)
            {
                if (Input.GetButtonDown("LeftMenu")) ToggleLeftMenu();

                if (Input.GetButtonDown("NotificationMenu")) SwitchPage(0);
                if (Input.GetButtonDown("AreaReportMenu")) SwitchPage(1);
                if (Input.GetButtonDown("MercenaryManagementMenu")) SwitchPage(2);
                if (Input.GetButtonDown("StrongholdManagementMenu")) SwitchPage(3);
                if (Input.GetButtonDown("OptionsMenu")) SwitchPage(4);

                if(Input.GetButtonDown("Equip")) {
                    InventoryUIController.Instance.ToggleEquipMousedOverItem();
                }
            }
        }

        public void ToggleLeftMenu() {
            if (LeftMenuPanel) {
                leftPanelOpen = !leftPanelOpen;
                LeftMenuPanel.GetComponent<Animator>().SetBool("Open", leftPanelOpen);
            }

            InventoryUIController.Instance.OnToggleLeftMenu();
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

                if (InventoryUIController.Instance.bottomInventoryOpen)
                {
                    BottomInventoryPanel.SetTrigger("SwapSelected");

                    // Load the selected mercenary's data into the inventory panel while it's behind the left panel ui
                    InventoryUIController.Instance.Invoke("LoadMercenaryInventory", 0.075f);
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

        /// <summary>
        /// Swap which page in the base UI is displayed
        /// </summary>
        /// <param name="index">Index in the array of the page object to swap to.</param>
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

        public static Rect RectTransformToScreenSpace(RectTransform transform) {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
            rect.x -= (transform.pivot.x * size.x);
            rect.y -= ((1.0f - transform.pivot.y) * size.y);
            return rect;
        }

        public void CreateInformationalTooltip(string pText) {
            InfoTooltipUIObject tip = Instantiate(infoTooltipObject, Tooltip.GetTooltipPosition(TooltipType.Info), Quaternion.identity, CommonUIParent).GetComponent<InfoTooltipUIObject>();
            tip.FollowMouse = true;
            tip.SetText(pText);

            informationalTooltips.Add(tip.gameObject);
        }

        public void DestroyInformationalTooltips() {
            int c = informationalTooltips.Count;

            for(int i = 0; i < c; i++) {
                Destroy(informationalTooltips[0]);
                informationalTooltips.RemoveAt(0);
            }
        }
    }
}