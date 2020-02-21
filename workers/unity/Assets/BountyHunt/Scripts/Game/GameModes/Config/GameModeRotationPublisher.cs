using System.Collections;
using System.Collections.Generic;
using Fps.Config;
using UnityEngine;

public class GameModeRotationPublisher : MonoBehaviour, ISettingsPublisher
{
    [SerializeField] private GameModeRotation gameModeRotation;

    public void Publish()
    {
        GameModeRotation.Instance = gameModeRotation;
    }
}
