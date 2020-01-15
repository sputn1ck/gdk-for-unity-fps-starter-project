using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class ServerSubMenuUI : SubMenuUI
{
    public ToggleGroup toggleGroup;

    ServerListElementUI[] listElements;
    public Transform serverListElementContainer;
    public TextMeshProUGUI errorMessage;
    public Button refreshButton;
    public Button joinServerButton;
    public Button backButton;

    public SubMenuUI connectMenu;
    public SubMenuUI spawnMenu;

    private void Start()
    {
        refreshButton.onClick.AddListener(RequestServerListUpdate);
        joinServerButton.onClick.AddListener(JoinServer);
        backButton.onClick.AddListener(connectMenu.Select);
    }
    public override void OnSelect()
    {
        listElements = serverListElementContainer.GetComponentsInChildren<ServerListElementUI>();
        base.OnSelect();

        foreach(ServerListElementUI sle in listElements)
        {
            sle.Deactivate();
        }

        RequestServerListUpdate();
        joinServerButton.onClick.AddListener(JoinServer);
    }

    private void RequestServerListUpdate()
    {
        StartCoroutine(RequestDeployments());

    }

    IEnumerator RequestDeployments()
    {
        yield return LndConnector.Instance.ListDeployments();
        List<DeploymentJson> dList = LndConnector.Instance.deploymentList.deployments.Where(d=>d.status == 200 && !d.tag.Contains("sim")).ToList();
        UpdateServerList(dList.ToArray());

    }

    private void UpdateServerList(DeploymentJson[] deployments)
    {

        toggleGroup.SetAllTogglesOff();

        if (deployments.Length == 0)
        {
            foreach (ServerListElementUI le in listElements)
            {
                le.Deactivate();
            }
            errorMessage.text = "no servers online!";
            joinServerButton.interactable = false;
            return;
        }

        errorMessage.text = "";

        for (int i = 0; i < listElements.Length; i++)
        {
            if (deployments.Length > i)
            {
                listElements[i].UpdateServer(deployments[i]);
            }

            else
            {
                listElements[i].Deactivate();
            }
        }

    }

    public void OnAnyListElementToggleValueChanged(bool value)
    {
        if (toggleGroup.AnyTogglesOn())
        {
            DeploymentJson deployment = toggleGroup.ActiveToggles().First().GetComponent<ServerListElementUI>().deployment;
            if(deployment.playerInfo.capacity > deployment.playerInfo.activePlayers)
            {
                joinServerButton.interactable = true;
            }
            else
            {
                errorMessage.text = "server is full";
                joinServerButton.interactable = false;
            }
        }
        else
        {
            joinServerButton.interactable = false;
        }

    }

    void JoinServer()
    {
        if (!toggleGroup.AnyTogglesOn())
        {
            return;
        }

        DeploymentJson deployment = toggleGroup.ActiveToggles().First().GetComponent<ServerListElementUI>().deployment;
        StartCoroutine(JoinEnumerator(deployment.id));

    }

    IEnumerator JoinEnumerator(string deploymentID)
    {
        LndConnector.Instance.deploymentId = deploymentID;
        yield return LndConnector.Instance.GetLoginToken();

        LndConnector.Instance.Connect();
        spawnMenu.Select();
    }

}
