using Lnrpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public static class PaymentUIHelper
{
    public static async void ExternalPayment(string invoice,PayReq payreq,UnityAction onSuccess = null,UnityAction onFailure = null)
    {
        BBHUIManager.instance.mainMenu.BlendImage(true);
        string text =  String.Format(GameText.ExternalPaymentPopup,payreq.NumSatoshis + Utility.tintedSatsSymbol);
        Sprite qrCode = Utility.GetInvertedQRCode(invoice);


        PopUpButtonArgs copyAction = new PopUpButtonArgs("copy invoice", () => Utility.CopyToClipboard(invoice), false);
        var closeToken = new CancellationTokenSource();
        ImagePopUpArgs args = new ImagePopUpArgs("Lightning Payment", text, qrCode, "", new List<PopUpButtonArgs> { copyAction }, true, true, closeAction: () => {
            BBHUIManager.instance.mainMenu.BlendImage(false);
            closeToken.Cancel();
        });
        PopUpUI popup = PopUpManagerUI.instance.OpenImagePopUp(args);

        
        try
        {
            await PlayerServiceConnections.instance.BackendPlayerClient.WaitForPayment(invoice,payreq.Timestamp +payreq.Expiry, closeToken.Token);
        }
        catch (ExpiredException e)
        {
            if (onFailure != null) onFailure.Invoke();
            Debug.Log(e.Message);
            if (popup == null)return;
            else popup.Close();

            PopUpArgs errArgs = new PopUpArgs("error", GameText.PaymentExpiredPopup);
            PopUpManagerUI.instance.OpenPopUp(errArgs);

            return;
        }
        catch (Exception e)
        {
            if(onFailure != null) onFailure.Invoke();

            if (popup != null) popup.Close();
            PopUpArgs errArgs = new PopUpArgs("error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(errArgs);
            return;
        } finally
        {
            BBHUIManager.instance.mainMenu.BlendImage(false);
        }

        if (popup != null) popup.Close();

        PopUpArgs args1 = new PopUpArgs("info", GameText.PaymentSuccessPopup);
        PopUpManagerUI.instance.OpenPopUp(args1);
        if(onSuccess != null) onSuccess.Invoke();

        BBHUIManager.instance.mainMenu.BlendImage(false);
    }
    
    public static async void IngamePayment(string invoice, PayReq payreq, UnityAction onSuccess = null, UnityAction onFailure = null)
    {
        PopUpArgs args = new PopUpArgs("Info", GameText.WaitingForPaymentPopup);
        PopUpUI popup = PopUpManagerUI.instance.OpenPopUp(args);
        try
        {
            await PlayerServiceConnections.instance.lnd.PayInvoice(invoice);
        }
        catch (Exception e)
        {
            if (onFailure != null)onFailure.Invoke();
            Debug.Log(e.Message);
            if (popup != null) popup.Close();
            PopUpArgs args1 = new PopUpArgs("error", e.Message);
            PopUpManagerUI.instance.OpenPopUp(args1);
            return;
        }
        if(onSuccess != null) onSuccess.Invoke();
        if (popup != null) popup.Close();
        PopUpArgs args2 = new PopUpArgs("info", GameText.PaymentSuccessPopup);
        PopUpManagerUI.instance.OpenPopUp(args2);
    }
}
