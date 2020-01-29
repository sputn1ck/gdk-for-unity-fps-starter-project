using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendGameServerBehaviour : MonoBehaviour
{

    public static BackendGameServerBehaviour instance;
    public string BackendHost;

    public int testAmount;
    public string testName;


    public bool testTrigger;

    private BackendGameserverClient backendClient;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        backendClient = new BackendGameserverClient();
        backendClient.Setup(BackendHost);
        backendClient.StartListening();
    }

    private void Update()
    {
        if (testTrigger)
        {
            testTrigger = false;
            for (int i = 0; i < testAmount; i++)
            {
                AddEarnings(testName, 5);
            }
        }
    }

    void AddKill(string killer, string victim)
    {
        backendClient.AddToQueue(new Bbh.EventStreamRequest { Kill = new Bbh.KillEvent() { Killer = killer, Victim = victim } });
    }

    void AddEarnings(string user, long earnings)
    {
        backendClient.AddToQueue(new Bbh.EventStreamRequest { Earnings = new Bbh.EarningsEvent { Amt = earnings, User = user } });
    }

    private void OnApplicationQuit()
    {

        backendClient.Shutdown();
        
    }
}
