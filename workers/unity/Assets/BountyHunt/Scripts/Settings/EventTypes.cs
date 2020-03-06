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
    public long DeamonBalance;
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

