using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prometheus;
using System.Net.Http;
using System;
using System.Text;
using System.Threading.Tasks;

public class PrometheusManager
{
    MetricPusher metricPusher;
    HttpClient httpClient;
    string monitoringEndpoint;

    private bool hasStarted;
    // Gauges
    public static readonly Gauge ActivePlayers = Metrics.CreateGauge("bbh_active_players", "active players");
    public static readonly Gauge ActiveBounty = Metrics.CreateGauge("bbh_active_bounty", "active bounty");
    public static readonly Gauge ActiveSats = Metrics.CreateGauge("bbh_active_sats", "active sats");
    public static readonly Gauge ActiveCubes = Metrics.CreateGauge("bbh_active_cubes", "active cubes");

    // Counters
    public static readonly Counter TotalKills = Metrics.CreateCounter("bbh_total_kills", "kills");
    public static readonly Counter TotalEarnings = Metrics.CreateCounter("bbh_total_earnings", "earnings");
    public static readonly Counter TotalSatsAdded = Metrics.CreateCounter("bbh_total_sats_added", "sats added");
    public static readonly Counter TotalSubsidy = Metrics.CreateCounter("bbh_total_subsidy", "total subsidy added");

    // Payment Types
    public static readonly Counter TotalAuctionsPaidAmount = Metrics.CreateCounter("bbh_payments_auctions_amount", "total auctions paid");
    public static readonly Counter TotalAuctionsPaidSats = Metrics.CreateCounter("bbh_payments_auctions_sats", "sats of auctions");

    public static readonly Counter TotalTeleportPaidAmount = Metrics.CreateCounter("bbh_payments_teleport_amount", "total teleports paid");
    public static readonly Counter TotalTeleportPaidSats = Metrics.CreateCounter("bbh_payments_teleport_sats", "sats of teleport");

    public static readonly Counter TotalBountyPaidAmount = Metrics.CreateCounter("bbh_payments_bounty_amount", "total bounties paid");
    public static readonly Counter TotalBountyPaidSats = Metrics.CreateCounter("bbh_payments_bounty_sats", "sats of bounties");

    public static readonly Counter TotalChatPaidAmount = Metrics.CreateCounter("bbh_payments_chat_amount", "total chat paid");
    public static readonly Counter TotalChatPaidSats = Metrics.CreateCounter("bbh_payments_chat_sats", "sats of chats");

    // classes
    public static readonly Counter TotalSoldiersChosen = Metrics.CreateCounter("bbh_total_soldiers", "total soldiers picked");
    public static readonly Counter TotalSnipersChosen = Metrics.CreateCounter("bbh_total_snipers", "total snipers picked");
    public static readonly Counter TotalScoutsChosen = Metrics.CreateCounter("bbh_total_scouts", "total scouts picked");


    public static readonly Gauge ActiveSoldiers = Metrics.CreateGauge("bbh_active_soldier", "active soldiers");
    public static readonly Gauge ActiveSnipers = Metrics.CreateGauge("bbh_active_snipers", "active snipers");
    public static readonly Gauge ActiveScouts = Metrics.CreateGauge("bbh_active_scouts", "active scouts");

    public void Setup(string monitorEndpoint)
    {
        if (monitorEndpoint == "")
            return;
        var headerValue = Convert.ToBase64String(Encoding.UTF8.GetBytes("user:"+ FlagManager.instance.GetMonitoringPassword()));
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", headerValue);
        this.monitoringEndpoint = monitorEndpoint;
        /*
         * request test
         *
        var res = await httpClient.GetAsync("https://pushgateway.donnerlab.com/metrics");
        var resString = await res.Content.ReadAsStringAsync();
        Debug.Log(resString);
        */
        metricPusher = new MetricPusher(new MetricPusherOptions
        {
            Endpoint = monitoringEndpoint,
            Job ="gameserver",
            Instance = "eu-1",
            HttpClientProvider = () => httpClient,
            IntervalMilliseconds = 5000,
        });
        metricPusher.Start();
        hasStarted = true;
        Debug.Log("Prometheus started");
    }

    public void Shutdown()
    {
        Task t = Task.Run(() => ShutdownTask());
        t.Wait(5000);
    }
    private void ShutdownTask()
    {
        if (!hasStarted)
            return;
        metricPusher = new MetricPusher(new MetricPusherOptions
        {
            Endpoint = monitoringEndpoint,
            Job = "gameserver",
            Instance = "eu-1",
            HttpClientProvider = () => httpClient,
            IntervalMilliseconds = 1,
        });
        metricPusher.Start();
        ActivePlayers.Set(0);
        ActiveBounty.Set(0);
        ActiveCubes.Set(0);
        ActiveSats.Set(0);
        ActiveScouts.Set(0);
        ActiveSnipers.Set(0);
        ActiveSoldiers.Set(0);
        metricPusher.Stop();
    }
}
