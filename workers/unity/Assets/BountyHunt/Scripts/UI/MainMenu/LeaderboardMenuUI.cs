using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardMenuUI : MonoBehaviour
{
    public LeaderboardEntryUI headLine;
    public Transform entryContainer;
    List<LeaderboardEntryUI> entries;

    // Start is called before the first frame update
    void Start()
    {
        entries = entryContainer.GetComponentsInChildren<LeaderboardEntryUI>().ToList();
        headLine.Set(0, "Player", new List<string> { "kills", "deaths", "earnings", "other" });

        int counter = 0;
        foreach (LeaderboardEntryUI entry in entries)
        {
            counter++;
            entry.Set(counter, "player" + counter, new List<string> { Random.Range(0, 100).ToString(), Random.Range(0, 100).ToString(), Utility.SatsToShortString(Random.Range(0, 130000000),true), "something" });
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
