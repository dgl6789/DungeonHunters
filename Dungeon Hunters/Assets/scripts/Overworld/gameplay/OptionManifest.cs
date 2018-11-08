using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overworld
{
    public class OptionManifest : MonoBehaviour
    {
        public static OptionManifest Instance;

        public void Awake()
        {
            if (Instance == null) Instance = this;
            else if (Instance != this) Destroy(gameObject);
        }

        public void ExampleOptionCallback()
        {
            Debug.Log("Example option callback has been called.");
        }
    }
}
