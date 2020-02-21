using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "BBH/GameModes/Dictionary", order = 0)]
public class GameModeRotation : ScriptableObject
{
    public static GameModeRotation Instance;

    [SerializeField] private GameMode[] gameModes;
    // Start is called before the first frame update

    public static GameMode Get(int index)
    {
        if (Instance == null)
        {
            Debug.LogError("The Gun Dictionary has not been set.");
            return null;
        }

        if (index < 0 || index >= Count)
        {
            //Debug.LogErrorFormat("The index {0} is outside of the dictionary's range (size {1}).", index, Count);
            return null;
        }

        return Instance.gameModes[index];
    }

    public static int Count => Instance.gameModes.Length;
}
