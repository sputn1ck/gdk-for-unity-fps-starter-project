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

    public PopUpEventArgs(string headline, string text, List<LabelAndAction> actions, bool verticalButtonlayout, bool showX = true)
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.actions = actions;
        this.verticalButtonLayout = verticalButtonlayout;
    }
    public PopUpEventArgs(string headline, string text)
    {
        this.headline = headline;
        this.text = text;
        this.showX = true;
        this.actions = new List<LabelAndAction>();
        this.verticalButtonLayout = false;

    }
}

public struct YesNoPopUpEventArgs
{
    public string headline;
    public string text;
    public bool showX;
    public UnityAction yesAction;
    public UnityAction noAction;

    public YesNoPopUpEventArgs(string headline, string text, UnityAction<bool> action, bool showX = true)
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.yesAction = delegate { action.Invoke(true); };
        this.noAction = delegate { action.Invoke(false); };
    }

    public YesNoPopUpEventArgs(string headline, string text, UnityAction yesAction, UnityAction noAction, bool showX = true)
    {
        this.headline = headline;
        this.text = text;
        this.showX = showX;
        this.yesAction = yesAction;
        this.noAction = noAction;
    }
}

