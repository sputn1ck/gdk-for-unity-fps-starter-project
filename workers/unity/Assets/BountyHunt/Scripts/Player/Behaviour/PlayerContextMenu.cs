using Bbhrpc;
using Bountyhunt;
using Fps;
using Improbable.Gdk.Subscriptions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerContextMenu : MonoBehaviour, ILookAtHandler
{

    [Require] HunterComponentReader hunterComponentReader;
    [Require] HealthComponentReader healthComopnentReader;

    long walletBalance;

    private bool isShowing;
    public void OnEnable()
    {
        hunterComponentReader.OnBountyUpdate += UpdateBounty;
        healthComopnentReader.OnHealthUpdate += UpdateHealth;
    }
    public async void OnLookAtEnter()
    {

        
        List<(UnityAction, string)> actions = new List<(UnityAction, string)>();
        try
        {

            var balanceReq = await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance();
            walletBalance = balanceReq.DaemonBalance;
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

        Ranking ranking;
        try
        {
            ranking = await PlayerServiceConnections.instance.BackendPlayerClient.GetSpecificPlayerRanking(hunterComponentReader.Data.Pubkey);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }

        Badge badge = BadgeManager.GetBadge(ranking.GlobalRanking.Badge);

        string text = string.Format(GameText.PlayerContextMenuText, healthComopnentReader.Data.Health ,Utility.SatsToShortString(hunterComponentReader.Data.Bounty, true, UITinter.tintDict[TintColor.Sats]));
        ContextMenuArgs args = new ContextMenuArgs
        {
            Headline = hunterComponentReader.Data.Name,
            Text = text,
            Actions = actions,
            ImageSprite = badge.sprite,
            ImageColor = badge.color,
            OpenAction = Subscribe,
            CloseAction = Unsubscribe,
            Type = ContextMenuType.LOOKAT
        };
        ContextMenuUI.Instance.Set(args);
        isShowing = true;
    }

    public void OnLookAtExit()
    {
        isShowing = false;
        ContextMenuUI.Instance.UnsetLookAtMenu();
    }

    public void OnDisable()
    {
        if(isShowing)
            ContextMenuUI.Instance.UnsetLookAtMenu();
    }

    void OpenPlayerBountyIncreaseMenu()
    {
        List<(UnityAction, string)> actions = new List<(UnityAction, string)>();

        AddBountyIncreaseActionToList(ref actions, 100);
        AddBountyIncreaseActionToList(ref actions, 500);
        AddBountyIncreaseActionToList(ref actions, 1000);
        AddBountyIncreaseActionToList(ref actions, 5000);
        AddBountyIncreaseActionToList(ref actions, 10000);
        string text = GameText.IncreasePlayerBountyContextMenuText;
        ContextMenuArgs args = new ContextMenuArgs
        {
            Headline = hunterComponentReader.Data.Name,
            Text = text,
            Actions = actions,
            Type = ContextMenuType.REPLACE
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
        ContextMenuUI.Instance.CloseCurrentAndShowNext();
        
        try
        {
            var invoice = await PlayerServiceConnections.instance.BackendPlayerClient.GetBountyInvoice(hunterComponentReader.Data.Pubkey, sats);
            var res = await PlayerServiceConnections.instance.lnd.PayInvoice(invoice);
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
        ChatPanelUI.instance.SpawnMessage(Chat.MessageType.INFO_LOG, "Info", GameText.PaymentSuccessful, true);

    }

    void Subscribe()
    {
    }
    void Unsubscribe()
    {
    }
    void UpdateHealth(float health)
    {
        UpdateText();
    }
    void UpdateBounty(long bounty)
    {
        UpdateText();
    }
    void UpdateText()
    {
        OnLookAtEnter();
        //string text = string.Format(GameText.PlayerContextMenuText, healthComopnentReader.Data.Health, Utility.SatsToShortString(hunterComponentReader.Data.Bounty, true, UITinter.tintDict[TintColor.Sats]));
        //ContextMenuUI.Instance.UpdateText(text, mainID);
    }
}
