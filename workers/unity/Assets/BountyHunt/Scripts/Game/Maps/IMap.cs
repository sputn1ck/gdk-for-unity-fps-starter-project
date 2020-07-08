using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMap 
{
    void Initialize(MonoBehaviour caller, bool isServer, Vector3 spawnPosition, string mapData);
}
