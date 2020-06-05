using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "BBH/GameModes/Dictionary", order = 0)]
public class GameModeDictionary : ScriptableObject
{
    public static GameModeDictionary Instance;

    [SerializeField] private GameMode[] gameModes;
    // Start is called before the first frame update

    public static GameMode Get(string id)
    {
        if (Instance == null)
        {
            Debug.LogError("The Gun Dictionary has not been set.");
            return null;
        }

        GameMode gm = Instance.gameModes.FirstOrDefault(g => g.GameModeId == id);
        if (gm == null)
        {
            gm = new LobbyGameMode();
        }
        return gm;
    }

}
