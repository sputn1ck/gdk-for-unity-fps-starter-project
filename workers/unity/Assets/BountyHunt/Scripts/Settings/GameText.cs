
using System.Collections.Generic;
using UnityEngine;

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
    public const string linkBookmarkedInfo = "Link Bookmarked";

    /// text in the Quit Game Popup
    public const string QuitGamePopup = "Do you really want to quit the game?";

    /// assign Button Header
    /// {0} = key label
    public const string AssignButtonPopupHeader = "assign {0} key";

    /// assign Button Text
    public const string AssignButtonPopup = "press any key!\n(or \"escape\" to abort)";

    /// headline and description of the popup, that appears, when you click on the pay button in a sponsors Tile
    public const string IncreaseSponsorPlayerSatsPopupHeader = "Buy Player Sats";
    public const string IncreaseSponsorPlayerSatsPopupText = "How much Player Satoshi do you want to buy?";

    /// label of the bookmark link action in the billboard contextmenu
    public const string AdContextMenuBookmarkActionLabel = "bookmark link <sprite name=\"bookmark\" tint=1>";

    /// content of the players Context menu discription
    /// {0} player bounty
    public const string PlayerContextMenuText = "Bounty - {0}";

    /// label of the Bounty Increase action in the player contextmenu
    public const string IncreasePlayerBountyContextMenuActionLabel = "Increase Bounty";

    /// description of the Bounty Increase submenu in the player contextmenu
    public const string IncreasePlayerBountyContextMenuText = "Increase Bounty by:";

    /// announcement text, if balance doesnt cover a payment
    public const string BalanceToLowAnnouncement = "Balance to low";

    /// announcement text, if a payment was successfull
    public const string PaymentSuccesfullAnnouncement = "Payment Sucessfull";


    //-----TOOLTIPS----- Don't rename these Constants !!!
    public const string BuyPlayersatsTooltip = "buy player sats";
    public const string OpenLinkTooltip = "Open URL";
    public const string BookmarkLinkTooltip = "Bookmark URL";
    public const string IngameWalletTooltip = "pay with ingame wallet";
    public const string ExternalWalletTooltip = "pay with external wallet";

    public static string GetKeyName(KeyCode keyCode)
    {
        switch (keyCode)
        {
            default: return keyCode.ToString();
            case KeyCode.Alpha0: return "0";
            case KeyCode.Alpha1: return "1";
            case KeyCode.Alpha2: return "2";
            case KeyCode.Alpha3: return "3";
            case KeyCode.Alpha4: return "4";
            case KeyCode.Alpha5: return "5";
            case KeyCode.Alpha6: return "6";
            case KeyCode.Alpha7: return "7";
            case KeyCode.Alpha8: return "8";
            case KeyCode.Alpha9: return "9";
            case KeyCode.Keypad0: return "K0";
            case KeyCode.Keypad1: return "K1";
            case KeyCode.Keypad2: return "K2";
            case KeyCode.Keypad3: return "K3";
            case KeyCode.Keypad4: return "K4";
            case KeyCode.Keypad5: return "K5";
            case KeyCode.Keypad6: return "K6";
            case KeyCode.Keypad7: return "K7";
            case KeyCode.Keypad8: return "K8";
            case KeyCode.Keypad9: return "K9";
            case KeyCode.Mouse0: return "LMB";
            case KeyCode.Mouse1: return "RMB";
            case KeyCode.Mouse2: return "MB2";
            case KeyCode.Mouse3: return "MB3";
            case KeyCode.Mouse4: return "MB4";
            case KeyCode.Mouse5: return "MB5";
            case KeyCode.Mouse6: return "MB6";
            case KeyCode.UpArrow: return "Up";
            case KeyCode.DownArrow: return "Down";
            case KeyCode.LeftArrow: return "Left";
            case KeyCode.RightArrow: return "Right";
            case KeyCode.Escape: return "esc";
            case KeyCode.Caret: return "^";
            case KeyCode.LeftShift: return "L-shift";
            case KeyCode.RightShift: return "R-shift";
        }
    }
}
