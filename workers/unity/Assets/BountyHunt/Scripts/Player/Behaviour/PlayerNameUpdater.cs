using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Improbable.Gdk.Subscriptions;
using Fps;
using Bountyhunt;
using System;

public class PlayerNameUpdater : MonoBehaviour
{
    public Canvas nameCanvas;
    public TextMeshProUGUI playerNameText;
    public BountyPickUpMaterialSettings materialSettings;
    [Require] HunterComponentReader hunterComponentReader;

    private void OnEnable()
    {
        UpdateName(hunterComponentReader.Data.Bounty);
        hunterComponentReader.OnBountyUpdate += UpdateName;
    }

    public void UpdateName(long bounty)
    {
        Color c = materialSettings.getColorByValue(bounty);

        playerNameText.text = hunterComponentReader.Data.Name;
        playerNameText.color = c;
    }

    private void Update()
    {
        Vector3 vec = nameCanvas.transform.position - Camera.main.transform.position;
        //vec = new Vector3(vec.x, 0, vec.y);

        nameCanvas.transform.forward = vec.normalized;
    }
}
