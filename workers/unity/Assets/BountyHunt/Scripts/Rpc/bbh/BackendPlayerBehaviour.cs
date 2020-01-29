using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendPlayerBehaviour : MonoBehaviour
{

    public static BackendPlayerBehaviour instance;

    public BackendPlayerClient client;

    public string target;
    private void Awake()
    {

        instance = this;
        client = new BackendPlayerClient();
        client.Setup(target);
    }

    private void OnApplicationQuit()
    {
        client.Shutdown();
     
    }
}
