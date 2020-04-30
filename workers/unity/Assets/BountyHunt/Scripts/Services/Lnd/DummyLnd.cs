using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Lnrpc;
using System.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class DummyLnd : MonoBehaviour, IClientLnd
{
    public bool ThrowPaymentError;
    string pubkey;

    public event InvoiceSettledEventHandler OnInvoiceSettled;
    public Dictionary<string, Invoice> invoices;

    public DummyLnd()
    {
        invoices = new Dictionary<string, Invoice>();
    }

    public Task Setup(string config, bool listen, bool apdata, string tlsString, string lndConnect)
    {
        pubkey = "pubkey" + UnityEngine.Random.Range(0, int.MaxValue);
        return Task.CompletedTask;
    }

    public void ShutDown()
    {
        return;
    }

    public Task<GetInfoResponse> GetInfo()
    {
        return Task.FromResult(new GetInfoResponse() { IdentityPubkey = pubkey, SyncedToChain = false });
    }

    public Task<ConnectPeerResponse> ConnectPeer(string pubkey, string ip, string port)
    {
        return Task.FromResult(new ConnectPeerResponse());
    }

    public Task<ListChannelsResponse> ListChannels()
    {
        return Task.FromResult(new ListChannelsResponse { });
    }

    public Task<string> GetInvoice(long amount, string description, long expiry)
    {
        var payreq = "invoice" + UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        invoices.Add(payreq, new Invoice { Memo = description, Value = amount, PaymentRequest = payreq });
        return Task.FromResult(payreq);
    }

    public async Task PayInvoice(string paymentRequest)
    {
        if (ThrowPaymentError)
        {
            throw new PaymentException("Dummy payment failed");
        }
    }

    public Task<PendingChannelsResponse> PendingChannels()
    {
        return Task.FromResult(new PendingChannelsResponse { });
    }

    public SignMessageResponse SignMessage(string message)
    {
        return new SignMessageResponse { Signature = "signature" };
    }

    public void AddCallback(InvoiceSettledEventHandler e)
    {
        OnInvoiceSettled += e;
    }

    public void RemoveCallback(InvoiceSettledEventHandler e)
    {
        OnInvoiceSettled -= e;
    }

    public Task<ChannelPoint> OpenChannel(string pubkey, long satAmount)
    {
        return Task.FromResult(new ChannelPoint { });
    }

    public string GetPubkey()
    {
        return this.pubkey;
    }

    public LnConf GetConfig()
    {
        return new LnConf();
    }

    public void SetPaid(string payreq)
    {
        if (invoices.ContainsKey(payreq))
        {
            OnInvoiceSettled.Invoke(this, new InvoiceSettledEventArgs() { Invoice = invoices[payreq] });
        }
    }

    public async Task<PayReq> DecodePayreq(string payreq)
    {
        if (invoices.ContainsKey(payreq))
        {
            var invoice = invoices[payreq];
            return await Task.FromResult(new PayReq { NumSatoshis = invoice.Value, Description = invoice.Memo });
        }

        return await Task.FromResult(new PayReq { NumSatoshis = 0 });
    }

    public void Dispose()
    {
    }

    public Task<long> GetWalletBalace()
    {
        throw new NotImplementedException();
    }

    public Task CloseChannel(string fundingTx, uint index)
    {
        throw new NotImplementedException();
    }

    public Task<string> SendAllCoins(string address)
    {
        throw new NotImplementedException();
    }

    public IEnumerator HandleInvoices(CancellationTokenSource ct)
    {
        //throw new NotImplementedException();
        yield return null;
    }

    public Task KeysendPayment(string targetPubkey, long amount)
    {
        //throw new NotImplementedException();
        return null;
    }

    public Task<SendResponse> KeysendBountyIncrease(string targetPubkey, long amount, string message = "")
    {
        throw new NotImplementedException();
    }

    public Task<SendResponse> KeysendBufferDeposit(string platformPubkey, string targetPubkey, long amount)
    {
        throw new NotImplementedException();
    }

    public Task<SendResponse> KeysendBountyIncrease(string platformPubkey, string targetPubkey, long amount, string message = "")
    {
        throw new NotImplementedException();
    }
}

