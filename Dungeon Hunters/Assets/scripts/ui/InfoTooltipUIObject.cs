using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace App.UI {
    public class InfoTooltipUIObject : Tooltip {

        [SerializeField] TextMeshProUGUI content;

        public void SetText(string pText) {
            content.text = pText;
        }
    }
}
