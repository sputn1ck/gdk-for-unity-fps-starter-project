using Fps.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinDictionaryPublisher : MonoBehaviour, ISettingsPublisher
{
    [SerializeField] private SkinsLibrary MasterSkinsLibrary;

    public void Publish()
    {
        SkinsLibrary.Instance = Object.Instantiate(MasterSkinsLibrary) ;
        SkinsLibrary.Instance.Initialize();
    }
}
