using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMap 
{
    void Initialize(bool isServer, Vector3 spawnPosition);
}
