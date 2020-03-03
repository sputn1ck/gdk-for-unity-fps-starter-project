using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "LobbyGameMode", menuName = "BBH/GameModes/Lobby", order = 1)]
public class LobbyGameMode : GameMode
{
    private ServerGameModeBehaviour _serverGameModeBehaviour;
    public override async void OnGameModeStart(ServerGameModeBehaviour serverGameModeBehaviour)
    {
        Debug.Log("start lobby");
        _serverGameModeBehaviour = serverGameModeBehaviour;
        var res = await ServerServiceConnections.instance.AuctionController.StartAuction((int)GlobalSettings.SecondDuration);
        ServerEvents.instance.OnAuctionInvoicePaid.AddListener(OnAuctionPaid);
        ServerEvents.instance.OnRandomInvoicePaid.AddListener(OnDonationPaid);
    }

    public override async void OnGameModeEnd(ServerGameModeBehaviour serverGameModeBehaviour)
    {
        Debug.Log("end lobby");
        await Task.Delay(2000);
    }

    private void OnAuctionPaid(AuctionInvoice auctionInvoice)
    {
        AddCarryOverSats(auctionInvoice.Amount);
    }

    private void OnDonationPaid(RandomInvoice e)
    {
        AddCarryOverSats(e.amount);

        ServerGameChat.instance.SendGlobalMessage("DONATION", e.message, Chat.MessageType.INFO_LOG, e.amount >= 250);
    }

    private void AddCarryOverSats(long sats)
    {
        _serverGameModeBehaviour.GameStatsWriter.SendUpdate(new Bountyhunt.GameStats.Update
        {
            CarryoverSats = _serverGameModeBehaviour.GameStatsWriter.Data.CarryoverSats + sats
        });
    }

    private void RemoveListeners()
    {
        ServerEvents.instance.OnAuctionInvoicePaid.RemoveListener(OnAuctionPaid);
        ServerEvents.instance.OnRandomInvoicePaid.RemoveListener(OnDonationPaid);
    }
}

