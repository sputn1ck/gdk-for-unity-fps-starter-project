using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerServiceTestBehaviour : MonoBehaviour
{
    [Header("Daemon Info")]
    public bool allInfoTrigger;
    [Header("lncli")]
    public bool lncliTrigger;
    public string command;
    private void Update()
    {
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


    public async void GetAllInfoTrigger()
    {

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
