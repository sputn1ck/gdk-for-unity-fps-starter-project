using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class SkinContextMenu : MonoBehaviour ,ILookAtHandler
{
    public string uniqueID;

    public static Skin EquippedSkin;

    Skin skin;
    public Renderer renderer;

    private void Awake()
    {
        uniqueID = Utility.GetUniqueString();
    }

    void Set (Skin skin)
    {
        this.skin = skin;
        renderer.material = skin.material;
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
        if (skin.owned)
        {
            if (EquippedSkin == skin)
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
            text = Utility.SatsToShortString(skin.price,true,UITinter.tintDict[TintColor.Sats]);

            long balance;
            try
            {
                balance = await PlayerServiceConnections.instance.lnd.GetWalletBalace();
            }
            catch (Exception e)
            {
                throw (e);
            }

            if (balance >= skin.price)
            {
                actions.Add((buy, GameText.BuySkinContextMenuAction));
            }

        }

        var args = new ContextMenuArgs
        {
            ReferenceString = uniqueID,
            Headline = skin.group.groupName,
            Text = text,
            Actions = actions
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
            await PlayerServiceConnections.instance.BackendPlayerClient.EquipSkin(skin.ID);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "error", e.Message, true);
            return;
        }
        EquippedSkin = skin;
        RefreshContextMenu();
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

        if (balance < skin.price)
        {
            Debug.Log("balance to low");
            ChatPanelUI.instance.SpawnMessage(Chat.MessageType.DEBUG_LOG, "failure", GameText.BalanceToLowAnnouncement, true);
            return;
        }

        string invoice;
        try
        {
            
            invoice = await PlayerServiceConnections.instance.BackendPlayerClient.GetSkinInvoice(skin.ID);

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
        RefreshContextMenu();

    }
}
