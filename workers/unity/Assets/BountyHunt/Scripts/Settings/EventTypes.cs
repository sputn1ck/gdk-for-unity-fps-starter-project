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
[Serializable] public class EarningsUpdateEvent : UnityEvent<EarningsUpdateEventArgs> { }
public class StringColorEvent : UnityEvent<string, Color> { }
[Serializable] public class ChatMessageEvent : UnityEvent<Chat.ChatMessage> { }
public class StringLongEvent : UnityEvent<string, long> { }


[Serializable]
public struct BountyUpdateEventArgs
{
    public long NewAmount;
    public long OldAmount;
    public Bountyhunt.BountyReason Reason;
}

[Serializable]
public struct EarningsUpdateEventArgs
{
    public long NewAmount;
    public long OldAmount;
}


