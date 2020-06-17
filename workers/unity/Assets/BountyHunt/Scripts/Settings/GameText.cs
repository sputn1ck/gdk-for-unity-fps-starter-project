
public static class GameText
{
    /// Main Menu > Wallet > Pay Invoice > (i)
    public const string LicenceMissingInfo = "To fully enjoy the game, you have to gather enough sats to open a lightning channel to the game";

    /// Main Menu > Wallet > Pay Invoice > (i)
    public const string PayInvoiceInfo = "Create an Invoice and add it here to pay. NOTE: the payment can fail for a lot of reasons. Try creating an Invoice with small amounts first";

    /// Main Menu > Wallet > Pay Invoice > [PAY] > Popup
    /// {0} = invoice description
    /// {1} = price with sats symbol
    public const string PayInvoicePopup = "paying invoice: \n {0} \n for {1}\n are you sure?";

    /// Main Menu > Wallet > Donation > (i)
    public const string DonationInfo = "with the slider, you can adjust, how much of the donation goes into the game pot and how much the game developers will recieve";

    public const string WithdrawInfo = "You can easily withdraw money using mobile Wallets such as Phoenix Wallet, Breez Wallet, Wallet of Satoshi, Blue Wallet. Simply scan the qr code and follow the instructions";
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

    /// text to see, when clicked on a link (to open it later)
    public const string AdOpenInfo = "Link Remembered";

    /// text in the Quit Game Popup
    public const string QuitGamePopup = "Do you really want to quit the game?";

    /// assign Button Header
    /// {0} = key label
    public const string AssignButtonPopupHeader = "assign {0} key";

    /// assign Button Text
    public const string AssignButtonPopup = "press any key!\n(or \"escape\" to abort)";

    /// Button descriptions of Tiles in MainMenu > Sponsors
    public const string BuyPlayerSatsButtonDescription = "buy playerSats";
    public const string OpenLinkButtonDescription = "open link";
    public const string BookmarkLinkButtonDescription = "bookmark link";
}
