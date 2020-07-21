using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServerRemover
{
    MonoBehaviour GetMonoBehaviour();
}

public interface IClientInitializer
{
    void Initialize();
}

public abstract class MapClientOnlyBehaviour : MonoBehaviour, IServerRemover
{
    public MonoBehaviour GetMonoBehaviour()
    {
        return this;
    }
}

