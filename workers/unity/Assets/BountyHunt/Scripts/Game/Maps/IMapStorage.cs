using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapStorage
{
    Map GetMap(string mapId);
}