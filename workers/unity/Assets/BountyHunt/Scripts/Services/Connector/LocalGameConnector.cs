using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LocalGameConnector : IGameConnector
{
    public async Task<JoinGameResponse> JoinGame(ClientWorkerConnectorLnd connector)
    {
        await connector.Connect("", "", "", true);
        return new JoinGameResponse(true, "");
    }
}
