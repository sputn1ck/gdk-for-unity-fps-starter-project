using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendPlayerBehaviour : MonoBehaviour
{

    public static BackendPlayerBehaviour instance;

    public BackendPlayerClient client;

    public string target;
    public bool getHighscoreTrigger;
    private void Awake()
    {

        instance = this;
        client = new BackendPlayerClient();
        client.Setup(target);
    }

    private void Update()
    {
        if (getHighscoreTrigger)
        {
            getHighscoreTrigger = false;
            getHighscore();
        }
    }

    private async void getHighscore()
    {
        var res = await client.GetHighscore();
        var s = "";
        foreach(var h in res)
        {
            s += h.Name + ";" + h.Earnings + ";" + h.Kills + ";" + h.Deaths + "\n";
        }
        s = s.Replace("\n", System.Environment.NewLine);
        Debug.Log(s);
    }
    private void OnApplicationQuit()
    {
        client.Shutdown();
     
    }
}
