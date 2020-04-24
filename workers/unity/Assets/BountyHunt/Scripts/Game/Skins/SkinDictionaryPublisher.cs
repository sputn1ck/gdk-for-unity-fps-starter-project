using Fps.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinDictionaryPublisher : MonoBehaviour, ISettingsPublisher
{
    [SerializeField] private SkinsLibrary SkinLibrary;

    public void Publish()
    {
        SkinsLibrary.Instance = SkinLibrary;
    }
}
