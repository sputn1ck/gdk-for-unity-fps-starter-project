using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BountyUIController : MonoBehaviour
{
    public TextMeshProUGUI satText;
    public TextMeshProUGUI satUpdate;
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
        satUpdate.GetComponent<Animator>().SetTrigger("SatUpdate");
    }
}
