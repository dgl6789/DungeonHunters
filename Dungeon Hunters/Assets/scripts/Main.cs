using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App.UI;

public class Main : MonoBehaviour {

    [SerializeField]
    GameObject controllerUI;

    private void Awake() {
        if(Universal.Instance == null) {
            GameObject g = Instantiate(controllerUI, transform);
            Universal.Instance = g.GetComponent<Universal>();
        }
    }
}
