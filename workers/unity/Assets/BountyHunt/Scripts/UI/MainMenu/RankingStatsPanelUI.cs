using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingStatsPanelUI : MonoBehaviour
{
    public Image BadgeImage;
    public Transform StatsContainer;
    [HideInInspector] public StatsValueUI[] stats;
    public TextMeshProUGUI HeadlineExtraValue; 

    private void Awake()
    {
        stats = StatsContainer.GetComponentsInChildren<StatsValueUI>();
    }
}
