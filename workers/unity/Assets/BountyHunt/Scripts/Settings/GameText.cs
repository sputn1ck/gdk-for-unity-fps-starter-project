
public static class GameText
{
    /// Main Menu > Wallet > Pay Invoice > (i)
    public const string PayInvoiceInfo = "Here some info about invoices! Maybe Kon Has some suggestions for this text.";

    /// Main Menu > Wallet > Pay Invoice > [PAY] > Popup
    /// {0} = invoice description
    /// {1} = price with sats symbol
    public const string PayInvoicePopup = "paying invoice: \n {0} \n for {1}\n are you sure?";

    /// Main Menu > Wallet > Donation > (i)
    public const string DonationInfo = "with the slider, you can adjust, how much of the Donation goes into the game pot and how much the game developers will recieve";

    /// text above the Image in the Buy Skin Popup
    /// {0} = skin name
    /// {1} = price with sats symbol
    public const string BuySkinPopup = "You are going to buy: \n {0} \n for {1}";

    /// text below the Image in the Buy Skin Popup if ingame balance is to low
    public const string BuySkinPopupWarning = "Your Ingame Wallet doesent cover the required amount!";

    /// text above the QR Code in the external wallet payment Popup
    /// {0} = price with sats symbol
    public const string ExternalPaymentPopup = "payment for {0}";

    /// text in the payment waiting for Payment Popup
    public const string WaitingForPaymentPopup = "waiting for payment...";

    /// text in the payment Success Popup
    public const string PaymentSuccessPopup = "payment successfull";

    /// text in the payment Expired Popup
    public const string PaymentExpiredPopup = "payment has expired!";

}
