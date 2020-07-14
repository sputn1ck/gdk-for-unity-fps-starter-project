using Bountyhunt;
using Fps.Movement;
using Improbable.Gdk.Subscriptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class SkinContextMenu : MonoBehaviour ,ILookAtHandler
{
    [Require] HunterComponentCommandSender hunterCommandSender;

    public List<Renderer> bodyRenderers;
    string uniqueID;
    SkinItem item;

    private void Awake()
    {
        uniqueID = Utility.GetUniqueString();
    }

    public void Set (SkinItem item)
    {
        gameObject.SetActive(true);
        this.item = item;
        SetSkin(item.skin);
    }

    public void SetSkin(Skin skin)
    {
        if (skin.material != null)
        {
            foreach (var renderer in bodyRenderers)
            {
                renderer.material = skin.material;
            }
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public async void OnLookAtEnter()
    {
        ContextMenuArgs args;
        try
        {
            args = await CreateContextMenuArgs();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }
        ContextMenuUI.Instance.Set(args);
    }

    public async void RefreshContextMenu()
    {
        ContextMenuArgs args;
        try
        {
            args = await CreateContextMenuArgs();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }
        ContextMenuUI.Instance.UpdateAll(args);
    }

    async Task<ContextMenuArgs> CreateContextMenuArgs()
    {
        List<(UnityAction action, string label)> actions = new List<(UnityAction action, string label)>();
        string text = "";
        if (item.owned)
        {
            if (SkinShop.EquippedSkin == item)
            {
                text = GameText.SkinEquippedContextMenuText;
            }
            else
            {
                actions.Add((Equip, GameText.EquipSkinContextMenuAction));
            }
        }
        else
        {
            text = Utility.SatsToShortString(item.price,true,UITinter.tintDict[TintColor.Sats]);

            long balance;
            try
            {
                balance = await PlayerServiceConnections.instance.lnd.GetWalletBalace();
            }
            catch (Exception e)
            {
                throw (e);
            }

            if (balance >= item.price)
            {
                actions.Add((buy, GameText.BuySkinContextMenuAction));
            }

        }

        var args = new ContextMenuArgs
        {
            ReferenceString = uniqueID,
            Headline = item.skin.group.groupName,
            Text = text,
            Actions = actions,
            ImageSprite = item.skin.group.sprite,
            ImageColor =  item.skin.identificationColor
        };

        return args;

    }

    public void OnLookAtExit()
    {
        ContextMenuUI.Instance.Hide(uniqueID);
    }

    async void Equip()
    {

        try
        {
            await PlayerServiceConnections.instance.BackendPlayerClient.EquipSkin(item.skin.ID);
            await SkinShop.Refresh();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }
        RefreshContextMenu();
        //FpsDriver.instance.GetComponent<SkinChangeBehaviour>().UpdateSkin(SkinShop.EquippedSkin.skin.ID);
        FpsDriver.instance.GetComponent<BountyPlayerAuthorative>().RefreshAppearance();
    }
    async void buy()
    {
        long balance;
        try
        {
            balance = await PlayerServiceConnections.instance.lnd.GetWalletBalace();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }

        if (balance < item.price)
        {
            Debug.Log("balance to low");
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "failure", GameText.BalanceToLowAnnouncement, true);
            return;
        }

        string invoice;
        try
        {
            
            invoice = await PlayerServiceConnections.instance.BackendPlayerClient.GetSkinInvoice(item.skin.ID);

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }

        try
        {
            await PlayerServiceConnections.instance.lnd.PayInvoice(invoice);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }

        ChatPanelUI.instance.SpawnMessage(Chat.MessageType.INFO_LOG, "Info", GameText.PaymentSuccesfullAnnouncement, true);

        try
        {
            await SkinShop.Refresh();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }

        RefreshContextMenu();

    }
}
