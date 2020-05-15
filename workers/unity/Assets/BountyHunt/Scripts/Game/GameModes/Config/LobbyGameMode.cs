using Bbhrpc;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "LobbyGameMode", menuName = "BBH/GameModes/Lobby", order = 1)]
public class LobbyGameMode : GameMode
{
    private ServerGameModeBehaviour _serverGameModeBehaviour;

    public override async void ServerOnGameModeStart(ServerGameModeBehaviour serverGameModeBehaviour, GameModeSettings settings, long subsidy)
    {
        Debug.Log("start lobby");
        this.GameModeSettings = settings;
        _serverGameModeBehaviour = serverGameModeBehaviour;
        //ServerGameChat.instance.SendAuctionStartedChatMessage("new auction started, check escape menu to participate");

    }

    public override async void ServerOnGameModeEnd(ServerGameModeBehaviour serverGameModeBehaviour)
    {
        Debug.Log("end lobby");
        await Task.Delay(2000);
    }


    private void AddCarryOverSats(long sats)
    {
        _serverGameModeBehaviour.GameStatsWriter.SendUpdate(new Bountyhunt.GameStats.Update
        {
            CarryoverSats = _serverGameModeBehaviour.GameStatsWriter.Data.CarryoverSats + sats
        });
    }


    public override void ClientOnGameModeStart(ClientGameModeBehaviour clientGameModeBehaviour)
    {
        
    }

    public override void ClientOnGameModeEnd(ClientGameModeBehaviour clientGameModeBehaviour)
    {
        
    }
}

