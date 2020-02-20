using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class BountyUIController : MonoBehaviour
{
    public TextMeshProUGUI satText;
    public TextMeshProUGUI satUpdate;
    public Animator updatePanel;
    public Color positiveColor;
    public Color negativeColor;

    public void UpdateSats(long sats, long diff)
    {
        satText.text = sats.ToString();
        if (diff == 0)
            return;
        if (diff > 0)
        {


            satUpdate.text = "+"+diff.ToString();
            satUpdate.color = positiveColor;
        }
        else
        {

            satUpdate.text = diff.ToString();
            satUpdate.color = negativeColor;
        }

        updatePanel.GetComponent<Animator>().SetTrigger("play");
    }
    public void UpdateSats(float sats, float diff, string format = "F1")
    {
        satText.text = sats.ToString(format, CultureInfo.InvariantCulture);
        if (diff == 0)
            return;
        if (diff > 0)
        {


            satUpdate.text = "+" + diff.ToString();
            satUpdate.color = positiveColor;
        }
        else
        {

            satUpdate.text = diff.ToString();
            satUpdate.color = negativeColor;
        }
        updatePanel.GetComponent<Animator>().SetTrigger("play");

    }


}
