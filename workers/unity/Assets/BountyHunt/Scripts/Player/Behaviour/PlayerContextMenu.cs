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

    private void Awake()
    {
        mainID = Utility.GetUniqueString();
        subMenuID = Utility.GetUniqueString();
    }
    public void OnLookAtEnter()
    {
        List<(UnityAction, string)> actions = new List<(UnityAction, string)>();
        (UnityAction, string) bountyIncreaseMenuAction = (OpenPlayerBountyIncreaseMenu, GameText.IncreasePlayerBountyContextMenuActionLabel);
        actions.Add(bountyIncreaseMenuAction);
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
        
        actions.Add(IncreasePlayerbountyActionString(100));
        actions.Add(IncreasePlayerbountyActionString(500));
        actions.Add(IncreasePlayerbountyActionString(1000));
        actions.Add(IncreasePlayerbountyActionString(5000));
        actions.Add(IncreasePlayerbountyActionString(10000));
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
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }

        if (balance < sats)
        {
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
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }
        ChatPanelUI.instance.SpawnMessage(Chat.MessageType.INFO_LOG, "Info", GameText.PaymentSuccesfullAnnouncement, true);

    }
}
