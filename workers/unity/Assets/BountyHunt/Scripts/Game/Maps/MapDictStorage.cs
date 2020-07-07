using Fps.Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDictStorage : MonoBehaviour, IMapStorage, ISettingsPublisher
{
    public static MapDictStorage Instance;
    public List<Map> MapsToInitialize;
    private Dictionary<string, Map> mapDict;

    public Map GetMap(string mapId)
    {
        if(mapDict.ContainsKey(mapId))
        {
            return mapDict[mapId];
        }
        throw new System.Exception("Map Id not found");
    }

    public void Publish()
    {
        this.mapDict = new Dictionary<string, Map>();
        foreach(var m in MapsToInitialize)
        {
            mapDict.Add(m.MapId, m);
        }
        Instance = this;
    }
}
