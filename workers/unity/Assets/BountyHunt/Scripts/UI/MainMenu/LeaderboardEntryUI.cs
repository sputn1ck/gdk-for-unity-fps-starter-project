using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TextMeshProUGUI rank;
    public TextMeshProUGUI playerName;
    public List<TextMeshProUGUI> values;
    public CanvasGroup canvasGroup;
    public Image highlight;

    public void Set(long rank, string playerName, List<string> values)
    {
        canvasGroup.alpha = 1;
        highlight.gameObject.SetActive(false);
        if (values.Count > this.values.Count)
        {
            throw new ArgumentException("length of values-List is larger than th ui elements count!");
        }

        this.rank.text = rank + ".";
        if (rank == 0) this.rank.text = "";
        this.playerName.text = playerName;
        for(int i = 0; i < this.values.Count;i++)
        {
            if (i < values.Count)
            {
                this.values[i].gameObject.SetActive(true);
                this.values[i].text = values[i];
            }
            else
            {
                this.values[i].gameObject.SetActive(false);
            }

        }
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
    }

    public void Highlight()
    {
        highlight.gameObject.SetActive(true);
    }
}
