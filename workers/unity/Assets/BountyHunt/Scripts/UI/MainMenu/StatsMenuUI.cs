using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsMenuUI : MonoBehaviour
{
    public Transform PlayerStatsContainer;
    public Transform GameStatsContainer;
    public StatsValueUI statsValuePrefab;
    Dictionary<string, StatsValueUI> stats = new Dictionary<string, StatsValueUI>();


    void Start()
    {
        WritePlayerStats();
        WriteGameStats();
        AddListeners();
    }

    void WritePlayerStats()
    {
        Transform ctr = PlayerStatsContainer;

        NewLine("Lifetime Kills", "0", ctr);
        NewLine("Lifetime Earnings", "0", ctr);
        NewLine("Lifetime Deaths", "0", ctr);

    }

    void WriteGameStats()
    {
        Transform ctr = GameStatsContainer;

        NewLine("Total Players Lifetime Kills", "0", ctr);
        NewLine("Total Players Lifetime Earnings", "0", ctr);
    }

    void AddListeners()
    {

    }

    void NewLine(string name, string value, Transform container,string key = "")
    {
        if (key == "") key = name;
        stats[key] = Instantiate(statsValuePrefab, container);
        stats[key].Set(name, value);
    }

}
