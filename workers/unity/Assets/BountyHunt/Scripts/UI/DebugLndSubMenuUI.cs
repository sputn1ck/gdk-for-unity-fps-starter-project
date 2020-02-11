using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using System;

public class DebugLndSubMenuUI : SubMenuUI
{
    public TMP_InputField commandInput;
    public TMP_InputField commandOutput;
    public Button SendCommandButton;


    public void Awake()
    {
        SendCommandButton.onClick.AddListener(SendCommand);
        commandInput.onSubmit.AddListener(SendCommand);
    }

    public async void SendCommand()
    {
       SendCommand(commandInput.text);
    }

    public async void SendCommand(string command)
    {
        try
        {
            var res = await PlayerServiceConnections.instance.DonnerDaemonClient.Lncli(command);
            Debug.Log(res);
            commandOutput.text = res;
            commandOutput.Rebuild(CanvasUpdate.PostLayout);
        } catch(Exception e)
        {

            commandOutput.text = "Type: help /n error: " + e;
        }
        
    }

}
