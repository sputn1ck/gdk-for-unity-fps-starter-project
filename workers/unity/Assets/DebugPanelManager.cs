using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelManager : MonoBehaviour
{
    public DebugUiPanel balancePanel;
    public DebugUiPanel bountyPanel;
    public DebugUiPanel sessionEarningsPanel;
    public DebugUiPanel bountyInPlayersPanel;
    public DebugUiPanel bountyInCubesPanel;
    public DebugUiPanel carryOverSatsPanel;
    public DebugUiPanel RemainingPotPanel;
    void Start()
    {
        ClientEvents.instance.onBalanceUpdate.AddListener((BalanceUpdateEventArgs e) =>
        {
            balancePanel.UpdateText("Balance", e.NewAmount.ToString());
        });

        ClientEvents.instance.onBountyUpdate.AddListener((BountyUpdateEventArgs e) =>
        {
            bountyPanel.UpdateText("Bounty", e.NewAmount.ToString());
        });
        ClientEvents.instance.onSessionEarningsUpdate.AddListener((SessionEarningsEventArgs e) =>
        {
            sessionEarningsPanel.UpdateText("Session Earnings", e.NewAmount.ToString());
        });
        ClientEvents.instance.onBountyInPlayersUpdate.AddListener((long e) =>
        {
            bountyInPlayersPanel.UpdateText("Bounty in Players", e.ToString());
        });
        ClientEvents.instance.onBountyinCubesUpdate.AddListener((long e) =>
        {
            bountyInCubesPanel.UpdateText("Bounty in Cubes", e.ToString());
        });
        ClientEvents.instance.onCarryoverSatsUpdate.AddListener((long e) =>
        {
            carryOverSatsPanel.UpdateText("Carryover Sats", e.ToString());
        });
        ClientEvents.instance.onRemainingPotUpdate.AddListener((long e) =>
        {
            RemainingPotPanel.UpdateText("Remaining Pot", e.ToString());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
