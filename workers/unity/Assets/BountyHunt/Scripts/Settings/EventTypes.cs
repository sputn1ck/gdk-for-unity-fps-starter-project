using Bountyhunt;
using Improbable.Gdk.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoolEvent : UnityEvent<bool> { }
public class FloatEvent : UnityEvent<float> { }
public class IntEvent : UnityEvent<int> { }
public class LongEvent : UnityEvent<long> { }
public class StringEvent : UnityEvent<string> { }
public class Vector2Event : UnityEvent<Vector2> { }
public class GameObjectEvent : UnityEvent<GameObject> { }
public class ScoreboardUIItemListEvent : UnityEvent<List<ScoreboardUIItem>,EntityId> { }
[Serializable] public class BountyUpdateEvent : UnityEvent<BountyUpdateEventArgs> { }
[Serializable] public class SessionEarningsUpdateEvent : UnityEvent<SessionEarningsEventArgs> { }

[Serializable] public class BalanceUpdateEvent : UnityEvent<BalanceUpdateEventArgs> { }
public class StringColorEvent : UnityEvent<string, Color> { }
[Serializable] public class ChatMessageEvent : UnityEvent<Chat.ChatMessage> { }
public class StringLongEvent : UnityEvent<string, long> { }

public class PaymentSuccessEvent : UnityEvent<PaymentSuccesArgs> { }
public class PaymentFailureEvent : UnityEvent<PaymentFailureArgs> { }

public class KillEvent : UnityEvent<KillEventArgs> { }

public class RoundUpdateEvent : UnityEvent<RoundUpdateEventArgs> { }

public class KillsAndDeathsUpdateEvent : UnityEvent<KillsAndDeathsUpdateEventArgs> { }

public class AllTimeScoreUpdateEvent : UnityEvent<AllTimeScoreUpdateArgs> { }

public class LeaderboardUpdateEvent : UnityEvent<LeaderboardUpdateArgs> { }

public class UpdateAdvertisersEvent : UnityEvent<List<Advertiser>> { }

public class PopUpEvent : UnityEvent<PopUpEventArgs> { }
public class YesNoPopUpEvent : UnityEvent<YesNoPopUpEventArgs> { }
public class ImagePopUpEvent : UnityEvent<ImagePopUpEventArgs> { }

[Serializable]
public struct BountyUpdateEventArgs
{
    public long NewAmount;
    public long OldAmount;
    public Bountyhunt.BountyReason Reason;
}

[Serializable]
public struct SessionEarningsEventArgs
{
    public long NewAmount;
    public long OldAmount;
}

[Serializable]
public struct PaymentSuccesArgs
{
    public string invoice;
    public long amount;
    public string descripion;
}

[Serializable]
public struct PaymentFailureArgs
{
    public string invoice;
    public string message;
}

public struct BalanceUpdateEventArgs
{
    public long GameServerBalance;
    public long BufferBalance;
    public long DaemonBalance;
    public long ChannelCost;
}

public struct KillEventArgs
{
    public string killer;
    public string victim;
}

public struct RoundUpdateEventArgs
{
    /// <summary>
    /// rounds remainig time in seconds
    /// </summary>
    public float remainingTime;
    public GameMode gameMode;
}

public struct KillsAndDeathsUpdateEventArgs
{
    public int kills;
    public int deaths;
}

public struct AllTimeScoreUpdateArgs
{
    public string name;
    public long score;
}
public struct LeaderboardUpdateArgs
{
    public Bbh.Highscore[] highscores;
    public string PlayerPubKey;
}

public struct PopUpEventArgs
{
    public string headline;
    public string text;
    public bool verticalButtonLayout;
    public bool showX;
    public List<LabelAndAction> actions;
    public string popUpID;

    /// <param name="actions">the button labels and their corresponding actions for OnClick()</param>
    /// <param name="verticalButtonlayout">should the buttons be layouted vertical instead of horizontal?</param>
    /// <param name="popUpID">this is used, if you want to need acces to the created popup (such as closing it from another script) </param>
    public PopUpEventArgs(string headline, string text, List<LabelAndAction> actions, bool verticalButtonlayout, bool showX = true, string popUpID = "")
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.actions = actions;
        this.verticalButtonLayout = verticalButtonlayout;
        this.popUpID = popUpID;
    }

    /// <param name="popUpID">this is used, if you want to need acces to the created popup (such as closing it from another script) </param>
    public PopUpEventArgs(string headline, string text, string popUpID = "")
    {
        this.headline = headline;
        this.text = text;
        this.showX = true;
        this.actions = new List<LabelAndAction>();
        this.verticalButtonLayout = false;
        this.popUpID = popUpID;
    }
}

public struct YesNoPopUpEventArgs
{
    public string headline;
    public string text;
    public bool showX;
    public UnityAction yesAction;
    public UnityAction noAction;
    public string popUpID;

    /// <param name="action">the action for both buttons, "YES" sends true, "NO" sends false</param>
    /// <param name="popUpID">this is used, if you want to need acces to the created popup (such as closing it from another script) </param>
    public YesNoPopUpEventArgs(string headline, string text, UnityAction<bool> action, bool showX = true, string popUpID = "")
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.yesAction = delegate { action.Invoke(true); };
        this.noAction = delegate { action.Invoke(false); };
        this.popUpID = popUpID;
    }

    /// <param name="yesAction">the action, for the "YES" Button</param>
    /// <param name="noAction">the action, for the "NO" Button</param>
    /// <param name="popUpID">this is used, if you want to need acces to the created popup (such as closing it from another script) </param>
    public YesNoPopUpEventArgs(string headline, string text, UnityAction yesAction, UnityAction noAction, bool showX = true, string popUpID = "")
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.yesAction = yesAction;
        this.noAction = noAction;
        this.popUpID = popUpID;
    }
}

public struct ImagePopUpEventArgs
{
    public string headline;
    public string text1;
    public string text2;
    public bool verticalButtonLayout;
    public bool showX;
    public List<LabelAndAction> actions;
    public Sprite sprite;
    public bool tintImage;
    public float imageSizeMultiplier;
    public string popUpID;

    /// <param name="text1">text above the image</param>
    /// <param name="text2">text below the image</param>
    /// <param name="actions">the button labels and their corresponding actions for OnClick()</param>
    /// <param name="verticalButtonlayout">should the buttons be layouted vertical instead of horizontal?</param>
    /// <param name="tintImage">should the image be tinted with the primary color? if not, the image color is white</param>
    /// <param name="imageSizeMultiplier">relative size of the image. Size 1 is a square fitted to the panel. Sprite aspect is always preserved.</param>
    /// <param name="popUpID">this is used, if you want to need acces to the created popup (such as closing it from another script) </param>
    public ImagePopUpEventArgs(string headline, string text1, Sprite sprite, string text2, List<LabelAndAction> actions, bool verticalButtonlayout, bool tintImage, float imageSizeMultiplier = 1, bool showX = true, string popUpID = "")
    {
        this.headline = headline;
        this.text1 = text1;
        this.text2 = text2;
        this.showX = showX;
        this.actions = actions;
        this.verticalButtonLayout = verticalButtonlayout;
        this.sprite = sprite;
        this.tintImage = tintImage;
        this.imageSizeMultiplier = imageSizeMultiplier;
        this.popUpID = popUpID;
    }
}

