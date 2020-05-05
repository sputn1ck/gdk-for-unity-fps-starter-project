using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IGameConnector
{
    Task<JoinGameResponse> JoinGame(ClientWorkerConnectorLnd connector); 
}

public struct JoinGameResponse
{
    public bool Ok;
    public string ErrorMessage;

    public JoinGameResponse(bool ok, string errorMessage)
    {
        this.Ok = ok;
        this.ErrorMessage = errorMessage;
    }
}
