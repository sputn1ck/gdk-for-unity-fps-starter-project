using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LndConnectTest : MonoBehaviour
{
    public string lndConnectString;
    public bool connectLndTrigger;
    public bool getInfoTrigger;
    private IClientLnd lnd;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(connectLndTrigger)
        {
            connectLndTrigger = false;
            ConnectLnd(lndConnectString);

        }
        if(getInfoTrigger)
        {
            getInfoTrigger = false;
            GetInfo();
        }
    }

    private void ConnectLnd(string lndConnectString)
    {
        lnd = new LndClient();
        lnd.Setup("", false, false, "", lndConnectString);
    }

    private async void GetInfo()
    {
        var res = await lnd.GetInfo();
        Debug.Log(res);
    }
}
