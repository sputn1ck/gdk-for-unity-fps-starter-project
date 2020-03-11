using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevServiceTestBehaviour : MonoBehaviour
{

    public string targetPubkey;
    public long targetAmount;
    public bool sendKeysend;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(sendKeysend)
        {
            sendKeysend = false;
            SendKeysend(targetPubkey, targetAmount);
        }
    }

    async void SendKeysend(string target, long amount)
    {
        var res = await ServerServiceConnections.instance.lnd.KeysendBufferDeposit(target, amount);
        Debug.LogFormat("Payment: error: {0} preimage: {1}", res.PaymentError, DonnerUtils.ByteArrayToString(res.PaymentPreimage.ToByteArray()));
    }
}
