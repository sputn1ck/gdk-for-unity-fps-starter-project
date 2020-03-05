using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class RoundTimerUI : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public Image icon;
    float remainingTime;
    public TintColor defaultTint;
    public TintColor hurryTint;
    public float hurryDuration;
    

    void OnTimerUpdate(RoundUpdateEventArgs args)
    {
        icon.sprite = args.gameMode.Icon;
        remainingTime = args.remainingTime;
        timeText.text = TimeSpan.FromSeconds(remainingTime).ToString(@"mm\:ss");

    }

    // Start is called before the first frame update
    void Start()
    {
        ClientEvents.instance.onRoundUpdate.AddListener(OnTimerUpdate);
    }

    public float testTimeRemaining;
    public GameMode testGameMode;
    public bool test;


    // Update is called once per frame
    void Update()
    {
        remainingTime = Mathf.Max(remainingTime - Time.deltaTime,0);
        //timeText.text = TimeSpan.FromSeconds(remainingTime).ToString();
        timeText.text = TimeSpan.FromSeconds(remainingTime).ToString(@"mm\:ss");
        TintColor tc;
        if(remainingTime < hurryDuration+1) tc = hurryTint;
        else tc = defaultTint;
        timeText.color = UITinter.tintDict[tc];

        //test
        if (test)
        {
            test = false;
            RoundUpdateEventArgs args = new RoundUpdateEventArgs { remainingTime = testTimeRemaining, gameMode = testGameMode };
            ClientEvents.instance.onRoundUpdate.Invoke(args);
        }    
    }
}
