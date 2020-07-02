using Bountyhunt;
using Improbable.Gdk.Subscriptions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerContextMenu : MonoBehaviour, ILookAtHandler
{

    [Require] HunterComponentReader hunterComponentReader;

    string mainID;
    string subMenuID;

    long walletBalance;

    private void Awake()
    {
        mainID = Utility.GetUniqueString();
        subMenuID = Utility.GetUniqueString();
    }
    public async void OnLookAtEnter()
    {

        List<(UnityAction, string)> actions = new List<(UnityAction, string)>();
        try
        {
            walletBalance = await PlayerServiceConnections.instance.lnd.GetWalletBalace();
            if (walletBalance >= 100)
            {
                (UnityAction, string) bountyIncreaseMenuAction = (OpenPlayerBountyIncreaseMenu, GameText.IncreasePlayerBountyContextMenuActionLabel);
                actions.Add(bountyIncreaseMenuAction);
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }


        string text = string.Format(GameText.PlayerContextMenuText, Utility.SatsToShortString(hunterComponentReader.Data.Bounty, true, UITinter.tintDict[TintColor.Sats]));
        ContextMenuArgs args = new ContextMenuArgs
        {
            ReferenceString = mainID,
            Headline = hunterComponentReader.Data.Name,
            Text = text,
            Actions =  actions
        };
        ContextMenuUI.Instance.Set(args);
    }


    public void OnLookAtExit()
    {
        ContextMenuUI.Instance.Hide(mainID);
    }

    void OpenPlayerBountyIncreaseMenu()
    {
        ContextMenuUI.Instance.Hide(mainID);

        List<(UnityAction, string)> actions = new List<(UnityAction, string)>();

        AddBountyIncreaseActionToList(ref actions, 100);
        AddBountyIncreaseActionToList(ref actions, 500);
        AddBountyIncreaseActionToList(ref actions, 1000);
        AddBountyIncreaseActionToList(ref actions, 5000);
        AddBountyIncreaseActionToList(ref actions, 10000);
        string text = GameText.IncreasePlayerBountyContextMenuText;
        ContextMenuArgs args = new ContextMenuArgs
        {
            ReferenceString = subMenuID,
            Headline = hunterComponentReader.Data.Name,
            Text = text,
            Actions = actions
        };
        ContextMenuUI.Instance.Set(args);
    }

    void AddBountyIncreaseActionToList(ref List<(UnityAction, string)> list,long value)
    {
        if(walletBalance >= value) list.Add(IncreasePlayerbountyActionString(value));
    }

    (UnityAction, string) IncreasePlayerbountyActionString(long sats)
    {
        UnityAction action = () => IncreasePlayerBounty(sats);
        string label = Utility.SatsToShortString(sats, true, UITinter.tintDict[TintColor.Sats]);
        return (action, label);
    }

    async void IncreasePlayerBounty(long sats)
    {
        ContextMenuUI.Instance.Hide(subMenuID);
        long balance;
        try
        {
            balance = await PlayerServiceConnections.instance.lnd.GetWalletBalace();
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }

        if (balance < sats)
        {
            Debug.Log("balance to low");
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "failure", GameText.BalanceToLowAnnouncement, true);
            return;
        }

        try
        {
            string serverpubkey = PlayerServiceConnections.instance.BackendPubkey;
            var res = await PlayerServiceConnections.instance.lnd.KeysendBountyIncrease(serverpubkey, hunterComponentReader.Data.Pubkey, sats);
            if (res.PaymentError != "")
            {
                throw new Exception(res.PaymentError);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }
        ChatPanelUI.instance.SpawnMessage(Chat.MessageType.INFO_LOG, "Info", GameText.PaymentSuccesfullAnnouncement, true);

    }
}
