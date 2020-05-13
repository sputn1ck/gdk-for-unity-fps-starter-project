using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SimpleGameConnector : IGameConnector
{
    private string _host;
    private ClientWorkerConnectorLnd _connector;
    public SimpleGameConnector(string loginHost)
    {
        _host = loginHost;
    }
    // Tries to connect to a Game Server, if all goes well it returns OK, else returns a descriptive error message
    public async Task<JoinGameResponse> JoinGame(ClientWorkerConnectorLnd connector)
    {
        _connector = connector;
        var pitRes = await GetPit();
        if (!pitRes.ok)
        {
            return new JoinGameResponse(false, pitRes.errMsg);
        }
        var deploymentRes = await GetSuitableDeployment();
        if (!deploymentRes.ok)
        {
            return new JoinGameResponse(false, deploymentRes.errMsg);
        }
        var loginRes = await GetLoginToken(deploymentRes.deployment, pitRes.pit);
        if (!loginRes.ok)
        {
            return new JoinGameResponse(false, pitRes.errMsg);
        }
        try
        {
            await connector.Connect(deploymentRes.deployment, loginRes.loginToken, pitRes.pit, false);
        } catch(Exception e)
        {
            return new JoinGameResponse(true, e.Message);
        }
        return new JoinGameResponse(true, "");
    }

    private async Task<(bool ok, string pit, string errMsg)> GetPit()
    {
        var message = "heyho";
        var sigfunc = PlayerServiceConnections.instance.lnd.SignMessage(message);
        var webReq = new AwaitRequestText(_connector, this._host + "/spatial/pit/" + message + "/" + sigfunc.Signature);
        var webRes = await webReq.GetResult();
        if (webRes.hasError)
        {
            return (false, "", webRes.error);
        }
        // TODO Get Pit here
        var pit = webRes.response;
        return (true, pit, "");
    }

    private async Task<(bool ok, string deployment, string errMsg)> GetSuitableDeployment()
    {

        var webReq = new AwaitRequestText(_connector, this._host + "/spatial/deployments/");
        var webRes = await webReq.GetResult();
        if (webRes.hasError)
        {
            return (false, "", webRes.error);
        }
        var res = JsonUtility.FromJson<DeploymentList>(webRes.response);
        var deployment = res.deployments.FirstOrDefault(d => d.status == 200);
        if (deployment == null || deployment.tags[2] != PlayerServiceConnections.instance.GameVersion)
        {
            return (false, "", "all servers offline");
        }
        return (true, deployment.id,"");
    }
    private async Task<(bool ok, string loginToken, string errMsg)> GetLoginToken(string deploymentId, string pit)
    {

        var webReq = new AwaitRequestText(_connector, this._host + "/spatial/login/" + pit + "/" + deploymentId);
        var webRes = await webReq.GetResult();
        if (webRes.hasError)
        {
            return (false, "", webRes.error);
        }
        var loginToken = webRes.response;
        return (true, loginToken, "");
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
    public List<string> tags;
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

