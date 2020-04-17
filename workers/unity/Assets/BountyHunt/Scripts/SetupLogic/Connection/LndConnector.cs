
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
    public int gunId;
    public bool connectLocal;
    public string host;
    public string playername;
    public string pit;
    public string deploymentId;
    public string loginToken;

    public bool getPitTrigger;
    public bool listDeploymentsTrigger;
    public bool loginTokenTrigger;
    public bool connectTrigger;
    public bool spawnPlayerTrigger;
    public bool doAllTrigger;
    public GameObject ClientWorkerConnectorPrefab;
    public string pubkey;
    public GameObject mainMenu;

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
    
    public async Task<string> NewConnect()
    {
        var clientWorker = UnityEngine.Object.Instantiate(ClientWorkerConnectorPrefab, this.transform.position, Quaternion.identity);
        clientConnector = clientWorker.GetComponent<ClientWorkerConnectorLnd>();
        clientConnector.OnWorkerCreationFinished += GetComponent<ClientFlagManager>().WorkerCreated;
        clientConnector.OnWorkerCreationFinished += ClientConnector_OnWorkerCreationFinished1;
        if(connectLocal)
        {
            gameConnector = new LocalGameConnector();
        }
        var res = await gameConnector.JoinGame(clientConnector);
        if (!res.Ok)
        {
            return res.ErrorMessage;
        }
        return "";
    }
    void NewSpawn(string Playername, int GunId)
    {
        Debug.Log("trying to spawn player");
        clientConnector.SpawnPlayer(Playername, GunId, OnPlayerResponse);
    }
    // Update is called once per frame
    void Update()
    {
        if (getPitTrigger)
        {
            getPitTrigger = false;
            StartCoroutine(GetPit());
        }
        if(listDeploymentsTrigger)
        {
            listDeploymentsTrigger = false;
            StartCoroutine(ListDeployments());
        }
        if(loginTokenTrigger)
        {
            loginTokenTrigger = false;
            StartCoroutine(GetLoginToken());

        }

        if(connectTrigger)
        {
            connectTrigger = false;
            Connect();
        }
        if(spawnPlayerTrigger)
        {
            spawnPlayerTrigger = false;
            SpawnPlayer(pubkey,0);
        }
    }

    
    public void Disconnect()
    {
        clientConnector.DisconnectPlayer();
        mainMenu.SetActive(true);
        BBHUIManager.instance.ShowFrontEnd();
        mainMenu.GetComponent<MenuUI>().Reset();
    }

    public IEnumerator GetPit()
    {
        var message = "heyho";
        
            var sigfunc = PlayerServiceConnections.instance.lnd.SignMessage(message);
        
        UnityWebRequest www = UnityWebRequest.Get(host+"/spatial/pit/" + message+"/"+sigfunc.Signature);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            UnityEngine.Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            UnityEngine.Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            pit = www.downloadHandler.text;
        }
    }

    public IEnumerator ListDeployments()
    {
        UnityWebRequest www = UnityWebRequest.Get(host + "/spatial/deployments/");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            UnityEngine.Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            UnityEngine.Debug.Log(www.downloadHandler.text);
            
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            var deploymentsText = www.downloadHandler.text;
            var res = JsonUtility.FromJson<DeploymentList>(deploymentsText);
            deploymentList = res;
            var deployment = res.deployments.Where(d => d.status == 200 && d.name == "live").FirstOrDefault();
            Debug.Log(deployment);
            /*
            if (deployment == null)
            {
                deploymentId = "empty";
            }else
            {
                deploymentId = deployment.id;
            }
            */
                
        }
    }
    public IEnumerator GetLoginToken()
    {
        UnityWebRequest www = UnityWebRequest.Get(host + "/spatial/login/" + pit+"/"+deploymentId);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            UnityEngine.Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            UnityEngine.Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
            loginToken = www.downloadHandler.text;
        }
    }

    // Here is where we should use the correct Connector and then 
    public async void Connect()
    {
        var clientWorker = UnityEngine.Object.Instantiate(ClientWorkerConnectorPrefab, this.transform.position, Quaternion.identity);
        clientConnector = clientWorker.GetComponent<ClientWorkerConnectorLnd>();
        clientConnector.OnWorkerCreationFinished += GetComponent<ClientFlagManager>().WorkerCreated;
        clientConnector.OnWorkerCreationFinished += ClientConnector_OnWorkerCreationFinished1;
        await clientConnector.Connect(deploymentId, loginToken,pit, connectLocal);
    }

    private void ClientConnector_OnWorkerCreationFinished1(Improbable.Gdk.Core.Worker obj)
    {
        Debug.Log("worker created");
        clientConnector.Worker.OnDisconnect += Worker_OnDisconnect;
    }

    private void Worker_OnDisconnect(string obj)
    {
        Debug.Log("Disconnected");
        mainMenu.SetActive(true);
        BBHUIManager.instance.ShowFrontEnd();
        mainMenu.GetComponent<MenuUI>().Reset();
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
[Serializable]

public class LaunchConfig
{
    public string configJson;
}
[Serializable]

public class WorkerFlag
{
    public string workerType;
    public string key;
    public string value;
}
[Serializable]

public class PlayerInfo
{
    public int activePlayers;
    public int capacity;
    public int queueLength;
}
[Serializable]

public class StartTime
{
    public int seconds;
    public int nanos;
}
[Serializable]

public class WorkerConnectionCapacity
{
    public string workerType;
    public object maxCapacity;
    public object remainingCapacity;
}
[Serializable]

public class Duration
{
    public int seconds;
    public int nanos;
}
[Serializable]

public class WorkerConnectionRateLimit
{
    public string workerType;
    public Duration duration;
    public int requestsInDuration;
}
[Serializable]

public class ExpiryTime
{
    public int seconds;
    public int nanos;
}
[Serializable]

public class DeploymentJson
{
    public string id;
    public string projectName;
    public string name;
    public string regionCode;
    public string clusterCode;
    public string assemblyId;
    public string startingSnapshotId;
    public List<string> tag;
    public int status;
    public LaunchConfig launchConfig;
    public List<WorkerFlag> workerFlags;
    public PlayerInfo playerInfo;
    public StartTime startTime;
    public object stopTime;
    public List<WorkerConnectionCapacity> workerConnectionCapacities;
    public List<WorkerConnectionRateLimit> workerConnectionRateLimits;
    public string description;
    public string runtimeVersion;
    public ExpiryTime expiryTime;
}
[Serializable]
public class DeploymentList
{
    public List<DeploymentJson> deployments;
}
