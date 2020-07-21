using Fps.Movement;
using Improbable.Gdk.Subscriptions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GunSelectContextMenu : MapClientOnlyBehaviour, ILookAtHandler
{
    
    public string gunName;
    public int gunID;

    string uniqueId;

    

    private void Awake()
    {
        uniqueId = Utility.GetUniqueString();
    }

    public  void OnLookAtEnter()
    {
        List<(UnityAction action, string label)> list = new List<(UnityAction action, string label)>();
        string text = GameText.GunEquippedContextMenuText;


        if (FpsDriver.instance.GetGunId() != gunID)
        {
            text = "";
            (UnityAction action, string label) action = (SelectGun, GameText.EquipGunContextMenuAction);
            list.Add(action);
        }

        ContextMenuArgs args = new ContextMenuArgs
        {
            ReferenceString = uniqueId,
            Headline = gunName,
            Actions = list,
            Text = text
        };
        
        ContextMenuUI.Instance.Set(args);
    }

    public void OnLookAtExit()
    {
        ContextMenuUI.Instance.Hide(uniqueId);
    }

    public void SelectGun()
    {
        PlayerPrefs.SetInt("SelectedGunID", gunID);
        PlayerPrefs.Save();
        FpsDriver.instance.ChangeGunId(gunID);

        ContextMenuUI.Instance.Hide(uniqueId);
        OnLookAtEnter();
    }

}
