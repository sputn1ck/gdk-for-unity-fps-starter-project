using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TextMeshProUGUI rank;
    public TextMeshProUGUI playerName;
    public List<TextMeshProUGUI> values;
    public CanvasGroup canvasGroup;

    public void Set(int rank, string playerName, List<string> values)
    {
        canvasGroup.alpha = 1;
        if(values.Count != this.values.Count)
        {
            throw new ArgumentException("length of values-List has to match with the objects values");
        }

        this.rank.text = rank + ".";
        this.playerName.text = playerName;
        for(int i = 0; i < values.Count;i++)
        {
            this.values[i].text = values[i];
        }
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
    }
}
