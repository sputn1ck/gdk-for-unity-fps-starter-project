using System.Collections;
using System.Collections.Generic;
using Fps.Config;
using UnityEngine;

public class GameModeDictionaryPublisher : MonoBehaviour, ISettingsPublisher
{
    [SerializeField] private GameModeDictionary gameModeDictionary;

    public void Publish()
    {
        GameModeDictionary.Instance = gameModeDictionary;
    }
}
