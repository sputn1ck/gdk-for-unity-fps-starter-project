using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading.Tasks;

public class LnSubMenuUI : SubMenuUI
{
    //public long onChainBalance;
    public long LightningBalance;

    public Lnrpc.Channel DonnerChannel;

    public TMP_Text ErrorText;
    public TMP_Text OkText;

    public Button RefreshButton;
    public TMP_Text BalanceText;
    
    public Button PayoutLnButton;
    public TMP_InputField InvoiceInput;
    //public Button CloseAndSendButton;
    //public TMP_InputField BtcAddressInput;
    public Button WikiButton;

    private PlayerServiceConnections lnClient;
    private const string donnerPubkey = "024b0f1e453299eb39fd629ebc0f881e7714a86eb86f173b11f7606fcb4731e246";
    //private const long minBtc = 25000;

    public void Awake()
    {
        RefreshButton.onClick.AddListener(Refresh);
        PayoutLnButton.onClick.AddListener(PayoutLN);
        WikiButton.onClick.AddListener(OpenWiki);
        //CloseAndSendButton.onClick.AddListener(CloseAndSend);
    }
    public async void OnEnable()
    {
        ErrorText.text = "";
        OkText.text = "";
        lnClient = PlayerServiceConnections.instance;
        await UpdateBalance();
    }

    public void OnDisable()
    {
        
    }

    public async void Refresh()
    {
        await UpdateBalance();
    }
    public async Task GetDonnerChannel()
    {
        try
        {

            var channels = await lnClient.lnd.ListChannels();
            DonnerChannel = channels.Channels.FirstOrDefault(c => c.RemotePubkey == donnerPubkey );
        } catch(Exception e)
        {


            StartCoroutine(ShowErrorText(e.Message));
        }
    }
    public async Task UpdateBalance()
    {
        try
        {
            var balance = await PlayerServiceConnections.instance.DonnerDaemonClient.GetWalletBalance();
            BalanceText.text = "Lightning Balance: " + balance.DaemonBalance;
                
        } catch(Exception e)
        {


            StartCoroutine(ShowErrorText(e.Message));
        }
        
    }

    public async void PayoutLN()
    {
        try
        {
            var invoice = await lnClient.lnd.DecodePayreq(InvoiceInput.text);
            if(invoice.NumSatoshis > LightningBalance)
            {
                throw new Exception("Not enough sats to pay invoice");
            }
            if (invoice.NumSatoshis >= LightningBalance)
            {
                throw new Exception("You need to specify less sats for your invoice, as you need to pay for route fees");
            }

            await lnClient.lnd.PayInvoice(InvoiceInput.text);
            await UpdateBalance();
            OkText.text = "Succeeded payout";
            
                

        } catch(Exception e)
        {
            if(e.Message != "No lightning channel found" && e.Message != "Not enough sats to pay invoice")
                StartCoroutine(ShowErrorText("An error occured: maybe try paying a smaller amount. " + e.Message));
            else
                StartCoroutine(ShowErrorText(e.Message));
        }
        
    }

    IEnumerator ShowErrorText(string text)
    {
        ErrorText.text = text;
        yield return new WaitForSeconds(5f);
        ErrorText.text = "";
    }
    public void OpenWiki()
    {
        Application.OpenURL("https://github.com/donnerlab1/bitcoin-bounty-hunt-public/wiki/Payouts");
    }
    /*
    public async void CloseAndSend()
    {
        try
        {
            await UpdateBalance();
            if(DonnerChannel != null)
            {
                if(DonnerChannel.LocalBalance < minBtc)
                {
                    throw new Exception("Not enough balance on Channel to close: " + minBtc + " sats required");
                }
                await CloseChannel();
            }
            await UpdateBalance();
            await SendBTC();
        }
        catch (Exception e)
        {

            StartCoroutine(ShowErrorText(e.Message));
        }
    }

    public async Task CloseChannel()
    {
       var channelpoint = DonnerChannel.ChannelPoint.Substring(0, DonnerChannel.ChannelPoint.Length - 2);
        var index = DonnerChannel.ChannelPoint.Substring(DonnerChannel.ChannelPoint.Length - 1, 1);
       await LnClient.instance.lnd.CloseChannel(channelpoint, uint.Parse(index));
    }

    public async Task SendBTC()
    {
        
        var res = await LnClient.instance.lnd.SendAllCoins(BtcAddressInput.text);
        OkText.text = "btc payout suceeded txId: " + res;
    }*/
    public override void OnDeselect()
    {
        base.OnDeselect();
    }

    public override void OnSelect()
    {
        base.OnSelect();
    }
}
