using Bbhrpc;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "LobbyGameMode", menuName = "BBH/GameModes/Lobby", order = 1)]
public class LobbyGameMode : GameMode
{

    public override async void ServerOnGameModeStart()
    {
        Debug.Log("start lobby");
        //ServerGameChat.instance.SendAuctionStartedChatMessage("new auction started, check escape menu to participate");

    }

    public override async void ServerOnGameModeEnd()
    {
        Debug.Log("end lobby");
    }


}

