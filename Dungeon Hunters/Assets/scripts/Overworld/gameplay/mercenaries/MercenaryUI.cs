using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace App.UI {
    public class MercenaryUI : MonoBehaviour {

        [HideInInspector] public MercenaryData Data;

        [SerializeField] TextMeshProUGUI NameField;
        [SerializeField] TextMeshProUGUI RankField;
        [SerializeField] TextMeshProUGUI StatsField;
        [SerializeField] Image Portrait;

        public void AssignData(MercenaryData pData) {
            Data = pData;

            NameField.text = pData.Name;
            RankField.text = "Rank " + pData.Stats.Rank;
            StatsField.text = pData.Stats.Mind + " / " + pData.Stats.Body + " / " + pData.Stats.Spirit;
            // Portrait.sprite = pData.Portrait;
        }
    }
}
