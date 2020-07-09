using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySkinShop : MonoBehaviour
{
    public List<SkinContextMenu> skinDolls;
    private void Awake()
    {
        ClientEvents.instance.onGameJoined.AddListener(LoadSkins);
    }

    private async void LoadSkins()
    {
        if (!PlayerServiceConnections.instance.ServicesReady) return;

        await SkinShop.Refresh();
        int counter = 0;
        foreach (var s in SkinShop.AvailableItems)
        {
            skinDolls[counter].Set(s.Value);
            counter++;
        }
        for (; counter < skinDolls.Count; counter++)
        {
            skinDolls[counter].Hide();
        }
    }


}
