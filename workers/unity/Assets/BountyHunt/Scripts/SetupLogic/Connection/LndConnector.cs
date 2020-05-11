
using Fps;
using Fps.UI;
using Improbable.Gdk.PlayerLifecycle;
using Improbable.Worker.CInterop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class LndConnector : MonoBehaviour
{
    public bool connectLocal;
    public string host;

    public GameObject ClientWorkerConnectorPrefab;
    public string pubkey;

    public ClientWorkerConnectorLnd clientConnector;

    public static LndConnector Instance;

    public DeploymentList deploymentList;

    private IGameConnector gameConnector;
    private void Awake()
    {
        if (!Instance) Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public async Task Connect()
    {
        var clientWorker = UnityEngine.Object.Instantiate(ClientWorkerConnectorPrefab, this.transform.position, Quaternion.identity);
        clientConnector = clientWorker.GetComponent<ClientWorkerConnectorLnd>();
        clientConnector.OnWorkerCreationFinished += GetComponent<ClientFlagManager>().WorkerCreated;
        clientConnector.OnWorkerCreationFinished += ClientConnector_OnWorkerCreationFinished1;
        if(connectLocal)
        {
            gameConnector = new LocalGameConnector();
        }else
        {
            gameConnector = new SimpleGameConnector(host);
        }
        JoinGameResponse res = new JoinGameResponse { Ok = false};
        try
        {
            res = await gameConnector.JoinGame(clientConnector);
        }
        catch(Exception e)
        {
            throw new Exception("Exception: " + e.Message);
        }
        
        if (!res.Ok)
        {
            throw new Exception(res.ErrorMessage);
        }
    }
    
    public void Disconnect()
    {
        clientConnector.DisconnectPlayer();
        BBHUIManager.instance.ShowMainMenu();
    }

   

    private void ClientConnector_OnWorkerCreationFinished1(Improbable.Gdk.Core.Worker obj)
    {
        Debug.Log("worker created");
        clientConnector.Worker.OnDisconnect += Worker_OnDisconnect;
    }

    private void Worker_OnDisconnect(string obj)
    {
        Debug.Log("Disconnected");
        BBHUIManager.instance.ShowMainMenu();
    }

    public void SpawnPlayer(string playername, int gunId)
    {
        Debug.Log("trying to spawn player");
        clientConnector.SpawnPlayer(playername, gunId, OnPlayerResponse);
    }



    private void OnPlayerResponse(PlayerCreator.CreatePlayer.ReceivedResponse response)
    {
        //Manager.InGameManager.Timer.SetActive(false);
        BBHUIManager.instance.ShowGameView();

        ClientEvents.instance.onGameJoined.Invoke();
    }

}
