using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Lnrpc;
using System.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public interface IClientLnd :IDisposable
{

    Task Setup(string config, bool listen, bool useApdata, string tlsString, string lndConnect = "");
    void ShutDown();
    Task<GetInfoResponse> GetInfo();

    Task<ConnectPeerResponse> ConnectPeer(string pubkey, string ip, string port);
    Task<ListChannelsResponse> ListChannels();
    Task<string> GetInvoice(long amount, string description, long expiry);
    Task<SendResponse> PayInvoice(string paymentRequest);

    Task<PendingChannelsResponse> PendingChannels();

    SignMessageResponse SignMessage(string message);

    void AddCallback(InvoiceSettledEventHandler e);

    void RemoveCallback(InvoiceSettledEventHandler e);

    IEnumerator HandleInvoices(CancellationTokenSource ct);
    Task<ChannelPoint> OpenChannel(string pubkey, long satAmount);
    
    string GetPubkey();

    LnConf GetConfig();

    void SetPaid(string payreq);

    Task<PayReq> DecodePayreq(string payreq);

    Task<long> GetWalletBalace();
    Task<string> SendAllCoins(string address);
    Task CloseChannel(string channelPoint, uint index);
    Task<SendResponse> KeysendBufferDeposit(string platformPubkey, string targetPubkey, long amount);
    Task<SendResponse> KeysendBountyIncrease(string platformPubkey, string targetPubkey, long amount, string message = "");

}


public delegate void InvoiceSettledEventHandler(object obj, InvoiceSettledEventArgs e);

public struct InvoiceSettledEventArgs
{
    public Invoice Invoice;
}
