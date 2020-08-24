using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySkinShop : MapClientOnlyBehaviour,  IClientInitializer
{
    public List<SkinContextMenu> skinDolls;


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

    public void Initialize()
    {
        Debug.Log("initialize called");
        LoadSkins();
    }

}
