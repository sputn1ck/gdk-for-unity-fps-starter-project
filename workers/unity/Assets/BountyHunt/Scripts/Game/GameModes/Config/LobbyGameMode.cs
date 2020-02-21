using UnityEngine;


    [CreateAssetMenu(fileName = "LobbyGameMode", menuName = "BBH/GameModes/Lobby", order = 1)]
    public class LobbyGameMode : GameMode
    {
        public override void OnGameModeStart(ServerGameModeBehaviour serverGameModeBehaviour)
        {
            Debug.Log("start lobby");
        }

        public override void OnGameModeEnd(ServerGameModeBehaviour serverGameModeBehaviour)
        {
            Debug.Log("end lobby");
        }
    }

