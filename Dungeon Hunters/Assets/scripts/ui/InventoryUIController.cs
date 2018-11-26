﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using App.UI;
using App;

public class InventoryUIController : MonoBehaviour {

    public static InventoryUIController Instance;

    const int itemUISize = 31;

    [HideInInspector] public bool topInventoryOpen, bottomInventoryOpen, topInventoryLocked, bottomInventoryLocked;
    MercenaryData lastLoadedMercenaryInventory;
    Inventory topInventory, bottomInventory;

    [HideInInspector] public RectTransform SelectedItem;
    Inventory FocusedInventory;
    RectTransform FocusedGrid;

    private RectTransform tooltip;
    private Item lastMousedOverItem;
    private bool mouseHovering;

    public void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void Update() {
        if (mouseHovering) MouseOver(FocusedGrid);
    }

    public void MouseOver(RectTransform pGridParent) {
        // Only show a tooltip if there is no item selected.
        if (SelectedItem != null) {
            UnloadTooltip();
            return;
        }

        Item mousedOver = GetMousedOverItem(pGridParent);

        // No item moused over, but there is a tooltip.
        // Should get rid of the current tooltip
        if (mousedOver == null && tooltip != null) {
            UnloadTooltip();
        
        // An item is moused over, and it's different than the one moused over in the previous frame.
        // Should unload the loaded one and load a new one.
        } else if (mousedOver != null && mousedOver != lastMousedOverItem) {
            // Destroy the active tooltip
            UnloadTooltip();

            LoadTooltip(mousedOver);
        } 
        
        // An item is moused over, and it is the same one as in the previous frame.
        // Should just recalculate the tooltip's position.
        else if (mousedOver != null && mousedOver == lastMousedOverItem) {
            if (!tooltip) LoadTooltip(mousedOver);
            tooltip.position = GetTooltipPosition(mousedOver);
        }

        lastMousedOverItem = mousedOver;
    }

    /// <summary>
    /// Load a tooltip at the correct position for a given item.
    /// </summary>
    /// <param name="pItem">Item whose data should be loaded into the tooltip.</param>
    void LoadTooltip(Item pItem) {
        Vector2 position = GetTooltipPosition(pItem);

        // Instantiate a new tooltip for the moused over item
        tooltip = Instantiate(AppUI.Instance.ItemTooltipObject, position, Quaternion.identity, AppUI.Instance.CommonUIParent).GetComponent<RectTransform>();
        tooltip.GetComponent<ItemTooltipUIObject>().Initialize(pItem);
    }

    /// <summary>
    /// Delete a tooltip.
    /// </summary>
    public void UnloadTooltip() {
        if (tooltip) Destroy(tooltip.gameObject);
        tooltip = null;
    }

    /// <summary>
    /// Get the correct position of the active tooltip basesd on the mouse position and tooltip rect.
    /// </summary>
    /// <returns>The position of the tooltip.</returns>
    Vector2 GetTooltipPosition(Item pItem) {
        // Just update the tooltip's position 
        // It should be anchored to the mouse such that no part of it is off screen
        Vector3 mp = Input.mousePosition;
        Vector2 position = new Vector2();
        Rect targetRect = GetTargetTooltipRect(pItem);
        targetRect.size = Vector2.Scale(targetRect.size, AppUI.Instance.UICanvas.transform.lossyScale);

        if (mp.y + targetRect.height > Screen.height) {
            // Choose one of the top corners instead.
            if (mp.x - targetRect.width < 0) {
                // Should follow the top left corner
                position = new Vector2(mp.x + (targetRect.width / 2), mp.y - (targetRect.height / 2));
            } else {
                // Should follow the top right corner
                position = new Vector2(mp.x - (targetRect.width / 2), mp.y - (targetRect.height / 2));
            }
        } else {
            // Choose one of the bottom corners
            if (mp.x - targetRect.width < 0) {
                // Should follow the bottom left corner
                position = new Vector2(mp.x + (targetRect.width / 2), mp.y + (targetRect.height / 2));
            } else {
                // Should follow the bottom right corner
                position = new Vector2(mp.x - (targetRect.width / 2), mp.y + (targetRect.height / 2));
            }
        }
        
        return position;
    }

    private Rect GetTargetTooltipRect(Item pItem) {
        Rect targetRect = AppUI.Instance.ItemTooltipObject.GetComponent<RectTransform>().rect;

        switch(pItem.Type) {
            case ItemType.Resource:
                targetRect.yMax -= AppUI.Instance.ItemTooltipObject.GetComponent<ItemTooltipUIObject>().EquipmentBase.rect.height;
                break;
        }

        return targetRect;
    }

    public void MouseExit() {

        UnloadTooltip();
        lastMousedOverItem = null;
        
        mouseHovering = false;
    }

    public void MouseEnter() {
        mouseHovering = true;
    }

    public void ClickCloseTopInventoryButton() {
        if (topInventoryLocked) ClickLockTopInventory();
        else ClickShowStrongholdInventoryButton();
    }

    public void ClickCloseBottomInventoryButton() {
        if (bottomInventoryLocked) ClickLockBottomInventory();

        bottomInventoryOpen = false;

        // Toggle the inventory panel's visibility
        AppUI.Instance.BottomInventoryPanel.SetBool("Open", bottomInventoryOpen);
        AppUI.Instance.InventoryInstructions.gameObject.SetActive(bottomInventoryOpen || topInventoryOpen);
    }

    public void ClickLockTopInventory() {
        topInventoryLocked = !topInventoryLocked;

        AppUI.Instance.LockTopInventoryButton.sprite = AppUI.Instance.inventoryButtonSprites[topInventoryLocked ? 1 : 0];

        if (!topInventoryLocked) {
            if (!AppUI.Instance.leftPanelOpen) {
                ClickCloseTopInventoryButton();
            }
        }
    }

    public void ClickLockBottomInventory() {
        bottomInventoryLocked = !bottomInventoryLocked;

        AppUI.Instance.LockBottomInventoryButton.sprite = AppUI.Instance.inventoryButtonSprites[bottomInventoryLocked ? 1 : 0];

        if (!bottomInventoryLocked) {
            if (!AppUI.Instance.leftPanelOpen) {
                ClickCloseBottomInventoryButton();
            }
        }
    }

    void UpdateFocusedInventory(RectTransform pGridParent) {
        Inventory inv;
        if (pGridParent == AppUI.Instance.MercInventoryGridParent) inv = bottomInventory;
        else inv = topInventory;

        FocusedInventory = inv;
        FocusedGrid = pGridParent;
    }

    public void UpdateFocusedGrid(RectTransform pGrid) {
        FocusedGrid = pGrid;
    }

    Vector2Int GetInventorySlotFromScreenPoint(Vector2 pScreenPoint, RectTransform pGridParent) {

        UpdateFocusedInventory(pGridParent);

        // Get the inventory object that the screen point overlaps
        Rect invPanelSS = AppUI.RectTransformToScreenSpace(pGridParent);

        Vector2Int inventorySelectionIndex = new Vector2Int(
            Mathf.FloorToInt(Math.Map(pScreenPoint.x, invPanelSS.x, invPanelSS.xMax, 0, FocusedInventory.Width)),
            Mathf.FloorToInt(Math.Map(Screen.height - pScreenPoint.y, invPanelSS.y, invPanelSS.yMax, 0, FocusedInventory.Height))
        );

       return inventorySelectionIndex;
    }

    public void ClickInventoryGrid(RectTransform pGridParent) {
        if(SelectedItem == null) {
            // Try to pick up an inventory item
            PickupInventoryItem(pGridParent);
        } else {
            // Try to put down a held inventory item
            PutDownInventoryItem(pGridParent);
        }
    }

    public void PickupInventoryItem(RectTransform pGridParent) {
        if (SelectedItem != null) return;

        UpdateFocusedInventory(pGridParent);

        Vector2Int inventorySelectionIndex = GetInventorySlotFromScreenPoint(Input.mousePosition, pGridParent);

        // Check validity of clicked inventory index
        Item toPickUp = FocusedInventory.GetItemFromLayout(inventorySelectionIndex.x, inventorySelectionIndex.y);

        foreach (ItemObject i in pGridParent.GetComponentsInChildren<ItemObject>()) {
            if (toPickUp == i.Data) {
                SelectedItem = i.GetComponent<RectTransform>();
                break;
            }
        }

        if (SelectedItem == null) return;

        // We have the item object that should be held.
        CursorController.Instance.PickupInventoryItem();
    }

    public Item GetMousedOverItem(RectTransform pGridParent) {
        Vector2Int mousedOverIndex = GetInventorySlotFromScreenPoint(Input.mousePosition, pGridParent);
        return FocusedInventory.GetItemFromLayout(mousedOverIndex.x, mousedOverIndex.y);
    }

    public void PutDownInventoryItem(RectTransform pGridParent) {
        if (SelectedItem == null) return;
        
        // Get the inventory index that was clicked
        Vector2Int clickedIndex = GetInventorySlotFromScreenPoint(Input.mousePosition, pGridParent);

        UpdateFocusedInventory(pGridParent);

        // compare the item's indices to the indices in the inventory
        Item item = SelectedItem.GetComponent<ItemObject>().Data;
        if (!FocusedInventory.TestAddition(item, clickedIndex.x, clickedIndex.y)) return;

        // This item can go in the slots indicated by the clicked origin slot
        // Remove the item from the inventory it came from, and put it in the focused inventory
        // with its origin at the clicked slot
        item.Inventory.RemoveItem(item);
        FocusedInventory.AddItem(item, clickedIndex.x, clickedIndex.y);

        // null the selected item and destroy the ui object
        Destroy(SelectedItem.gameObject);
        SelectedItem = null;

        // Reload the focused inventory
        LoadInventory(FocusedInventory, pGridParent);
    }

    public void ClickShowStrongholdInventoryButton() {
        topInventoryOpen = topInventoryLocked ? true : !topInventoryOpen;
        AppUI.Instance.TopInventoryPanel.SetBool("Open", topInventoryOpen);

        if (topInventoryOpen) LoadStrongholdInventory();

        AppUI.Instance.InventoryInstructions.gameObject.SetActive(bottomInventoryOpen || topInventoryOpen);
    }

    public void ClickShowMercenaryInventoryButton() {
        bool before = bottomInventoryOpen;

        bottomInventoryOpen = bottomInventoryLocked ? true : !bottomInventoryOpen;
        if (bottomInventoryOpen && !before) {
            AppUI.Instance.BottomInventoryPanel.SetBool("Open", bottomInventoryOpen);

            // Load the selected mercenary's data into the inventory panel while it's behind the left panel ui
            Invoke("LoadMercenaryInventory", 0.075f);
        }

        // Toggle the inventory panel's visibility
        AppUI.Instance.BottomInventoryPanel.SetBool("Open", bottomInventoryOpen);
        AppUI.Instance.InventoryInstructions.gameObject.SetActive(bottomInventoryOpen || topInventoryOpen);
    }

    /// <summary>
    /// Load the inventory of the selected mercenary into the mercenary inventory panel
    /// </summary>
    public void LoadMercenaryInventory() {
        // Unload the current inventory
        UnloadMercenaryInventory();

        // Load the new inventory
        LoadInventory(AppUI.Instance.SelectedMercenary.Equipment, AppUI.Instance.MercInventoryGridParent);
    }

    /// <summary>
    /// Load the stronghold's inventory into the stronghold inventory panel
    /// </summary>
    public void LoadStrongholdInventory() {
        Debug.Log("Loaded the Stronghold inventory.");
    }

    /// <summary>
    /// Load the contents of an inventory into a UI panel.
    /// </summary>
    /// <param name="pToLoad">Inventory to load.</param>
    /// <param name="pUIGrid">Panel to load into.</param>
    public void LoadInventory(Inventory pToLoad, RectTransform pUIGrid) {
        List<Item> closedList = new List<Item>();

        for (int x = 0; x < pToLoad.Width; x++) {
            for (int y = 0; y < pToLoad.Height; y++) {

                Item i = pToLoad.GetItemFromLayout(x, y);

                if (i != null && !closedList.Contains(i)) {
                    // This item still needs to be instantiated
                    if (i.AbsoluteCenter == new Vector2Int(x, y)) {
                        // If this index is the index of the absolute center of the item, place it here and add it to the closed list.

                        GameObject item = Instantiate(AppUI.Instance.ItemObject, pUIGrid);
                        item.GetComponent<Image>().sprite = i.Image;
                        item.GetComponent<ItemObject>().Data = i;

                        // Set the scale
                        // Get the index bounds of the item in the inventory
                        Rect itemBounds = pToLoad.ItemBounds(i);

                        // xMin = itembounds.x, xMax = xMin + itemBounds.width * itemUISize, etc
                        item.GetComponent<RectTransform>().sizeDelta = new Vector2(itemBounds.width * itemUISize, itemBounds.height * itemUISize);

                        // Calculate the position of this index in the grid after scaling.
                        Vector2 position = new Vector2(
                            pUIGrid.rect.x + (itemBounds.x + itemBounds.width / 2) * itemUISize,
                            pUIGrid.rect.y - (itemBounds.y + itemBounds.height / 2) * itemUISize + pUIGrid.rect.height
                        );
                        item.GetComponent<RectTransform>().anchoredPosition = position;

                        item.GetComponent<ItemObject>().Initialize();

                        closedList.Add(i);
                    }
                }
            }
        }

        // Set up the text fields
        if (pUIGrid == AppUI.Instance.MercInventoryGridParent) {
            bottomInventory = pToLoad;
            AppUI.Instance.MercenaryNameInventory.text = AppUI.Instance.SelectedMercenary.Name;
            AppUI.Instance.MercenaryPowerLevelInventory.text = "Power " + pToLoad.PowerLevel;
        } else topInventory = pToLoad;
    }

    /// <summary>
    /// Unload the inventory currently loaded into the bottom inventory panel
    /// </summary>
    public void UnloadMercenaryInventory() {
        UnloadInventory(AppUI.Instance.MercInventoryGridParent);
    }

    /// <summary>
    /// Remove the contents of an inventory UI panel.
    /// </summary>
    /// <param name="pGridToUnload">Transform parent from which to unload.</param>
    public void UnloadInventory(RectTransform pGridToUnload) {
        RectTransform[] rects = pGridToUnload.GetComponentsInChildren<RectTransform>();

        foreach (RectTransform r in rects) {
            ItemObject[] items = r.GetComponentsInChildren<ItemObject>();
            foreach (ItemObject o in items) Destroy(o.gameObject);
        }

        if (pGridToUnload == AppUI.Instance.MercInventoryGridParent) bottomInventory = null;
        else topInventory = null;
    }

    /// <summary>
    /// When the left menu is toggled, hide our inventory UI or keep it open if it is locked.
    /// </summary>
    public void OnToggleLeftMenu() {
        if (!AppUI.Instance.leftPanelOpen) {
            if (!bottomInventoryLocked) AppUI.Instance.BottomInventoryPanel.SetBool("Open", false);
            if (!topInventoryLocked) AppUI.Instance.TopInventoryPanel.SetBool("Open", false);

            AppUI.Instance.InventoryInstructions.gameObject.SetActive(false);
        } else {
            AppUI.Instance.BottomInventoryPanel.SetBool("Open", bottomInventoryOpen);
            AppUI.Instance.TopInventoryPanel.SetBool("Open", topInventoryOpen);

            AppUI.Instance.InventoryInstructions.gameObject.SetActive(bottomInventoryOpen || topInventoryOpen);
        }
    }
}