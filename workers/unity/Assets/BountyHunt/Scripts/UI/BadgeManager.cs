using Bbhrpc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BadgeManager : MonoBehaviour
{
    static BadgeManager instance;
    public List<Badge> allBadges;

    private void Awake()
    {
        instance = this;
    }

    public static Badge GetBadge(RankBadge rankBadge)
    {
        return instance.allBadges.FirstOrDefault(b => b.rankBadge == rankBadge);
    }
}

[System.Serializable]
public class Badge
{
    
    public RankBadge rankBadge;
    public string League;
    public Sprite sprite;
    public Color color;
}
