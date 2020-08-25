using Bbhrpc;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "LobbyGameMode", menuName = "BBH/GameModes/Lobby", order = 1)]
public class LobbyGameMode : GameMode
{
    private ServerRoomGameModeBehaviour _serverGameModeBehaviour;

    public override async void ServerOnGameModeStart(ServerRoomGameModeBehaviour serverGameModeBehaviour)
    {
        Debug.Log("start lobby");
        _serverGameModeBehaviour = serverGameModeBehaviour;
        //ServerGameChat.instance.SendAuctionStartedChatMessage("new auction started, check escape menu to participate");

    }

    public override async void ServerOnGameModeEnd(ServerRoomGameModeBehaviour serverGameModeBehaviour)
    {
        Debug.Log("end lobby");
        await Task.Delay(2000);
    }


    private void AddCarryOverSats(long sats)
    {
        /*
        _serverGameModeBehaviour.GameStatsWriter.SendUpdate(new Bountyhunt.GameStats.Update
        {
            CarryoverSats = _serverGameModeBehaviour.GameStatsWriter.Data.CarryoverSats + sats
        });*/
    }



    public override void GameModeUpdate(float deltaTime)
    {
    }

    public override void OnPlayerJoin(string playerId)
    {
    }

    public override void OnPlayerLeave(string playerId)
    {
    }
}

