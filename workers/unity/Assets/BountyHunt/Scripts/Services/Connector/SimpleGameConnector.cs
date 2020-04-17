using System.Collections;
using System.Collections.Generic;
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
    // Tries to connect to a Game Server, if all goes well it returns OK, else 
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
        return new JoinGameResponse(false, "not implemented");
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
        // Get Pit here
        var pit = webRes.response;
        return (true, pit, "");
    }

    private async Task<(bool ok, string deployment, string errMsg)> GetSuitableDeployment()
    {
        return (false, "","");
    }
    private async Task<(bool ok, string loginToken, string errMsg)> GetLoginToken(string deployment, string pit)
    {
        return (false, "", "");
    }
    
}
