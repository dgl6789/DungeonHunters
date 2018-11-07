using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudTileAnimation : MonoBehaviour {

    Animator anim;
    
    void Awake() { anim = GetComponent<Animator>(); }

    public void LowerCloudLevel() { anim.SetBool("isMountains", true); }
}
