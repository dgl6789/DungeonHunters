using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {

    [SerializeField]
    Texture2D[] commonTextures;

    static bool useDefault;

	// Use this for initialization
	void Awake () {
        Cursor.SetCursor(commonTextures[0], Vector2.zero, CursorMode.Auto);
        useDefault = true;
	}

    private void Update() {
        if (useDefault) {
            if (Input.GetMouseButtonDown(0)) SetCursor(commonTextures[1], Vector2.zero);
            if (Input.GetMouseButtonUp(0)) SetCursor(commonTextures[0], Vector2.zero);
        }
    }

    public static void SetCursor(Texture2D pCursor, Vector2 pHotSpot) {
        Cursor.SetCursor(pCursor, pHotSpot, CursorMode.Auto);
    }

    public static void UseDefault() {
        useDefault = true;
    }
}
