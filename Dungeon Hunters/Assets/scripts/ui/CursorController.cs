using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {

    public static CursorController Instance;

    public Texture2D[] commonTextures;

    static bool useDefault;
    bool holdingItem;

	// Use this for initialization
	void Awake () {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        Cursor.SetCursor(commonTextures[0], Vector2.zero, CursorMode.Auto);
        useDefault = true;
	}

    private void Update() {
        if (useDefault && !holdingItem) {
            if (Input.GetMouseButtonDown(0)) SetCursor(commonTextures[1], Vector2.zero);
            if (Input.GetMouseButtonUp(0)) SetCursor(commonTextures[0], Vector2.zero);
        }
    }

    public static void SetCursor(Texture2D pCursor, Vector2 pHotSpot) {
        Cursor.SetCursor(pCursor, pHotSpot, CursorMode.Auto);
    }

    public void MouseOverInventoryItem() {
        SetCursor(commonTextures[2], Vector2.zero);
        useDefault = false;
    }

    public void MouseExitInventoryItem() {
        if (!holdingItem) {
            SetCursor(commonTextures[0], Vector2.zero);
        }

        useDefault = true;
    }

    public void PickupInventoryItem() {
        SetCursor(commonTextures[3], Vector2.zero);
        holdingItem = true;
    }

    public void PutDownInventoryItem() {
        SetCursor(commonTextures[2], Vector2.zero);
        holdingItem = false;
    }

    public static void UseDefault() {
        useDefault = true;
    }
}
