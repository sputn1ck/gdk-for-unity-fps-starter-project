using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerServiceTestBehaviour : MonoBehaviour
{
    [Header("Daemon Invoice")]
    public bool getInvoiceTrigger;
    public string invoicememo;
    public long amount;
    [Header("Daemon Info")]
    public bool allInfoTrigger;
    [Header("lncli")]
    public bool lncliTrigger;
    public string command;
    private void Update()
    {
        if (getInvoiceTrigger)
        {
            getInvoiceTrigger = false;
            GetInvoiceTrigger();
        }
        if (allInfoTrigger)
        {
            allInfoTrigger = false;
            GetAllInfoTrigger();
        }
        if (lncliTrigger)
        {
            lncliTrigger = false;
            Lncli();
        }
    }

    public async void GetInvoiceTrigger()
    {
        var res = await PlayerServiceConnections.instance.DonnerDaemonClient.GetInvoice(invoicememo, amount);
        Debug.Log(res);
    }

    public async void GetAllInfoTrigger()
    {
        var info = await PlayerServiceConnections.instance.DonnerDaemonClient.GetInfo();
        Debug.Log(info);
        var connection = await PlayerServiceConnections.instance.DonnerDaemonClient.GetConnection();
        Debug.Log(connection);
        var walletBalance = await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance();
        Debug.Log(walletBalance);
        //var channel = await PlayerServiceConnections.instance.DonnerDaemonClient.GetPlatformChannel();
        //Debug.Log(channel);
    }

    public async void Lncli()
    {
        var lncli = await PlayerServiceConnections.instance.DonnerDaemonClient.Lncli(command);
        Debug.Log(lncli);
    }
}
