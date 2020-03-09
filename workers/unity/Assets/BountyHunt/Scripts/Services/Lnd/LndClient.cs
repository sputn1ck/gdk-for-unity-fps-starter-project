using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Fps;
using Grpc.Core;
using Lnrpc;
using UnityEngine;

[Serializable]
public struct LnConf
{
    public string host;
    public int rpcport;
    public int listenport;
    public string tlsfile;
    public string macaroonfile;
}

public class LndClient : IClientLnd
{
    public string confName;
    public PlayerServiceConnections instance;

    private Grpc.Core.Channel rpcChannel;
    private Lightning.LightningClient lightningClient;

    public event InvoiceSettledEventHandler OnInvoiceSettled;

    private LnConf lnconf;
    private string tlsCert;
    private string macaroon;
    public string pubkey;
    public bool useAppdata;


    public CancellationTokenSource ct;
    private Thread listenThread;
    private AsyncServerStreamingCall<Invoice> _invoiceStream;
    private ConcurrentQueue<InvoiceSettledEventArgs> invoiceSettledQueue;
    private RNGCryptoServiceProvider provider;

    public LndClient()
    {
    }


    public async Task Setup(string config, bool listen, bool useApdata, string tlsString)
    {
        this.confName = config;
        this.useAppdata = useApdata;
        LoadConfig(tlsString);
        var macaroonCallCredentials = new MacaroonCallCredentials(macaroon);
        var sslCreds = new SslCredentials(tlsCert);
        var channelCreds = ChannelCredentials.Create(sslCreds, macaroonCallCredentials.credentials);
        rpcChannel = new Grpc.Core.Channel(lnconf.host, lnconf.rpcport, channelCreds);
        lightningClient = new Lightning.LightningClient(rpcChannel);
        pubkey = (await GetInfo()).IdentityPubkey;
        invoiceSettledQueue = new ConcurrentQueue<InvoiceSettledEventArgs>();
        provider = new RNGCryptoServiceProvider();
        Debug.Log("my pubkey: " + pubkey);

        if (listen)
        {
            StartListening();
        }

        Debug.Log("finished setup");
    }

    public IEnumerator HandleInvoices(CancellationTokenSource ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if(invoiceSettledQueue.TryDequeue(out InvoiceSettledEventArgs e))
            {
                ServerEvents.instance.OnInvoicePaid.Invoke(e);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void StartListening()
    {
        //await ListenInvoices();
        listenThread = new Thread(async () =>
        {
            while (!rpcChannel.ShutdownToken.IsCancellationRequested)
            {
                await ListenInvoicesTask();
                Thread.Sleep(1000);
            }
        });
        listenThread.Start();
    }

    private async Task ListenInvoicesTask()
    {
        var request = new InvoiceSubscription();

        try
        {
            using (_invoiceStream =
                lightningClient.SubscribeInvoices(request, cancellationToken: rpcChannel.ShutdownToken))
            {
                Debug.Log("listening successfully started");
                while (!rpcChannel.ShutdownToken.IsCancellationRequested &&
                    await _invoiceStream.ResponseStream.MoveNext(rpcChannel.ShutdownToken))
                {
                    var invoice = _invoiceStream.ResponseStream.Current;

                    Debug.Log("new invoice " + invoice);
                    if (invoice.State == Invoice.Types.InvoiceState.Settled)
                    {
                        var e = new InvoiceSettledEventArgs();
                        e.Invoice = invoice;

                        //OnInvoiceSettled?.Invoke(this, e);
                        //ServerEvents.instance.OnInvoicePaid.Invoke(e);
                        invoiceSettledQueue.Enqueue(e);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void LoadConfig(string tlsString = "")
    {
        string path = Application.dataPath + "/StreamingAssets";
#if UNITY_EDITOR
        if (useAppdata)
        {
            path = Application.dataPath + "/StreamingAssets";
            confName = "/dd-win.conf";

            var json = File.ReadAllText(path + "/" + confName);
            lnconf = JsonUtility.FromJson<LnConf>(json);
            string home = "";
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                home = Environment.GetEnvironmentVariable("HOME") + "/Library/Application Support";
            }
            else
            {
                home = Environment.GetEnvironmentVariable("Appdata");
            }

            tlsCert = File.ReadAllText(home + "/Donner/Daemon/data/tls.cert");
            macaroon = MacaroonCallCredentials.ToHex(
                File.ReadAllBytes(home + "/Donner/Daemon/data/lnd/chain/bitcoin/mainnet/admin.macaroon"));
        }
        else
        {
            path = Application.dataPath + "/StreamingAssets";
            var json = File.ReadAllText(path + "/" + confName);
            lnconf = JsonUtility.FromJson<LnConf>(json);

            if(tlsString == "")
            {

                tlsCert = File.ReadAllText(path + "/" + lnconf.tlsfile);
            } else
            {
                tlsCert = tlsString;
            }
            macaroon = MacaroonCallCredentials.ToHex(File.ReadAllBytes(path + "/" + lnconf.macaroonfile));
        }
#elif UNITY_STANDALONE_OSX
        path = Application.dataPath + "/Resources/Data/StreamingAssets";
        confName = "/dd-mac.conf";

        var json = File.ReadAllText(path + "/" + confName);
        lnconf = JsonUtility.FromJson<LnConf>(json);

        var home = Environment.GetEnvironmentVariable("HOME");
        tlsCert = File.ReadAllText(home + "/Library/Application Support/Donner/Daemon/data/tls.cert");
        macaroon =
 MacaroonCallCredentials.ToHex(File.ReadAllBytes(home + "/Library/Application Support/Donner/Daemon/data/lnd/chain/bitcoin/mainnet/admin.macaroon"));
#elif UNITY_STANDALONE_WIN
        path = Application.dataPath + "/StreamingAssets";
        confName = "/dd-win.conf";

        var json = File.ReadAllText(path + "/" + confName);
        lnconf = JsonUtility.FromJson<LnConf>(json);

        var home = Environment.GetEnvironmentVariable("Appdata");
        tlsCert = File.ReadAllText(home + "/Donner/Daemon/data/tls.cert");
        macaroon =
 MacaroonCallCredentials.ToHex(File.ReadAllBytes(home + "/Donner/Daemon/data/lnd/chain/bitcoin/mainnet/admin.macaroon"));

#else
        path = Application.dataPath + "/StreamingAssets";
        var json = File.ReadAllText(path + "/" + confName);
        lnconf = JsonUtility.FromJson<LnConf>(json);

        tlsCert = File.ReadAllText(path + "/" + lnconf.tlsfile);
        macaroon = MacaroonCallCredentials.ToHex(File.ReadAllBytes(path + "/" + lnconf.macaroonfile));

#endif
    }

    public async Task<GetInfoResponse> GetInfo()
    {
        return await lightningClient.GetInfoAsync(new GetInfoRequest { });
    }

    public async Task<ConnectPeerResponse> ConnectPeer(string pubkey, string ip, string port)
    {
        Debug.Log(pubkey + "@" + ip + ":" + port);
        var res = await lightningClient.ConnectPeerAsync(new ConnectPeerRequest
        {
            Addr = new LightningAddress
            {
                Host = ip + ":" + port,
                Pubkey = pubkey
            }
        });
        return res;
    }

    public async Task<ListChannelsResponse> ListChannels()
    {
        var res = await lightningClient.ListChannelsAsync(new ListChannelsRequest());
        return res;
    }


    public async Task<string> GetInvoice(long amount, string description, long expiry)
    {
        if (amount < 1)
            amount = 1;
        if (lightningClient == null)
        {
            Debug.LogError("for some reason lightning client is null?");
        }

        var invoice = await lightningClient.AddInvoiceAsync(new Invoice
        {
            Value = amount,
            Memo = description,
            Expiry = expiry,
        });
        return invoice.PaymentRequest;
    }

    public async Task<SendResponse> PayInvoice(string paymentRequest)
    {
        var res = await lightningClient.SendPaymentSyncAsync(new SendRequest { PaymentRequest = paymentRequest });
        return res;
    }

    public InvoiceSettledEventHandler GetInvoiceSettledEventHandler()
    {
        return OnInvoiceSettled;
    }

    public void AddCallback(InvoiceSettledEventHandler e)
    {
        OnInvoiceSettled += e;
    }

    public void RemoveCallback(InvoiceSettledEventHandler e)
    {
        OnInvoiceSettled -= e;
    }

    public async Task ListenInvoices()
    {
        var request = new InvoiceSubscription();

        try
        {
            using (_invoiceStream =
                lightningClient.SubscribeInvoices(request, cancellationToken: rpcChannel.ShutdownToken))
            {
                Debug.Log("listening successfully started");
                while (!rpcChannel.ShutdownToken.IsCancellationRequested &&
                    await _invoiceStream.ResponseStream.MoveNext(rpcChannel.ShutdownToken))
                {
                    var invoice = _invoiceStream.ResponseStream.Current;

                    Debug.Log("new invoice " + invoice);
                    if (invoice.State == Invoice.Types.InvoiceState.Settled)
                    {
                        var e = new InvoiceSettledEventArgs();
                        e.Invoice = invoice;
                        OnInvoiceSettled(this, e);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        Debug.Log("listen invoices over");
        if (!rpcChannel.ShutdownToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
            StartListening();
        }
    }

    public async Task<PendingChannelsResponse> PendingChannels()
    {
        var res = await lightningClient.PendingChannelsAsync(new PendingChannelsRequest());

        return res;
    }

    public SignMessageResponse SignMessage(string message)
    {
        var res = lightningClient.SignMessage(new SignMessageRequest
            { Msg = Google.Protobuf.ByteString.CopyFromUtf8(message) });
        return res;
    }

    private void ShutDownRpc()
    {
        Task task = Task.Run(async () => await rpcChannel.ShutdownAsync());
        task.Wait(5000);
    }

    public void ShutDown()
    {
        Debug.Log("shutdown called");
        if (rpcChannel != null)
        {
            ShutDownRpc();
            Debug.Log("shutting down rpc");
        }


        if (_invoiceStream != null)
        {
            Debug.Log("disposing invoiceStream");
            _invoiceStream.Dispose();
        }
    }

    public async Task<ChannelPoint> OpenChannel(string pubkey, long satAmount)
    {
        var res = await lightningClient.OpenChannelSyncAsync(new OpenChannelRequest
        {
            NodePubkeyString = pubkey, LocalFundingAmount = satAmount, SpendUnconfirmed = true,
            TargetConf = FlagManager.instance.GetTargetConf(), Private = true
        });
        return res;
    }

    public string GetPubkey()
    {
        return pubkey;
    }

    public LnConf GetConfig()
    {
        return lnconf;
    }

    public async void SetPaid(string payreq)
    {
        if (OnInvoiceSettled != null)
        {
            var invoice = await lightningClient.DecodePayReqAsync(new PayReqString { PayReq = payreq });
            OnInvoiceSettled.Invoke(this,
                new InvoiceSettledEventArgs()
                {
                    Invoice = new Invoice
                        { PaymentRequest = payreq, Value = invoice.NumSatoshis, Memo = invoice.Description }
                });
        }
    }


    public async Task<PayReq> DecodePayreq(string payreq)
    {
        return await lightningClient.DecodePayReqAsync(new PayReqString { PayReq = payreq });
    }

    public async Task CloseChannel(string channelPoint, uint index)
    {
        var req = new CloseChannelRequest
        {
            ChannelPoint = new ChannelPoint
            {
                FundingTxidStr = channelPoint,
                OutputIndex = index
            }
        };
        using (var closeChannelStream = lightningClient.CloseChannel(req))
        {
            while (!rpcChannel.ShutdownToken.IsCancellationRequested &&
                await closeChannelStream.ResponseStream.MoveNext(rpcChannel.ShutdownToken))
            {
                if (closeChannelStream.ResponseStream.Current.UpdateCase ==
                    CloseStatusUpdate.UpdateOneofCase.ClosePending)
                {
                    return;
                }
            }
        }

        /* var res = await lightningClient.CloseChannel(new CloseChannelRequest()
         {
             ChannelPoint = new ChannelPoint() { FundingTxidStr = fundingTx }
         });*/
    }

    public async Task<string> SendAllCoins(string address)
    {
        var res = await lightningClient.SendCoinsAsync(new SendCoinsRequest()
        {
            Addr = address,
            SendAll = true,
            TargetConf = 6
        });
        return res.Txid;
    }

    public void Dispose()
    {
        ShutDown();
    }

    public async Task<long> GetWalletBalace()
    {
        var res = await lightningClient.WalletBalanceAsync(new WalletBalanceRequest());
        return res.TotalBalance;
    }

    public async Task<SendResponse> KeysendPayment(string targetPubkey, long amount)
    {
        var preImage = GetRandomBytes();
        var rHash = GetRHash(preImage);

        // TODO add platform pubkey
        var req = new SendRequest
        {
            Dest = Google.Protobuf.ByteString.CopyFrom(DonnerUtils.StringToByteArrayFastest(targetPubkey)),
            Amt = amount,
            PaymentHash = Google.Protobuf.ByteString.CopyFrom(rHash),
        };
        
        req.DestCustomRecords.Add(5482373484, Google.Protobuf.ByteString.CopyFrom(preImage));
        //req.DestCustomRecords.Add(684568698384, Google.Protobuf.ByteString.CopyFrom(DonnerUtils.StringToByteArrayFastest(targetPubkey)));
        //req.DestCustomRecords.Add(684577697779, Google.Protobuf.ByteString.CopyFrom(DonnerUtils.StringToByteArrayFastest("BBH payout")));

        return await lightningClient.SendPaymentSyncAsync(req);
    }
    private byte[] GetRandomBytes()
    {
        var byteArray = new byte[32];

        provider.GetBytes(byteArray);
        return byteArray;
    }

    private byte[] GetRHash(byte[] rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(rawData);

            return bytes;
        }
    }


}

/* concurrent channel test
public class Channel<T>
{

    private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
    public void Write(T value)
    {
        _queue.Enqueue(value); 
        _semaphore.Release(); 
    }

    public async Task<T> ReadAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false); 
        bool gotOne = _queue.TryDequeue(out T item); 
        return item;
    }
}*/
