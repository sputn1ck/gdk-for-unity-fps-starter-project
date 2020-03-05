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
        satText.text = Utility.SatsToShortString(sats, UITinter.tintDict[TintColor.Sats]);
        if (diff == 0)
            return;
        if (diff > 0)
        {
            

            satUpdate.text = "+"+ Utility.SatsToShortString(diff);
            satUpdate.color = positiveColor;
        }
        else
        {

            satUpdate.text = Utility.SatsToShortString(diff);
            satUpdate.color = negativeColor;
        }
        
        updatePanel.GetComponent<Animator>().SetTrigger("play");
    }
    
}
