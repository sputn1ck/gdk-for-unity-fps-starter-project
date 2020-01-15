using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(Toggle))]
public class ServerListElementUI : MonoBehaviour
{
    Toggle toggle;
    public TextMeshProUGUI serverNameText;
    public TextMeshProUGUI regionText;
    public TextMeshProUGUI playerCountText;

    [HideInInspector] public DeploymentJson deployment;


    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }
    public void UpdateServer(DeploymentJson deployment)
    {
        gameObject.SetActive(true);
        this.deployment = deployment;
        serverNameText.text = deployment.name;
        regionText.text = deployment.regionCode;
        playerCountText.text = deployment.playerInfo.activePlayers + "/" + deployment.playerInfo.capacity;

    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        deployment = null;
        serverNameText.text = "";
    }

}
